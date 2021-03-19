using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TestSettings : MonoBehaviour
{
    public GameManager gm;
    
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI maxSpeedText;
    public TextMeshProUGUI jumpText;
    public TextMeshProUGUI sensitivityText;
    public TextMeshProUGUI dashText;
    public TextMeshProUGUI gravityText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI enemySpeedText;
    public TextMeshProUGUI cutObjGravityText;
    
    public Slider speedSlider;
    public Slider maxSpeedSlider;
    public Slider jumpSlider;
    public Slider sensitivitySlider;
    public Slider dashSlider;
    public Slider gravitySlider;
    public Slider timeSlider;
    public Slider enemySpeedSlider;
    public Slider cutObjGravitySlider;

    private void Start()
    {
        speedSlider.onValueChanged.AddListener(delegate {OnSpeedChange();});//
        maxSpeedSlider.onValueChanged.AddListener(delegate {OnMaxSpeedChange();});//
        jumpSlider.onValueChanged.AddListener(delegate {OnJumpChange();});//
        sensitivitySlider.onValueChanged.AddListener(delegate {OnSensitivityChange();});//
        dashSlider.onValueChanged.AddListener(delegate {OnDashChange();});//
        gravitySlider.onValueChanged.AddListener(delegate {OnGravityChange();});//
        timeSlider.onValueChanged.AddListener(delegate {OnTimeChange();});//
        enemySpeedSlider.onValueChanged.AddListener(delegate {OnEnemySpeedChange();});//
        cutObjGravitySlider.onValueChanged.AddListener(delegate {OnCutObjGravityChange();});//
    }
    
    private void OnEnable()
    {
        speedText.text = Math.Round(gm.speedCoefficient,3).ToString();
        speedSlider.value = gm.speedCoefficient / 2f;
        
        maxSpeedText.text = Math.Round(gm.maxSpeedCoefficient,3).ToString();
        maxSpeedSlider.value = gm.maxSpeedCoefficient / 2f;
        
        jumpText.text = Math.Round(gm.jumpCoefficient,3).ToString();
        jumpSlider.value = gm.jumpCoefficient / 2f;
        
        sensitivityText.text = Math.Round(gm.sensitivityCoefficient,3).ToString();
        sensitivitySlider.value = gm.sensitivityCoefficient / 2f;
        
        dashText.text = Math.Round(gm.dashCoefficient,3).ToString();
        dashSlider.value = gm.dashCoefficient / 2f;
        
        gravityText.text = Math.Round(gm.gravityCoefficient,3).ToString();
        gravitySlider.value = gm.gravityCoefficient / 2f;
            
        timeText.text = Math.Round(gm.timeCoefficient,3).ToString();
        timeSlider.value = gm.timeCoefficient / 2f;
        
        enemySpeedText.text = Math.Round(gm.enemySpeedCoefficient,3).ToString();
        enemySpeedSlider.value = gm.enemySpeedCoefficient / 2f;
        
        cutObjGravityText.text = Math.Round(gm.cutObjGravityCoefficient,3).ToString();
        cutObjGravitySlider.value = gm.cutObjGravityCoefficient / 2f;
    }

    private void OnDisable()
    {
        gm.SaveSettings();
    }

    private void OnSpeedChange()
    {
        gm.speedCoefficient = 2*speedSlider.value;
        speedText.text = Math.Round(gm.speedCoefficient,3).ToString();
    }
    
    private void OnMaxSpeedChange()
    {
        gm.maxSpeedCoefficient = 2*maxSpeedSlider.value;
        maxSpeedText.text = Math.Round(gm.maxSpeedCoefficient,3).ToString();
    }
    
    private void OnJumpChange()
    {
        gm.jumpCoefficient = 2*jumpSlider.value;
        jumpText.text = Math.Round(gm.jumpCoefficient,3).ToString();
    }
    
    private void OnSensitivityChange()
    {
        gm.sensitivityCoefficient = 2*sensitivitySlider.value;
        sensitivityText.text = Math.Round(gm.sensitivityCoefficient,3).ToString();
    }
    
    private void OnDashChange()
    {
        gm.dashCoefficient = 2*dashSlider.value;
        dashText.text = Math.Round(gm.dashCoefficient,3).ToString();
    }
    
    private void OnGravityChange()
    {
        gm.gravityCoefficient = 2*gravitySlider.value;
        Physics.gravity = new Vector3(0f,gm.gravityCoefficient*(-9.81f),0f);
        gravityText.text = Math.Round(gm.gravityCoefficient,3).ToString();
    }
    
    private void OnTimeChange()
    {
        gm.timeCoefficient = 2*timeSlider.value;
        timeText.text = Math.Round(gm.timeCoefficient,3).ToString();
    }
    
    private void OnEnemySpeedChange()
    {
        gm.enemySpeedCoefficient = 2*enemySpeedSlider.value;
        enemySpeedText.text = Math.Round(gm.enemySpeedCoefficient,3).ToString();
    }
    
    private void OnCutObjGravityChange()
    {
        gm.cutObjGravityCoefficient = 2*cutObjGravitySlider.value;
        cutObjGravityText.text = Math.Round(gm.cutObjGravityCoefficient,3).ToString();
    }

    public void ResetSetting()
    {
        PlayerPrefs.SetFloat("SpeedCoefficient", 1);
        PlayerPrefs.SetFloat("MaxSpeedCoefficient", 1);
        PlayerPrefs.SetFloat("JumpCoefficient", 1);
        PlayerPrefs.SetFloat("SensitivityCoefficient", 1);
        PlayerPrefs.SetFloat("DashCoefficient", 1);
        PlayerPrefs.SetFloat("GravityCoefficient", 1);
        PlayerPrefs.SetFloat("TimeCoefficient", 1);
        PlayerPrefs.SetFloat("EnemySpeedCoefficient", 1);
        PlayerPrefs.SetFloat("CutObjGravityCoefficient", 1);
        gm.InitSettings();
        this.OnEnable();
    }
}
