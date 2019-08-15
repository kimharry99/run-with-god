using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockBoss : NormalEnemy
{
	public override EnemyType Type { get { return EnemyType.BOSS_CLOCK; } }

	public static float timeSpeed = 1.0f;

    public Transform handOrigin;
    public ClockHand hourHand, minuteHand, secondHand;
    public float shakeTimer = 5f;
    public float changeTimeSpeedTimer = 10f;

    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        hourHand.EnableHand();
    }

    protected override void Update()
    {
        base.Update();
        if (shakeTimer > 0)
            shakeTimer -= Time.deltaTime;
        if (changeTimeSpeedTimer > 0)
            changeTimeSpeedTimer -= Time.deltaTime;
    }

    protected override void InitEnemy()
    {
        InGameUIManager.inst.UpdateBossHelthUI(1);

        State phase1 = new State();
        State phase2 = new State();
        State phase3 = new State();

        State P1To2 = new State();
        State P2To3 = new State();


        phase1.StateUpdate += delegate
        {

            if (shakeTimer <= 0)
            {
                minuteHand.Shake();
                secondHand.Shake();
                shakeTimer = Random.Range(10, 15);
            }
            if (changeTimeSpeedTimer <= 0)
            {
                ChangeTimeSpeed();
            }
            if (Health < 700)
            {
                stateMachine.Transition("p1to2");
            }
        };

        P1To2.Enter += delegate {
            hourHand.DisableHand();
            minuteHand.MoveToOrigin(handOrigin);
            anim.SetTrigger("NextPhase");
            StartCoroutine(MovePhaseRoutine("phase2"));
        };

        P1To2.Exit += delegate
        {
            hourHand.EnableHand();
        };

        phase2.StateUpdate += delegate
        {
            if (shakeTimer <= 0)
            {
                minuteHand.Shake();
                secondHand.Shake();
                shakeTimer = Random.Range(10, 15);
            }
            if (changeTimeSpeedTimer <= 0)
            {
                ChangeTimeSpeed();
            }
            if (Health < 400)
            {
                stateMachine.Transition("p2to3");
            }
        };

        P2To3.Enter += delegate {
            hourHand.DisableHand();
            minuteHand.DisableHand();
            secondHand.MoveToOrigin(handOrigin);
            anim.SetTrigger("NextPhase");
            StartCoroutine(MovePhaseRoutine("phase3"));
        };

        P2To3.Exit += delegate
        {
            hourHand.EnableHand();
            minuteHand.EnableHand();
        };

        phase3.Enter += delegate
        {

        };

        phase3.StateUpdate += delegate
        {
            if (changeTimeSpeedTimer <= 0)
            {
                ChangeTimeSpeed();
            }
            if (shakeTimer <= 0)
            {
                minuteHand.Shake();
                secondHand.Shake();
                shakeTimer = Random.Range(10, 15);
            }
        };

        stateMachine.AddNewState("phase1", phase1);
        stateMachine.AddNewState("phase2", phase2);
        stateMachine.AddNewState("phase3", phase3);

        stateMachine.AddNewState("p1to2", P1To2);
        stateMachine.AddNewState("p2to3", P2To3);

        stateMachine.Transition("phase1");
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

    private IEnumerator MovePhaseRoutine(string nextState)
    {
        yield return new WaitForSeconds(3);
        stateMachine.Transition(nextState);
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
        changeTimeSpeedTimer = 5f;
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

	public override void GetDamaged(int damage)
	{
		base.GetDamaged(damage);
		InGameUIManager.inst.UpdateBossHelthUI((float)Health / maxHealth);
	}

	public override void GetDamagedToDeath()
	{
		
	}
}
