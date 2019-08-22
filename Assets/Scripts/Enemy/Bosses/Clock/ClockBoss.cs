using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockBoss : Boss
{
	public override EnemyType Type { get { return EnemyType.BOSS_CLOCK; } }

	public static float timeSpeed = 1.0f;

    public Transform handOrigin;
    public ClockHand hourHand, minuteHand, secondHand;
    public float patternTimer = 5f;

    private Animator anim;

    private int phase = 0;

    public AnimationCurve handCurve;

	[SerializeField]
	private GameObject traps;
	[SerializeField]
	private HitRange trapRange;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        hourHand.EnableHand();
    }

    protected override void Update()
    {
        base.Update();
        if (phase == 0 && Health < 700)
        {
            stateMachine.Transition("p1to2");
        }
        else if (phase == 1 && Health < 400)
        {
            stateMachine.Transition("p2to3");
        }
    }

    protected override void InitEnemy()
    {
        InGameUIManager.inst.UpdateBossHelthUI(1);

        State P1To2 = new State();
        State P2To3 = new State();

        State idle = new State();
        State pattern1 = new State();
        State pattern2 = new State();
        State pattern3 = new State();
        State pattern4 = new State();
        State pattern5 = new State();
        State pattern6 = new State();

		State dead = new State();

        idle.Enter += delegate
        {
            patternTimer = Random.Range(10f - phase, 15f - 2 * phase);
        };

        idle.StateUpdate += delegate
        {
            if (patternTimer > 0)
            {
                patternTimer -= Time.deltaTime;
                if (patternTimer <= 0)
                {
                    stateMachine.Transition("pattern" + Random.Range(1, 3).ToString());
                }
            }
        };

        pattern1.Enter += delegate
        {
            minuteHand.Shake();
            secondHand.Shake();
        };

        pattern2.Enter += delegate
        {
            ChangeTimeSpeed();
            stateMachine.Transition("idle");
        };

        P1To2.Enter += delegate {
            hourHand.DisableHand();
            minuteHand.MoveToOrigin(handOrigin);
            anim.SetTrigger("NextPhase");
            StartCoroutine(MovePhaseRoutine());
        };

        P1To2.Exit += delegate
        {
            hourHand.EnableHand();
			StartCoroutine(ActiveTrapRoutine());
        };

        P2To3.Enter += delegate {
            hourHand.DisableHand();
            minuteHand.DisableHand();
            secondHand.MoveToOrigin(handOrigin);
            anim.SetTrigger("NextPhase");
            StartCoroutine(MovePhaseRoutine());
        };

        P2To3.Exit += delegate
        {
            hourHand.EnableHand();
            minuteHand.EnableHand();
        };

		dead.Enter += delegate
		{
			hourHand.DisableHand();
			minuteHand.DisableHand();
			secondHand.DisableHand();
		};

        stateMachine.AddNewState("idle", idle);
        stateMachine.AddNewState("pattern1", pattern1);
        stateMachine.AddNewState("pattern2", pattern2);
        stateMachine.AddNewState("pattern3", pattern3);
        stateMachine.AddNewState("pattern4", pattern4);
        stateMachine.AddNewState("pattern5", pattern5);
        stateMachine.AddNewState("pattern6", pattern6);

        stateMachine.AddNewState("p1to2", P1To2);
        stateMachine.AddNewState("p2to3", P2To3);

		stateMachine.AddNewState("dead", dead);

        stateMachine.Transition("idle");
    }

	private void RotateHand(Transform target, Vector3 axis, float speed)
    {
        target.Rotate(axis, speed * timeSpeed * Time.deltaTime);
    }

    private void ShotBullet(Transform hand, Transform shotOrigin, GameObject prefab)
    {
        float angle = Mathf.Deg2Rad * (hand.rotation.eulerAngles.z + 90);
        GameObject bullet = Instantiate(prefab, shotOrigin.position, hand.rotation);
        bullet.GetComponent<Rigidbody2D>().velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;
    }

    private IEnumerator MovePhaseRoutine()
    {
        ++phase;
        yield return new WaitForSeconds(3);
        stateMachine.Transition("idle");
    }

    private IEnumerator ShakeHand(Transform hand)
    {
        Vector3 randPos = Random.insideUnitCircle * 2;
        Vector3 destination = PlayerController.inst.transform.position + randPos;
        Vector3 oriPos = hand.position;

        for (float t = 0; t < 2f; t += Time.deltaTime)
        {
            hand.position = Vector3.Lerp(oriPos, destination, 1 - (t / 2 - 1) * (t / 2 - 1));
            yield return null;
        }

        Quaternion oriRot = hand.rotation;
        for (float t = 0; t <= 1f; t += Time.deltaTime)
        {
            hand.rotation = Quaternion.Euler(0, 0, oriRot.eulerAngles.z + 360 * t);
            yield return null;
        }
        hand.rotation = oriRot;

        for (float t = 0; t < 2f; t += Time.deltaTime)
        {
            hand.position = Vector3.Lerp(destination, oriPos, 1 - (t / 2 - 1) * (t / 2 - 1));
            yield return null;
        }
    }

    private void ChangeTimeSpeed()
    {
        float rand = Random.Range(0f, 100f);
        if (rand < 25)
        {
            StartCoroutine(ChangeTimeSpeedRoutine(-1f));
        }
        else if (rand < 50)
        {
            StartCoroutine(ChangeTimeSpeedRoutine(0.5f));
        }
        else if (rand < 75)
        {
            StartCoroutine(ChangeTimeSpeedRoutine(1f));
        }
        else
        {
            StartCoroutine(ChangeTimeSpeedRoutine(2f));
        }
    }

    private IEnumerator ChangeTimeSpeedRoutine(float speed)
    {
        float oriSpeed = timeSpeed;
        for (float t = 0; t <= 1f; t += Time.deltaTime)
        {
            timeSpeed = Mathf.Lerp(oriSpeed, speed, t);
            yield return null;
        }
    }

	private IEnumerator ActiveTrapRoutine()
	{
		yield return trapRange.Activate(3);
		traps.SetActive(true);
	}

	public override void GetDamagedToDeath()
	{
		
	}

	protected override void OnDead()
	{
		base.OnDead();
		stateMachine.Transition("dead");
		GameManager.inst.GameClear();
	}
}
