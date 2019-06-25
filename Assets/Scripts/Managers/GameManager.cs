using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonBehaviour<GameManager>
{
	private float _playtime;
	public float Playtime
	{
		get	{ return _playtime;	}
		private set
		{
			_playtime = value;
			InGameUIManager.inst.UpdatePlaytimeText(_playtime);
		}
	}

	private int _killCount;
	public int KillCount
	{
		get { return _killCount; }
		set
		{
			_killCount = value;
			InGameUIManager.inst.UpdateKillCountText(_killCount);
		}
	}

	private StateMachine gameState = new StateMachine();

	private void OnEnable()
	{
		//Reset GameManager
	}

	private void Update()
	{
		Playtime += Time.deltaTime;
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
