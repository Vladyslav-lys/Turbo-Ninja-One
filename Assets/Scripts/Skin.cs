using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skin : MonoBehaviour
{
    public GameObject[] skins;

    public void SetAbleSkins(bool isEnable)
    {
        if(skins.Length <= 0)
            return;
        
        for (int i = 0; i < skins.Length; i++)
        {
            skins[i].SetActive(isEnable);
        }
    }
}
