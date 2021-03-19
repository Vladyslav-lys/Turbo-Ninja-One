using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CutObject : MonoBehaviour
{
    public GameManager gm;
    public GameObject plane;
    public GameObject scoreTextPref;
    public Transform ObjectContainer;

    public bool isRight;
    
    // How far away from the slice do we separate resulting objects
    public float separation;

    // Do we draw a plane object associated with the slice
    private Plane slicePlane = new Plane();
    public bool drawPlane;
    public GameObject slashEffect;
    
    private MeshCutter meshCutter;
    private TempMesh biggerMesh, smallerMesh;
    
    // Use this for initialization
    void Start () {
        // Initialize a somewhat big array so that it doesn't resize
        meshCutter = new MeshCutter(256);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        // 12 it's cuttable objects
        if (other.gameObject.layer == 12)
        {
            Quaternion rot = transform.rotation;
            rot *= isRight ? Quaternion.Euler(0f, 0f ,0f) : Quaternion.Euler(0f, 180f, 0f);
            gm.PlaySoundAfterCreating(gm.hitSound);
            SliceObjects(transform.position, rot.ToEulerAngles(), other.gameObject, false);
            PushSliceObj(new Vector3(0f,300f,0f), other.gameObject.GetComponent<Rigidbody>());
            gm.Score(1);
            if(scoreTextPref)
            {
                scoreTextPref.GetComponent<CutScoreCanvas>().scoreText.text = "+1";
                Vector3 spawnPos = new Vector3(other.transform.position.x, other.transform.position.y + 1f, other.transform.position.z - 1f);
                Destroy(Instantiate(scoreTextPref, spawnPos, Quaternion.identity), 2f);
            }
            //Destroy(other.gameObject, 2f);
        }
        // 21 it's enemies
        if (other.gameObject.layer == 21)
        {
            GameObject sliceObj = other.GetComponent<EnemyController>().Cut();
            if(sliceObj)
            {
                gm.PlaySoundAfterCreating(gm.bloodSound);
                gm.PlaySoundAfterCreating(gm.hitSound);
                PushSliceObj(new Vector3(0f, 300f, 0f), sliceObj.GetComponent<Rigidbody>());
                sliceObj.layer = 13;
                gm.Score(2);
                if (scoreTextPref)
                {
                    scoreTextPref.GetComponent<CutScoreCanvas>().scoreText.text = "+2";
                    Vector3 spawnPos = new Vector3(other.transform.position.x, other.transform.position.y + 1f, other.transform.position.z - 1f);
                    Destroy(Instantiate(scoreTextPref, spawnPos, Quaternion.identity), 2f);
                }
                //Destroy(sliceObj, 2f);
            }
        }
        // 9 it's player
        if (other.gameObject.layer == 9)
        {
            GameObject sliceObj = other.GetComponent<PlayerMovement>().Cut();
            gm.PlaySoundAfterCreating(gm.bloodSound);
            gm.PlaySoundAfterCreating(gm.hitSound);
            SliceObjects(transform.position, Vector3.up, sliceObj, true);
            PushSliceObj(new Vector3(0f,300f,0f), sliceObj.GetComponent<Rigidbody>());
        }
    }
    
    public void PushSliceObj(Vector3 forces, Rigidbody slicedObjRb) => slicedObjRb.AddForce(forces);
    
     void SliceObjects(Vector3 point, Vector3 normal, GameObject sliceObj, bool isPlayer)
    {
        // Put results in positive and negative array so that we separate all meshes if there was a cut made
        List<Transform> positive = new List<Transform>(), negative = new List<Transform>();
        
        bool slicedAny = false;
        // We multiply by the inverse transpose of the worldToLocal Matrix, a.k.a the transpose of the localToWorld Matrix
        // Since this is how normal are transformed
        var transformedNormal = ((Vector3)(sliceObj.transform.localToWorldMatrix.transpose * normal)).normalized;

        //Convert plane in object's local frame
        slicePlane.SetNormalAndPosition(
            transformedNormal,
            sliceObj.transform.InverseTransformPoint(point));

        slicedAny = SliceObject(ref slicePlane, sliceObj, positive, negative, isPlayer) || slicedAny;
        Destroy(Instantiate(slashEffect, transform.position, Quaternion.identity), 1f);

        // Separate meshes if a slice was made
        if (slicedAny)
            SeparateMeshes(positive, negative, normal);
    }

    bool SliceObject(ref Plane slicePlane, GameObject obj, List<Transform> positiveObjects, List<Transform> negativeObjects,  bool isPlayer)
    {
        var mesh = obj.GetComponent<MeshFilter>().mesh;
        
        if (!meshCutter.SliceMesh(mesh, ref slicePlane))
        {
            // Put object in the respective list
            if (slicePlane.GetDistanceToPoint(meshCutter.GetFirstVertex()) >= 0)
                positiveObjects.Add(obj.transform);
            else
                negativeObjects.Add(obj.transform);

            return false;
        }

        // TODO: Update center of mass

        // Silly condition that labels which mesh is bigger to keep the bigger mesh in the original gameobject
        bool posBigger = meshCutter.PositiveMesh.surfacearea > meshCutter.NegativeMesh.surfacearea;
        if (posBigger)
        {
            biggerMesh = meshCutter.PositiveMesh;
            smallerMesh = meshCutter.NegativeMesh;
        }
        else
        {
            biggerMesh = meshCutter.NegativeMesh;
            smallerMesh = meshCutter.PositiveMesh;
        }

        // Create new Sliced object with the other mesh
        GameObject newObject = Instantiate(obj, ObjectContainer);
        newObject.transform.SetPositionAndRotation(obj.transform.position, obj.transform.rotation);
        var newObjMesh = newObject.GetComponent<MeshFilter>().mesh;
        
        //add gravity and disable kinematic to new game object
        obj.GetComponent<Rigidbody>().useGravity = true;
        obj.GetComponent<Rigidbody>().isKinematic = false;
        newObject.layer = 13;
        obj.layer = 13;
        //Recalculate box collider
        Destroy(obj.GetComponent<BoxCollider>());
        // obj.AddComponent<MeshCollider>();
        // obj.GetComponent<MeshCollider>().convex = true;
        
        obj.AddComponent<SphereCollider>();
        obj.GetComponent<Rigidbody>().AddForce(isPlayer ? 10f : 0f, gm.cutObjGravityCoefficient*10f, 0f);
        //obj.GetComponent<SphereCollider>().isTrigger = true;
        // obj.GetComponent<SphereCollider>().radius = 0.5f;
        // obj.GetComponent<SphereCollider>().center = new Vector3(0f,1f,0f);
        
        // Put the bigger mesh in the original object
        // TODO: Enable collider generation (either the exact mesh or compute smallest enclosing sphere)
        ReplaceMesh(mesh, biggerMesh);
        ReplaceMesh(newObjMesh, smallerMesh);

        (posBigger ? positiveObjects : negativeObjects).Add(obj.transform);
        (posBigger ? negativeObjects : positiveObjects).Add(newObject.transform);

        return true;
    }


    /// <summary>
    /// Replace the mesh with tempMesh.
    /// </summary>
    void ReplaceMesh(Mesh mesh, TempMesh tempMesh, MeshCollider collider = null)
    {
        mesh.Clear();
        mesh.SetVertices(tempMesh.vertices);
        mesh.SetTriangles(tempMesh.triangles, 0);
        mesh.SetNormals(tempMesh.normals);
        mesh.SetUVs(0, tempMesh.uvs);
        
        //mesh.RecalculateNormals();
        mesh.RecalculateTangents();

        if (collider != null && collider.enabled)
        {
            collider.sharedMesh = mesh;
            collider.convex = true;
        }
    }

    void SeparateMeshes(List<Transform> positives, List<Transform> negatives, Vector3 worldPlaneNormal)
    {
        int i;
        var separationVector = worldPlaneNormal * separation;

        for(i = 0; i <positives.Count; ++i)
            positives[i].transform.position += separationVector;

        for (i = 0; i < negatives.Count; ++i)
            negatives[i].transform.position -= separationVector;
    }

}
