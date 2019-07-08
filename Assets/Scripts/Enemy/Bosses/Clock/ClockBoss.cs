using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockBoss : NormalEnemy
{
	public override EnemyType Type { get { return EnemyType.BOSS_CLOCK; } }

	public static float timeSpeed = 1.0f;

    public Transform handOrigin;
    public Transform hourHand, minuteHand, secondHand;
    public Transform hourShot, minuteShot, secondShot;
    public GameObject hourBulletPrefab, minBulletPrefab, secBulletPrefab;
    public float hourShotTimer, minShotTimer, secShotTimer;
    public float shakeTimer = 5f;
    public float changeTimeSpeedTimer = 10f;

    private Coroutine minuteShake, secondShake;

	protected override void Update()
    {
        base.Update();
        if (hourShotTimer > 0)
            hourShotTimer -= Time.deltaTime;
        if (minShotTimer > 0)
            minShotTimer -= Time.deltaTime;
        if (secShotTimer > 0)
            secShotTimer -= Time.deltaTime;
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
            RotateHand(hourHand, Vector3.back, 40);
            if (hourShotTimer <= 0)
            {
                ShotBullet(hourHand, hourShot, hourBulletPrefab);
                hourShotTimer = 0.2f;
            }
            if (shakeTimer <= 0)
            {
                secondShake = StartCoroutine(ShakeHand(secondHand));
                minuteShake = StartCoroutine(ShakeHand(minuteHand));
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

        phase1.Exit += delegate { if (minuteShake != null) StopCoroutine(minuteShake); };

        P1To2.Enter += delegate { StartCoroutine(MovePhaseRoutine(minuteHand, handOrigin, "phase2")); };

        phase2.StateUpdate += delegate
        {
            RotateHand(hourHand, Vector3.back, 40);
            RotateHand(minuteHand, Vector3.back, 60);
            if (hourShotTimer <= 0)
            {
                ShotBullet(hourHand, hourShot, hourBulletPrefab);
                hourShotTimer = 0.2f;
            }
            if (minShotTimer <= 0)
            {
                ShotBullet(minuteHand, minuteShot, minBulletPrefab);
                minShotTimer = 0.2f;
            }
            if (shakeTimer <= 0)
            {
                StartCoroutine(ShakeHand(secondHand));
                StartCoroutine(ShakeHand(minuteHand));
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

        phase2.Exit += delegate { if (secondShake != null) StopCoroutine(secondShake); };

        P2To3.Enter += delegate { StartCoroutine(MovePhaseRoutine(secondHand, handOrigin, "phase3")); };

        phase3.Enter += delegate
        {
            hourShotTimer = 2f;
            minShotTimer = 1.5f;
            secShotTimer = 1f;
        };

        phase3.StateUpdate += delegate
        {
            RotateHand(hourHand, Vector3.back, 40);
            RotateHand(minuteHand, Vector3.back, 60);
            RotateHand(secondHand, Vector3.back, 120);

            if (hourShotTimer <= 0)
            {
                ShotBullet(hourHand, hourShot, hourBulletPrefab);
                hourShotTimer = 0.2f;
            }
            if (minShotTimer <= 0)
            {
                ShotBullet(minuteHand, minuteShot, minBulletPrefab);
                minShotTimer = 0.2f;
            }
            if (secShotTimer <= 0)
            {
                ShotBullet(secondHand, secondShot, secBulletPrefab);
                secShotTimer = 0.2f;
            }
            if (changeTimeSpeedTimer <= 0)
            {
                ChangeTimeSpeed();
            }
            if (shakeTimer <= 0)
            {
                StartCoroutine(ShakeHand(secondHand));
                StartCoroutine(ShakeHand(minuteHand));
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

    private IEnumerator MovePhaseRoutine(Transform hand, Transform to, string nextState)
    {
        Vector3 oriPos = hand.position;
        Quaternion oriRot = hand.rotation;
        for (float t = 0; t <= 2.5f; t += Time.deltaTime)
        {
            hand.position = Vector3.Lerp(oriPos, to.position, t / 2.5f);
            hand.rotation = Quaternion.Lerp(oriRot, to.rotation, t / 2.5f);
            yield return null;
        }
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
