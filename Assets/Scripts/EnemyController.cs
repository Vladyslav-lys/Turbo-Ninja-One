using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public GameManager gm;
    public Animator enemyAnimator;
    public CutObject swordCut;
    public Transform targetTransform;
    public GameObject slicedEnemy;
    public GameObject skinMeshRenderer;
    public GameObject bloodEffect;
    public float speed;
    private bool _isRun;

    private void Start()
    {
        swordCut.gm = gm;
        speed *= gm.enemySpeedCoefficient;
    }

    public GameObject Cut()
    {
        if (slicedEnemy.layer == 13)
            return null;

        Destroy(Instantiate(bloodEffect, transform.position + new Vector3(0f, 0.3f, 0.7f), Quaternion.identity), 1f);
        skinMeshRenderer.SetActive(false);
        slicedEnemy.SetActive(true);
        return slicedEnemy;
    }

    private void Update()
    {
        if (slicedEnemy.activeSelf)
            enabled = false;
        
        if (_isRun)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetTransform.position, speed * Time.deltaTime); 
            transform.rotation = Quaternion.LookRotation(targetTransform.position - transform.position, Vector3.up);
        }
    }

    public void SetRun()
    {
        _isRun = true;
        enemyAnimator.transform.localRotation = Quaternion.Euler(Vector3.zero);
        enemyAnimator.SetBool("Run", true);
    }

    public void SetIdle()
    {
        if(!targetTransform)
            return;

        _isRun = false;
        enemyAnimator.transform.localRotation = Quaternion.Euler(new Vector3(0f,-180f,0f));
        enemyAnimator.SetBool("Run", false);
    }
}
