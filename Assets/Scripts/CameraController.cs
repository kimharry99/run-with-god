using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	public static Action<float, float> Shake;
	public static Action<Vector2> ShockWave;

	private Transform target;
	private const float offsetY = 0.5f, offsetZ = -9;

	public Material shockwave;
	
	private void Start()
	{
		target = PlayerController.inst.transform;
		Shake = CameraShake;
		ShockWave = ShockwaveEffect;
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
			amount = oriAmount * ((time - t) / oriTime);
			transform.position += new Vector3(randVec.x, randVec.y) * amount;
			yield return null;
		}
	}

	private void ShockwaveEffect(Vector2 center)
	{
		StartCoroutine(ShockWaveEffectRoutine(center));
	}

	IEnumerator ShockWaveEffectRoutine(Vector2 center)
	{
		shockwave.SetFloat("_CenterX", center.x);
		shockwave.SetFloat("_CenterY", center.y);

		float oriThickness = shockwave.GetFloat("_Thickness");

		float t = 0;
		float x = 0;
		float waveRadius = 0;
		while (x < 1)
		{
			x += Time.deltaTime;
			t = 1 - (x - 1) * (x - 1);
			shockwave.SetFloat("_Thickness", oriThickness * ((x - 1) * (x - 1)));
			waveRadius = Mathf.Lerp(-0.2f, 2, t);
			shockwave.SetFloat("_Radius", waveRadius);
			yield return null;
		}
		shockwave.SetFloat("_Thickness", oriThickness);
	}

	
	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		Graphics.Blit(source, destination, shockwave);
	}
	
}
