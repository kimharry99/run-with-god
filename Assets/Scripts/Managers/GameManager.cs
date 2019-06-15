using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonBehaviour<GameManager>
{
	private float _playtime;
	public float Playtime
	{
		get
		{
			return _playtime;
		}
		private set
		{
			//Update IngameUIManager's playtimer UI
		}
	}

	private int _lifeCount;
	public int LifeCount
	{
		get
		{
			return _lifeCount;
		}
		private set
		{
			//Update IngameUIManager's life UI
		}
	}

	private void OnEnable()
	{
		//Reset GameManager
	}

	private void Update()
	{
		//Update Playtime
	}
}
