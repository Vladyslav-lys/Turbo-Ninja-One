using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public GameObject[] shops;
    public GameObject[] shopBtns;
    public Button[] shopPlayerBtns;
    public Button[] shopSwordBtns;
    public GameObject currentShopBtn;
    public GameObject currentShop;
    public GameObject currentPlayerBtn;
    public GameObject currentSwordBtn;

    private void Start()
    {
        currentPlayerBtn.GetComponent<Button>().interactable = true;
        shopPlayerBtns[PlayerPrefs.GetInt("CurrentPlayerSkin")].interactable = false;
        currentPlayerBtn = shopPlayerBtns[PlayerPrefs.GetInt("CurrentPlayerSkin")].gameObject;
        
        currentSwordBtn.GetComponent<Button>().interactable = true;
        shopSwordBtns[PlayerPrefs.GetInt("CurrentSwordSkin")].interactable = false;
        currentSwordBtn = shopSwordBtns[PlayerPrefs.GetInt("CurrentSwordSkin")].gameObject;
    }
}
