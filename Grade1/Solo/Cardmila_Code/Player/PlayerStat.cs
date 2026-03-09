using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerStat : Singleton<PlayerStat>
{
    [SerializeField] private int playerMaxHp = 100;
    [SerializeField] private int playerHp = 100;
    [SerializeField] private int playerShield;
    [SerializeField] private int playerCost = 3;
    [SerializeField] private int PlayerEXP;
    [SerializeField] CostTextSet CostTextSet;
    TurnManager turnManager;
    [SerializeField]int nowExp = 0;
    int levelUpExp = 100;
    LevelManager levelManager;
    [SerializeField] private Image image;
    [SerializeField] Image gameover;
    [SerializeField] private GameOverManager gameoverManager;
    [SerializeField] Image hpbar;
    [SerializeField] TextMeshProUGUI shieldTxt;
    [SerializeField] Image shieldImage;
    [SerializeField] AudioSource plHit;
    private void Awake()
    {
        playerCost = 3;
        playerHp = 100;
        playerMaxHp = 100;
        nowExp = 0;
        gameover.gameObject.SetActive(false);
        levelManager  =GetComponent<LevelManager>();
        turnManager = TurnManager.Instance;
        turnManager.MyTurnstartEvent += CostHel;
        image.fillAmount = 0;
    }
    public void PlEXPUp(int a)
    {
        nowExp += a;
        //Debug.Log("nowExp" + nowExp);
        if (nowExp > levelUpExp)
        {
            nowExp = 0;
            levelManager.LevelUpWow();
            playerCost++;
            PlayerMaxCost++;
            CostTextSet.SetText(PlayerCost, PlayerMaxCost);
        }
        float d = (float)nowExp / levelUpExp;
        image.fillAmount = d;

    }
    
    public int PlayerMaxCost = 3;
    public int PlayerHp
    {
        get
        {
            return playerHp;
        }
        set
        {
            playerHp = value;
            
            if (playerHp <= 0)
            {
                Die();
            }
            float f = (float)playerHp / playerMaxHp;
            hpbar.fillAmount = f;
        }
    }
    
    public int PlayerShield
    {
        get
        {
            return playerShield;
        }
        set
        {
            plHit.Play();
            playerShield = value;
            shieldImage.enabled = true;
            shieldTxt.enabled = true;
            if (playerShield <= 0)
            {
                PlayerHp = PlayerHp + playerShield;
                playerShield = 0;
                shieldImage.enabled = false;
                shieldTxt.enabled = false;
            }
            shieldTxt.text = playerShield.ToString();
        }
    }
    
    public int PlayerCost
    {
        get
        {
            return playerCost;
        }
        set
        {
            playerCost = value;
        }
    }
    
    private void Die()
    {
        Time.timeScale = 0;
        gameover.gameObject.SetActive(true);
        gameoverManager.Gameover();
    }
    private void CostHel()
    {
        playerCost = PlayerMaxCost;
        CostTextSet.SetText(PlayerCost,PlayerMaxCost);
    }
}
