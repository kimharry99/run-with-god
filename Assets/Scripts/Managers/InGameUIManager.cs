using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class InGameUIManager : SingletonBehaviour<InGameUIManager>
{
	public Text playtimeText;
	public Text killCountText;

	public RectTransform lifeGrid;
	public GameObject lifeUIPrefab;

	public GameObject pauseUIPanel;

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
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
    }

    public void UpdatePlaytimeText(float playtime)
	{
        //Update playtimeText's text as input
        string minutes = ((int)playtime / 60).ToString();
        string seconds = (playtime % 60).ToString("f0");
        playtimeText.text = minutes + ":" + seconds;
	}
	
	public void UpdateKillCountText(int killCount)
	{
        //Update killCountText's text as input
        killCountText.text = killCount.ToString();
	}

	public void UpdateLifeUI(int lifeCount)
	{
		//Update LifeUI as input
	}

	public void OpenPauseUI()
	{

	}
}
