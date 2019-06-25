using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class InGameUIManager : SingletonBehaviour<InGameUIManager>
{
	public Text playtimeText;
	public Text killCountText;

	public GameObject[] lifeUIs = new GameObject[3];

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
	}
	
	public void UpdateKillCountText(int killCount)
	{
		//Update killCountText's text as input
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
}
