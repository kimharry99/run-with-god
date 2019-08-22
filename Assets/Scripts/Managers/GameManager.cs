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
    public GameObject tutorialMapBlockPrefab;
    public GameObject firstMapBlockPrefab;
    public GameObject lastMapBlockPrefab;
	public List<GameObject> mapBlockPrefabs = new List<GameObject>();

	[SerializeField]
	private List<GameObject> bossMapBlockPrefabs = new List<GameObject>();

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
        foreach (var block in Resources.LoadAll<GameObject>("MapPrefabs"))
        {
            if (block.GetComponent<MapBlock>() == null)
                continue;
            mapBlockPrefabs.Add(block);
        }
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

    /*
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
			GUI.Label(new Rect(10, 90 + 20 * i++, 500, 20), ts.type + " : " + (ts.Trust != null ? ts.Trust.GetName() : "Null"));
		}
		
	}
#endif
*/
	private void Update()
	{
		gameState.UpdateStateMachine();
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if(scene.name == "TrustSelection")
		{
			ResetGame();
			PlayerController.inst.transform.position = FindObjectOfType<MapBlock>().startPoint.position;
			mapSeed = UnityEngine.Random.Range(0, int.MaxValue);
		}
		if (scene.name == "InGameScene")
		{
			if (SelectedTrust != null)
			{
				SelectedTrust.Init();
				InGameUIManager.inst.UpdateTrustUI(SelectedTrust);
			}
            Playtime=0;
			GenerateMap();
			PlayerController.inst.ResetPlayer();
		}
		if (scene.name == "Boss")
		{
			GenerateBossMap();
            //티어별 패턴 선택
            //플레이어 위치 초기화 
		}
        else if (scene.name == "Boss_Butcher")
        {

        }
        else if (scene.name == "Boss_Vampire")
        {

        }
		if (scene.name == "InGameScene" || scene.name == "Boss" || scene.name == "Boss_Butcher" || scene.name == "Boss_Vampire")
		{
			gameState.Transition("play");
		}
		else
		{
			gameState.Transition("pause");
		}
		if (scene.name != "Title")
			Camera.main.transform.position = PlayerController.inst.transform.position;
        /*
#if UNITY_EDITOR
		if (scene.name == "TrustSelection")
		{
			selectors = FindObjectsOfType<TrustSelector>();
		}
#endif
*/
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
		SceneManager.LoadScene("InGameScene");
		yield return null;
	}
    
    //for Development, tier -> SelectedTruth.tier
    public int tier;
    [SerializeField]
    private bool isFirstPlay;
    private void GenerateMap()
    {
        UnityEngine.Random.InitState(mapSeed);

        MapBlock prevBlock;
        if (isFirstPlay)
        {
            prevBlock = Instantiate(tutorialMapBlockPrefab).GetComponent<MapBlock>();
            isFirstPlay = false;
        }
        else
        {
            prevBlock = Instantiate(firstMapBlockPrefab).GetComponent<MapBlock>();
        }
        PlayerController.inst.transform.position = prevBlock.startPoint.position;

        int[/*tier*/][/*difficulty*/] difficultyCount = { new int[] { 7, 2, 0, 0 }, new int[] { 5, 3, 1, 0 }, new int[] { 0, 5, 3, 1 }, new int[] { 0, 3, 4, 2 } };
        for (int i = 0; i < 9; ++i)
        {
            int rand = 0;
            for (bool is1 = true; is1;)
            {
                rand = UnityEngine.Random.Range(0, mapBlockPrefabs.Count);
                if (difficultyCount[tier][mapBlockPrefabs[rand].GetComponent<MapBlock>().difficulty] > 0)
                {
                    difficultyCount[tier][mapBlockPrefabs[rand].GetComponent<MapBlock>().difficulty]--;
                    is1 = false;
                }
            }

            /* delete map when first map is difficult */
            if (i == 0)
            {
                switch (tier)
                {
                    case 0:
                        if (mapBlockPrefabs[rand].GetComponent<MapBlock>().difficulty > 0)
                        {
                            difficultyCount[tier][mapBlockPrefabs[rand].GetComponent<MapBlock>().difficulty]++;
                            i--;
                            continue;
                        }
                        break;
                    case 1:
                    case -1:
                        if (mapBlockPrefabs[rand].GetComponent<MapBlock>().difficulty > 0)
                        {
                            difficultyCount[tier][mapBlockPrefabs[rand].GetComponent<MapBlock>().difficulty]++;
                            i--;
                            continue;
                        }
                        break;
                    case 2:
                    case -2:
                        if (mapBlockPrefabs[rand].GetComponent<MapBlock>().difficulty > 1)
                        {
                            difficultyCount[tier][mapBlockPrefabs[rand].GetComponent<MapBlock>().difficulty]++;
                            i--;
                            continue;
                        }
                        break;
                    case 3:
                    case -3:
                        if (mapBlockPrefabs[rand].GetComponent<MapBlock>().difficulty > 1)
                        {
                            difficultyCount[tier][mapBlockPrefabs[rand].GetComponent<MapBlock>().difficulty]++;
                            i--;
                            continue;
                        }
                        break;
                    default:
                        if (mapBlockPrefabs[rand].GetComponent<MapBlock>().difficulty > 1)
                        {
                            difficultyCount[tier][mapBlockPrefabs[rand].GetComponent<MapBlock>().difficulty]++;
                            i--;
                            continue;
                        }
                        break;

                }
            }

            MapBlock curBlock = Instantiate(mapBlockPrefabs[rand]).GetComponent<MapBlock>();
            curBlock.ConnectNextTo(prevBlock);
            prevBlock = curBlock;
        }
        MapBlock lastBlock = Instantiate(lastMapBlockPrefab).GetComponent<MapBlock>();
        lastBlock.ConnectNextTo(prevBlock);
    }

	private void GenerateBossMap()
	{
		MapBlock mapBlock;
		if (SelectedTrust != null)
		{
			mapBlock = Instantiate(bossMapBlockPrefabs[(int)SelectedTrust.trustType]).GetComponent<MapBlock>();
			mapBlock.difficulty = SelectedTrust.tier;
		}
		else
		{
			mapBlock = Instantiate(bossMapBlockPrefabs[0]).GetComponent<MapBlock>();
		}
		PlayerController.inst.transform.position = mapBlock.startPoint.position;
	}

	public void GameClear()
	{
		GameObject player = PlayerController.inst.gameObject;
		player.GetComponent<Rigidbody2D>().simulated = false;
		do
		{
			if (SelectedTrust == null)
				break;
			if (SelectedTrust.IsDone)
				trustTier[SelectedTrust.trustType]++;
			else
				trustTier[SelectedTrust.trustType]--;
		}
		while (trustTier[SelectedTrust.trustType] == 0);

		StartCoroutine(GameClearRoutine());
	}
	private IEnumerator GameClearRoutine()
	{
		yield return InGameUIManager.inst.FadeIn(5);
		SceneManager.LoadScene("TrustSelection");
	}

	private void ResetGame()
	{
        InGameUIManager.inst.BlackPanel.color = new Color(0, 0, 0, 0);
		TrustSelector.SelectedTrust = null;
		PlayerController.inst.ResetPlayer();
		enemyKillCounts = new Dictionary<EnemyType, int>();
		_playtime = 0;
	}
}
