using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public GameManager gm;
    public GameObject plane;
    public Transform objContainer;
    public Transform targetPlayerTransform;
    public List<EnemyController> enemies;
    public List<GameObject> buildings;
    
    private void Awake()
    {
        if(enemies.Count <= 0 || !targetPlayerTransform)
            return;

        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].gm = gm;
            enemies[i].targetTransform = targetPlayerTransform;
            enemies[i].swordCut.plane = plane;
            enemies[i].swordCut.ObjectContainer = objContainer;
        }
        
        foreach (var building in buildings)
        {
            StaticBatchingUtility.Combine(building); 
        }
    }
}
