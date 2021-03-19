using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public GameObject offImageViabrationObj;
    public GameObject offImageSoundObj;

    private void Start()
    {
        if (PlayerPrefs.GetInt("Audio") != 0)
        {
            offImageSoundObj.SetActive(false);
        }
        else
        {
            offImageSoundObj.SetActive(true);
        }

        if (PlayerPrefs.GetInt("IsVibration") != 0)
        {
            offImageViabrationObj.SetActive(false);
        }
        else
        {
            offImageViabrationObj.SetActive(true);
        }
    }
}
