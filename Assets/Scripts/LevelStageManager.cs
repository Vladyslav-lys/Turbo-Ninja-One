using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelStageManager : MonoBehaviour
{
    public Image[] stages;
    public TextMeshProUGUI[] texts;
    public Color[] colors;
    
    void Start()
    {
        int level = PlayerPrefs.GetInt("Level");
        int multiplier, remainder;
        
        //multiplier = Math.DivRem(level, level % 5 == 0 ? 6 : 5, out remainder);
        multiplier = Math.DivRem(level, 5, out remainder);

        if(level % 5 == 0)
        {
            for(int i = 1; i < texts.Length + 1; i++)
            {
                texts[i - 1].text = (i + (multiplier - 1) * 5).ToString();
            }
            foreach(Image img in stages)
            {
                img.color = colors[0];
            }
            return;
        }
        for(int i = 1; i < texts.Length + 1; i++)
        {
            texts[i - 1].text = (i + multiplier * 5).ToString();
        }
        for(int i = 0; i < remainder; i++)
        {
            stages[i].color = colors[i + 1 == remainder ? 1 : 0 ];
        }
    }
}
