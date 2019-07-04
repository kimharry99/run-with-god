using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockBoss : NormalEnemy
{
	public static float projSpeed = 1.0f;

	public Transform handOrigin;
	public Transform hourHand, minuteHand, secondHand;
	public Transform hourShot, minuteShot, secondShot;
	public GameObject hourBulletPrefab, minBulletPrefab, secBulletPrefab;
	public float hourShotTimer, minShotTimer, secShotTimer;

	protected override void Update()
	{
		base.Update();
		if (hourShotTimer > 0)
			hourShotTimer -= Time.deltaTime;
		if (minShotTimer > 0)
			minShotTimer -= Time.deltaTime;
		if (secShotTimer > 0)
			secShotTimer -= Time.deltaTime;
	}

	protected override void InitEnemy()
	{
		State phase1 = new State();
		State phase2 = new State();
		State phase3 = new State();

		State P1To2 = new State();
		State P2To3 = new State();


		phase1.StateUpdate += delegate
		{
			if (Health < 999)
			{
				stateMachine.Transtion("p1to2");
			}
		};

		P1To2.Enter += delegate { StartCoroutine(MovePhaseRoutine(minuteHand, handOrigin)); };

		phase3.Enter += delegate
		{
			hourShotTimer = 2f;
			minShotTimer = 1.5f;
			secShotTimer = 1f;
		};

		phase3.StateUpdate += delegate
		{
			RotateHand(hourHand, handOrigin, 20);
			RotateHand(minuteHand, handOrigin, 30);
			RotateHand(secondHand, handOrigin, 60);

			hourShotTimer -= Time.deltaTime;
			minShotTimer -= Time.deltaTime;
			secShotTimer -= Time.deltaTime;

			if (hourShotTimer < 0)
			{
				ShotBullet(hourHand, hourShot, hourBulletPrefab);
				hourShotTimer = 1f;
			}
			if (minShotTimer < 0)
			{
				ShotBullet(minuteHand, minuteShot, minBulletPrefab);
				minShotTimer = 1f;
			}
			if (secShotTimer < 0)
			{
				ShotBullet(secondHand, secondShot, secBulletPrefab);
				secShotTimer = 1f;
			}
		};

		stateMachine.AddNewState("phase1", phase1);
		stateMachine.AddNewState("phase2", phase2);
		stateMachine.AddNewState("phase3", phase3);

		stateMachine.AddNewState("p1to2", P1To2);
		stateMachine.AddNewState("p2to3", P2To3);

		stateMachine.Transtion("phase1");
	}

	private void RotateHand(Transform target, Transform pivot, float speed)
	{
		target.Rotate(pivot.position, speed * Time.deltaTime);
	}

	private void ShotBullet(Transform hand, Transform shotOrigin, GameObject prefab)
	{
		float angle = Mathf.Deg2Rad * (hand.rotation.eulerAngles.z + 90);
		GameObject bullet = Instantiate(prefab, shotOrigin.position, hand.rotation);
		Debug.Log(angle + " : (" + Mathf.Cos(angle) + ", " + Mathf.Sin(angle) + ")");
		Debug.Log(angle + " : (" + Mathf.Cos(angle) + ", " + Mathf.Sin(angle) + ")");
		bullet.GetComponent<Rigidbody2D>().velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized * projSpeed;
	}

	private IEnumerator MovePhaseRoutine(Transform hand, Transform to)
	{
		Vector3 oriPos = hand.position;
		Quaternion oriRot = hand.rotation;
		for (float t = 0; t <= 2.5f; t+= Time.deltaTime)
		{
			hand.position = Vector3.Lerp(oriPos, to.position, t / 2.5f);
			hand.rotation = Quaternion.Lerp(oriRot, to.rotation, t / 2.5f);
			yield return null;
		}
		stateMachine.Transtion("phase2");
	}
}
