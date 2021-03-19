using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool isStarted;
    public bool debugLevel;
    public int level;
    public int gold;
    public int currentPlayerSkin;
    public int currentSwordSkin;
    public Color[] playerColorsBySkin;
    public Skin[] playerSkins;
    public Skin[] swordSkins;
    public List<GameObject> levels;
    public PlayerMovement playerMove;
    public UIManager uiManager;
    public float curTimeScale;
    public GameObject confetti;
    public TargetCamera targetCamera;
    public TargetCamera runParticleTarget;
    public GameObject hitSound;
    public GameObject scoreSound;
    public GameObject bloodSound;
    public GameObject startGameSound;
    public GameObject jumpSound;
    public GameObject finishSound;
    public GameObject glassSound;
    public GameObject landingSound;

    #region Coefficient
    public float speedCoefficient;
    public float maxSpeedCoefficient;
    public float jumpCoefficient;
    public float sensitivityCoefficient;
    public float dashCoefficient;
    public float gravityCoefficient;
    public float timeCoefficient;
    public float enemySpeedCoefficient;
    public float cutObjGravityCoefficient;
    #endregion
    
    private void Awake()
    {
        InitPrefs();
        InitSettings();

        gold = 0;
        
        if(debugLevel)
        {
            PlayerPrefs.SetInt("Level",level);
        }
        level = PlayerPrefs.GetInt("Level");
        levels[level - 1].SetActive(true);

        currentPlayerSkin = PlayerPrefs.GetInt("CurrentPlayerSkin");
        currentSwordSkin = PlayerPrefs.GetInt("CurrentSwordSkin");
        playerSkins[currentPlayerSkin].SetAbleSkins(true);
        swordSkins[currentSwordSkin].SetAbleSkins(true);
        playerMove.ChangeColor();
        playerMove.SetSwords();
        
        uiManager.startGoldText.text = PlayerPrefs.GetInt("Coins").ToString();
    }

    private void Start()
    {
        Time.timeScale = timeCoefficient*curTimeScale;
    }

    public void NextLevel()
    {
        level++;
        if(level > levels.Count)
        {
            PlayerPrefs.SetInt("Level", 1);
        }
        else
        {  
            PlayerPrefs.SetInt("Level", level);
        }
        Restart();
    }

    public void StartPlay()
    {
        playerMove.isDash = true;
        isStarted = true;
        playerMove.runParticles.Play();
        uiManager.StartPlay();
        playerMove.playerAnimator.SetBool("Run", true);
        PlaySoundAfterCreating(startGameSound);
        Invoke(nameof(OnFakeTrail),0.2f);
    }

    private void OnFakeTrail()
    {
        playerMove.OnFakeKatana();
        playerMove.isDash = false;
    }

    public void Restart() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    public void Score(int gold)
    {
        this.gold += gold;
        uiManager.goldText.text = this.gold.ToString();
    }

    private void InitPrefs()
    {
        if(!PlayerPrefs.HasKey("Level"))
            PlayerPrefs.SetInt("Level", 1);
        
        if(!PlayerPrefs.HasKey("Audio"))
            PlayerPrefs.SetInt("Audio", 1);

        if (!PlayerPrefs.HasKey("IsVibration"))
            PlayerPrefs.SetInt("IsVibration", 1);
        
        if(!PlayerPrefs.HasKey("CurrentPlayerSkin"))
            PlayerPrefs.SetInt("CurrentPlayerSkin",0);
        
        if(!PlayerPrefs.HasKey("CurrentSwordSkin"))
            PlayerPrefs.SetInt("CurrentSwordSkin",0);
        
        if(!PlayerPrefs.HasKey("Coins"))
            PlayerPrefs.SetInt("Coins",0);
    }
    
    public void InitSettings()
    {
        speedCoefficient = PlayerPrefs.GetFloat("SpeedCoefficient", speedCoefficient);
        maxSpeedCoefficient = PlayerPrefs.GetFloat("MaxSpeedCoefficient", maxSpeedCoefficient);
        jumpCoefficient = PlayerPrefs.GetFloat("JumpCoefficient", jumpCoefficient);
        sensitivityCoefficient = PlayerPrefs.GetFloat("SensitivityCoefficient", sensitivityCoefficient);
        dashCoefficient = PlayerPrefs.GetFloat("DashCoefficient", dashCoefficient);
        gravityCoefficient = PlayerPrefs.GetFloat("GravityCoefficient", gravityCoefficient);
        timeCoefficient = PlayerPrefs.GetFloat("TimeCoefficient", timeCoefficient);
        enemySpeedCoefficient = PlayerPrefs.GetFloat("EnemySpeedCoefficient", enemySpeedCoefficient);
        cutObjGravityCoefficient = PlayerPrefs.GetFloat("CutObjGravityCoefficient", cutObjGravityCoefficient);
        Physics.gravity = new Vector3(0f,gravityCoefficient*(-9.81f),0f);
    }
    
    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("SpeedCoefficient", speedCoefficient);
        PlayerPrefs.SetFloat("MaxSpeedCoefficient", maxSpeedCoefficient);
        PlayerPrefs.SetFloat("JumpCoefficient", jumpCoefficient);
        PlayerPrefs.SetFloat("SensitivityCoefficient", sensitivityCoefficient);
        PlayerPrefs.SetFloat("DashCoefficient", dashCoefficient);
        PlayerPrefs.SetFloat("GravityCoefficient", gravityCoefficient);
        PlayerPrefs.SetFloat("TimeCoefficient", timeCoefficient);
        PlayerPrefs.SetFloat("EnemySpeedCoefficient", enemySpeedCoefficient);
        PlayerPrefs.SetFloat("CutObjGravityCoefficient", cutObjGravityCoefficient);
    }

    public void Finish(PlayerMovement playerMovement)
    {
        Vector3 playerPosition = playerMovement.gameObject.transform.position;
        Instantiate(confetti, playerPosition + new Vector3(0f, 10f, 0f), Quaternion.identity);
        Instantiate(confetti, playerPosition + new Vector3(4f, 6f, 0f), Quaternion.identity);
        Instantiate(confetti, playerPosition + new Vector3(-4f, 6f, 0f), Quaternion.identity);
        targetCamera.target = playerMovement.transform;
        targetCamera.isFinished = true;
        runParticleTarget.isFinished = true;
        playerMovement.Freeze();
        PlaySoundAfterCreating(finishSound);
        isStarted = false;
        uiManager.Finish();
        int gold = PlayerPrefs.GetInt("Coins") + this.gold;
        PlayerPrefs.SetInt("Coins", gold);
        StartCoroutine(FinishScore());
    }

    public void Lose()
    {
        isStarted = false;
        targetCamera.target = null;
        uiManager.Lose();
        //Time.timeScale = 0;
    }

    private IEnumerator FinishScore()
    {
        for(int curScore=0; curScore<=gold; curScore++)
        {
            uiManager.finishGoldText.text = "+" + curScore;
            yield return new WaitForSeconds(0.05f);
        }
    }

    public void PlaySoundAfterCreating(GameObject soundPref)
    {
        if(PlayerPrefs.GetInt("Audio") == 0 || !soundPref)
            return;
        
        GameObject soundObj = GameObject.Instantiate(soundPref);
        soundObj.transform.SetParent(Camera.main.transform);
        soundObj.GetComponent<AudioSource>().Play();
        Destroy(soundObj, 1f);
    }

    public void SetPlayerSkin(Skin skin)
    {
        playerSkins[currentPlayerSkin].SetAbleSkins(false);
        skin.SetAbleSkins(true);
    }
    
    public void SetSwordSkin(Skin skin)
    {
        swordSkins[currentSwordSkin].SetAbleSkins(false);
        skin.SetAbleSkins(true);
    }
    
    public void SavePlayerSkin(int choosenPlayerSkin)
    {
        PlayerPrefs.SetInt("CurrentPlayerSkin", choosenPlayerSkin);
        currentPlayerSkin = PlayerPrefs.GetInt("CurrentPlayerSkin");
        playerMove.SetSwords();
    }
    
    public void SaveSwordSkin(int choosenSwordSkin)
    {
        PlayerPrefs.SetInt("CurrentSwordSkin", choosenSwordSkin);
        currentSwordSkin = PlayerPrefs.GetInt("CurrentSwordSkin");
        playerMove.SetSwords();
    }
}