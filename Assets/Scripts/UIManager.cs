using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject playCanvas;
    public GameObject dragPanel;
    public GameObject losePanel;
    public GameObject finishPanel;
    public GameObject pausePanel;
    public GameObject settingsPanel;
    public GameObject tutorialPanel;
    public GameObject shopPanel;
    public GameObject creditsPanel;
    public GameManager gm;
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI finishGoldText;
    public TextMeshProUGUI startGoldText;

    private void Start()
    {
        mainMenu.SetActive(true);
    }

    public void StartPlay()
    {
        mainMenu.SetActive(false);
        playCanvas.SetActive(true);

        //PlayerPrefs.DeleteKey("Tutorial");
        //if(!PlayerPrefs.HasKey("Tutorial"))
        //    ShowTutorial();
    }

    public void Lose()
    {
        dragPanel.SetActive(false);
        playCanvas.SetActive(false);
        losePanel.SetActive(true);
    }

    public void Finish()
    {
        dragPanel.SetActive(false);
        playCanvas.SetActive(false);
        finishPanel.SetActive(true);
    }
    
    public void OnOffVibration(GameObject offImageObj)
    {
        if (PlayerPrefs.GetInt("IsVibration") != 0)
        {
            PlayerPrefs.SetInt("IsVibration", 0);
            offImageObj.SetActive(true);
        }
        else
        {
            PlayerPrefs.SetInt("IsVibration", 1);
            offImageObj.SetActive(false);
        }
    }
    
    public void OnOffSound(GameObject offImageObj)
    {
        if(PlayerPrefs.GetInt("Audio") != 0)
        {
            PlayerPrefs.SetInt("Audio", 0);
            offImageObj.SetActive(true);
        }
        else
        {
            PlayerPrefs.SetInt("Audio", 1);
            offImageObj.SetActive(false);
        }
    }

    public void OpenPause()
    {
        Time.timeScale = 0;
        dragPanel.SetActive(false);
        playCanvas.SetActive(false);
        pausePanel.SetActive(true);
    }

    public void ClosePause()
    {
        Time.timeScale = gm.timeCoefficient*gm.curTimeScale;
        dragPanel.SetActive(true);
        playCanvas.SetActive(true);
        pausePanel.SetActive(false);
    }
    
    public void OpenSettings()
    {
        dragPanel.SetActive(false);
        mainMenu.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        dragPanel.SetActive(true);
        mainMenu.SetActive(true);
        settingsPanel.SetActive(false);
    }
    
    public void OpenShop()
    {
        dragPanel.SetActive(false);
        mainMenu.SetActive(false);
        shopPanel.SetActive(true);
    }

    public void CloseShop()
    {
        dragPanel.SetActive(true);
        mainMenu.SetActive(true);
        shopPanel.SetActive(false);
    }
    
    public void ChooseShop(GameObject shop)
    {
        Shop shopScript = shopPanel.GetComponent<Shop>();
        
        shopScript.currentShop.SetActive(false);
        shop.SetActive(true);
        shopScript.currentShop = shop;
    }

    public void ChooseBtn(GameObject btn)
    {
        Shop shopScript = shopPanel.GetComponent<Shop>();

        // shopScript.currentShopBtn.GetComponent<RectTransform>().localPosition =
        //     new Vector2(shopScript.currentShopBtn.GetComponent<RectTransform>().localPosition.x, 310);
        // btn.GetComponent<RectTransform>().localPosition =
        //     new Vector2(btn.GetComponent<RectTransform>().localPosition.x, 280);
        shopScript.currentShopBtn.GetComponent<Button>().interactable = true;
        btn.GetComponent<Button>().interactable = false;
        shopScript.currentShopBtn = btn;
    }
    
    public void PlayerBtnChoosed(Button btn)
    {
        Shop shopScript = shopPanel.GetComponent<Shop>();
    
        shopScript.currentPlayerBtn.GetComponent<Button>().interactable = true;
        btn.interactable = false;
        shopScript.currentPlayerBtn = btn.gameObject;
    }
    
    public void SwordBtnChoosed(Button btn)
    {
        Shop shopScript = shopPanel.GetComponent<Shop>();
    
        shopScript.currentSwordBtn.GetComponent<Button>().interactable = true;
        btn.interactable = false;
        shopScript.currentSwordBtn = btn.gameObject;
    }

    public void OpenCredits()
    {
        settingsPanel.SetActive(false);
        creditsPanel.SetActive(true);
    }

    public void CloseCredits()
    {
        settingsPanel.SetActive(true);
        creditsPanel.SetActive(false);
    }

    public void ShowTutorial()
    {
        Time.timeScale = 0;
        tutorialPanel.SetActive(true);
    }

    public void HideTutorial()
    {
        Time.timeScale = 1.3f;
        PlayerPrefs.SetInt("Tutorial", 1);
        tutorialPanel.SetActive(false);
    }
}
