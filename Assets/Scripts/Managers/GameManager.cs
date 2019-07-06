using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            OnPlayTimeChanged?.Invoke(_playtime);
		}
	}
    public Action<float> OnPlayTimeChanged;

	private int _killCount;
	public int KillCount
	{
		get { return _killCount; }
		set
		{
			_killCount = value;
			OnKillCountChanged?.Invoke(_killCount);
		}
	}
	public Action<int> OnKillCountChanged;

	private StateMachine gameState = new StateMachine();
	public Trust SelectedTrust { get { return TrustSelector.SelectedTrust; } }

	private Dictionary<TrustType, int> trustTier = new Dictionary<TrustType, int>();
	private Dictionary<Tuple<TrustType, int>, List<Trust>> unplayedTrusts = new Dictionary<Tuple<TrustType, int>, List<Trust>>();
	private Dictionary<Tuple<TrustType, int>, List<Trust>> playedTrusts = new Dictionary<Tuple<TrustType, int>, List<Trust>>();

	private void OnEnable()
	{
		//Reset GameManager
	}

	private void Awake()
	{
		OnKillCountChanged += InGameUIManager.inst.UpdateKillCountText;

		InitGameState();

		trustTier.Add(TrustType.ATTACK, 0);
		trustTier.Add(TrustType.MOVE, 0);
		trustTier.Add(TrustType.ACTION, 0);

		foreach (var trust in Resources.LoadAll<Trust>("Trusts"))
		{
			Debug.Log(trust.name + " loaded");
			Tuple<TrustType, int> tuple = new Tuple<TrustType, int>(trust.type, trust.tier);
			if (!unplayedTrusts.ContainsKey(tuple))
			{
				unplayedTrusts.Add(tuple, new List<Trust>());
			}
			if (!playedTrusts.ContainsKey(tuple))
			{
				playedTrusts.Add(tuple, new List<Trust>());
			}
			unplayedTrusts[tuple].Add(trust);
		}

		SceneManager.sceneLoaded += OnSceneLoaded;
		OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
	}

#if UNITY_EDITOR
	private void OnGUI()
	{
		if (SceneManager.GetActiveScene().name != "TrustSelection")
			return;
		GUI.Label(new Rect(10, 10, 100, 20), "ATTACK : " + trustTier[TrustType.ATTACK]);
		trustTier[TrustType.ATTACK] = Mathf.RoundToInt(GUI.HorizontalSlider(new Rect(120, 10, 50, 20), trustTier[TrustType.ATTACK], -5, 5));
		GUI.Label(new Rect(10, 30, 100, 20), "MOVE : " + trustTier[TrustType.MOVE]);
		trustTier[TrustType.MOVE] = Mathf.RoundToInt(GUI.HorizontalSlider(new Rect(120, 30, 50, 20), trustTier[TrustType.MOVE], -5, 5));
		GUI.Label(new Rect(10, 50, 100, 20), "ACTION : " + trustTier[TrustType.ACTION]);
		trustTier[TrustType.ACTION] = Mathf.RoundToInt(GUI.HorizontalSlider(new Rect(120, 50, 50, 20), trustTier[TrustType.ACTION], -5, 5));
		if(GUI.Button(new Rect(10, 70, 200, 20), "Update Trustselector"))
		{
			foreach(var ts in FindObjectsOfType<TrustSelector>())
			{
				ts.InitTrustSelector(PickTrust(ts.type));
			}
		}
		int i = 0;
		foreach (var ts in FindObjectsOfType<TrustSelector>())
		{
			GUI.Label(new Rect(10, 90 + 20 * i++, 500, 20), ts.type + " : " + (ts.Trust != null ? ts.Trust.trustName : "Null"));
		}

	}
#endif
	private void Update()
	{
		gameState.UpdateStateMachine();
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (scene.name == "InGameScene" || scene.name == "Boss")
		{
			gameState.Transtion("play");
		}
		else
		{
			gameState.Transtion("pause");
		}
	}

	private void InitGameState()
	{
		State play = new State();
		State pause = new State();
		State load = new State();

		play.StateUpdate += delegate { Playtime += Time.deltaTime; };

		gameState.AddNewState("play", play);
		gameState.AddNewState("pause", pause);
		gameState.AddNewState("load", load);
	}

	public Trust PickTrust(TrustType type)
	{
		int tier = trustTier[type];
		Tuple<TrustType, int> tuple = new Tuple<TrustType, int>(type, tier);
		if (!unplayedTrusts.ContainsKey(tuple))
			return null;
		List<Trust> trusts = unplayedTrusts[tuple];
		if (trusts.Count < 1)
		{
			ResetPlayedTrusts(tuple);
		}
		if (trusts.Count < 1)
			return null;
		return trusts[UnityEngine.Random.Range(0, trusts.Count)];
	}

	public void PlayTrust(Trust trust)
	{
		Tuple<TrustType, int> tuple = new Tuple<TrustType, int>(trust.type, trust.tier);
		if (unplayedTrusts[tuple].Contains(trust))
		{
			unplayedTrusts[tuple].Remove(trust);
			playedTrusts[tuple].Add(trust);
		}
		else
		{
			Debug.LogError("Trust : " + trust.trustName + " not exists");
		}
	}

	private void ResetPlayedTrusts(Tuple<TrustType, int> tuple)
	{
		List<Trust> played = playedTrusts[tuple];
		List<Trust> unplayed = playedTrusts[tuple];

		unplayed.AddRange(played);
		played.RemoveRange(0, played.Count);
	}
}
