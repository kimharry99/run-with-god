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

	private StateMachine gameState = new StateMachine();

	private void OnEnable()
	{
		//Reset GameManager
	}

	private void Update()
	{
		//Update Playtime
	}

	private void InitGameState()
	{
		State play = new State();
		State pause = new State();
		State load = new State();

		gameState.AddNewState("play", play);
		gameState.AddNewState("pause", pause);
		gameState.AddNewState("load", load);
	}
}
