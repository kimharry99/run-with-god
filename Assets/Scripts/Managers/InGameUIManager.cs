﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class InGameUIManager : SingletonBehaviour<InGameUIManager>
{
	public Text playtimeText;
	public Text killCountText;
	public Text trustText;

	public GameObject[] lifeUIs = new GameObject[3];

	public GameObject pauseUIPanel;

	public Slider bossHealthUI;

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		bossHealthUI.gameObject.SetActive(scene.name.Contains("Boss"));
		gameObject.SetActive(scene.name != "TurstSelection");
		//gameObject.SetActive(scene.name == "InGameScene" || scene.name == "Boss");
		//Check Scene is ingame, boss scene
		//If not, disable gameobect
	}

    private void Start()
	{
		if (inst != this)
		{
			Destroy(gameObject);
			return;
		}
		else
			SetStatic();
		SceneManager.sceneLoaded += OnSceneLoaded;
		OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }
    public void UpdatePlaytimeText(float playtime)
	{
        //Update playtimeText's text as input
        string minutes = ((int)playtime / 60).ToString("00");
        string seconds = (playtime % 60).ToString("00");
        playtimeText.text = minutes + ":" + seconds;
	}
	
	public void UpdateKillCountText(int killCount)
	{
        //Update killCountText's text as input
        killCountText.text = killCount.ToString();
	}

	public void UpdateLifeUI(int lifeCount)
	{
		for(int i= 0; i<3; i++)
        {
            lifeUIs[i].SetActive(lifeCount > i);
        }
	}

	public void OpenPauseUI()
	{

	}

	public void UpdateTrustUI(Trust trust)
	{
		//Update Trust Text
	}

	public void UpdateBossHelthUI(float value)
	{
		bossHealthUI.value = value;
	}
}
