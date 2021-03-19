using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadTop : MonoBehaviour
{
    public List<EnemyController> enemies;
    public Transform lastTransform;
    
    public void SetRunAllEnemies()
    {
        if(enemies.Count <= 0)
            return;
        
        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].SetRun();
        }
    }
    
    public void SetIdleAllEnemies()
    {
        if(enemies.Count <= 0)
            return;
        
        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].SetIdle();
        }
    }
}
