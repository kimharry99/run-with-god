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
    public Action EnemyInCamera;
    public Action EnemyOutCamera;

	//private int _killCount;
	public int KillCount
	{
		get
		{
			int _killCount = 0;
			foreach (var killCount in enemyKillCounts.Values)
			{
				_killCount += killCount;
			}
			return _killCount;
		}
	}

	public Dictionary<EnemyType, int> enemyKillCounts = new Dictionary<EnemyType, int>();
	public Action<EnemyType> OnEnemyKilled;

	private StateMachine gameState = new StateMachine();
	public Trust SelectedTrust { get { return TrustSelector.SelectedTrust; } }

	private Dictionary<TrustType, int> trustTier = new Dictionary<TrustType, int>();
	private Dictionary<Tuple<TrustType, int>, List<Trust>> unplayedTrusts = new Dictionary<Tuple<TrustType, int>, List<Trust>>();
	private Dictionary<Tuple<TrustType, int>, List<Trust>> playedTrusts = new Dictionary<Tuple<TrustType, int>, List<Trust>>();

	public int mapSeed;
    public GameObject firstMapBlockPrefab;
	public List<GameObject> mapBlockPrefabs = new List<GameObject>();

	private void OnEnable()
	{
		//Reset GameManager
	}

	private void Awake()
	{
		if (inst != this)
		{
			Destroy(gameObject);
			return;
		}
		else
			SetStatic();

		InitGameState();

		trustTier.Add(TrustType.ATTACK, 0);
		trustTier.Add(TrustType.MOVE, 0);
		trustTier.Add(TrustType.ACTION, 0);

		foreach (var trust in Resources.LoadAll<Trust>("Trusts"))
		{
			//Debug.Log(trust.name + " loaded");
			Tuple<TrustType, int> tuple = new Tuple<TrustType, int>(trust.trustType, trust.tier);
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
	}

#if UNITY_EDITOR

	private TrustSelector[] selectors;
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
			foreach(var ts in selectors)
			{
				ts.InitTrustSelector(PickTrust(ts.type));
			}
		}
		int i = 0;
		foreach (var ts in selectors)
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
		if (scene.name == "InGameScene")
		{
			if (SelectedTrust != null)
			{
				SelectedTrust.Init();
				InGameUIManager.inst.UpdateTrustUI(SelectedTrust);
			}
			GenerateMap();
			PlayerController.inst.ResetPlayer();
		}
		if (scene.name == "Boss")
		{

		}
		if (scene.name == "InGameScene" || scene.name == "Boss")
		{
			gameState.Transition("play");
		}
		else
		{
			gameState.Transition("pause");
		}
#if UNITY_EDITOR
		if (scene.name == "TrustSelection")
		{
			selectors = FindObjectsOfType<TrustSelector>();
		}
#endif
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
		Tuple<TrustType, int> tuple = new Tuple<TrustType, int>(trust.trustType, trust.tier);
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

	public void OnEnemyKill(EnemyType type)
	{
		if (!enemyKillCounts.ContainsKey(type))
			enemyKillCounts.Add(type, 0);
		enemyKillCounts[type]++;
		OnEnemyKilled?.Invoke(type);
	}

	public void GameOver()
	{
		StartCoroutine(GameOverRoutine());
	}

	private IEnumerator GameOverRoutine()
	{
		Scene scene = SceneManager.GetActiveScene();
		if (scene.name == "InGameScene")
		{
			SceneManager.LoadScene(scene.name);
		}
		else if (scene.name == "Boss")
		{

		}
		yield return null;
	}

	private void GenerateMap()
	{
		UnityEngine.Random.InitState(mapSeed);

		MapBlock prevBlock = Instantiate(firstMapBlockPrefab).GetComponent<MapBlock>();
		PlayerController.inst.transform.position = prevBlock.startPoint.position;

		for (int i = 0; i < 10; ++i)
		{
			int rand = UnityEngine.Random.Range(0,mapBlockPrefabs.Count);
			MapBlock curBlock = Instantiate(mapBlockPrefabs[rand]).GetComponent<MapBlock>();
			curBlock.ConnectNextTo(prevBlock);
			prevBlock = curBlock;
		}
	}
}
