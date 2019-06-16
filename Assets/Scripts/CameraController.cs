using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	public static Action<float, float> Shake;

	private Transform target;
	private const float offsetY = 0.5f, offsetZ = -9;
	private void Start()
	{
		target = PlayerController.inst.transform;
		Shake = CameraShake;
	}

	private void Update()
	{
		transform.position = Vector3.Lerp(transform.position, target.position + new Vector3(0, offsetY, offsetZ), 3);
	}

	private void CameraShake(float amount, float time)
	{
		StartCoroutine(CameraShaking(amount, time));
	}

	private IEnumerator CameraShaking(float amount, float time)
	{
		float oriAmount = amount;
		float oriTime = time;
		for (float t = 0; t < time; t += Time.deltaTime)
		{
			Vector2 randVec = UnityEngine.Random.insideUnitCircle;
			amount = oriAmount * (time / oriTime);
			transform.position += new Vector3(randVec.x, randVec.y) * amount;
			yield return null;
		}
	}
}
