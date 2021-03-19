using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetCamera : MonoBehaviour
{
    public Transform target;
    public GameManager gm;
    public Vector3 offset;
    private PlayerMovement _playerMovement;
    public bool isFinished;
    private ParticleSystem _runParticle;
    private Camera _cam;
    
    private void Start()
    {
        //_playerMovement = target.parent.GetComponent<PlayerMovement>();
        if (GetComponent<ParticleSystem>())
            _runParticle = GetComponent<ParticleSystem>();
        if (GetComponent<Camera>())
            _cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (!isFinished)
        {
            if(!gm.isStarted)
                return;
            
            Vector3 followPos = new Vector3(target.position.x, (target.position + offset).y, (target.position + offset).z);
            transform.position = followPos;
            //transform.position = Vector3.MoveTowards(transform.position, followPos, 30f * Time.deltaTime);
        }
        else
        {
            if (_runParticle && _runParticle.isPlaying)
                _runParticle.Stop();
            if(_cam)
                transform.RotateAround(target.position, Vector3.up, 10f * Time.deltaTime);
        }
    }
}
