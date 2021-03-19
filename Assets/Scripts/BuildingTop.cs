using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingTop : MonoBehaviour
{
    //public bool isTrigger;
    //public float enableTime;
    public List<GameObject> colliderObjects;
    
    private void OnEnable()
    {
        for (int i = 0; i < colliderObjects.Count; i++)
            colliderObjects[i].SetActive(false);
    }

    // private void OnCollisionEnter(Collision collision)
    // {
    //     if(isTrigger || collision.gameObject.layer != 9)
    //         return;
    //     
    //     Invoke(nameof(EnableColliders), enableTime);   
    // }

    // private void OnTriggerEnter(Collider other)
    // {
    //     if(!isTrigger || other.gameObject.layer != 9)
    //         return;
    //     
    //     Invoke(nameof(EnableColliders), enableTime);  
    // }

    public void EnableColliders(float enableTime) => Invoke(nameof(EnableColliders), enableTime);

    private void EnableColliders()
    {
        for (int i = 0; i < colliderObjects.Count; i++)
        {
            colliderObjects[i].SetActive(true);
        }
    }
}
