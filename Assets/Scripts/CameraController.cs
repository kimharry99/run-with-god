using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	public static Action<float, float> Shake;
	public static Action<Vector2> ShockWave;
	public static Action ChromaticAberration;


	private Transform target;
	private const float offsetY = 0.5f, offsetZ = -9;

	public Material shockwave, chromatic;
	
	private void Start()
	{
		target = PlayerController.inst.transform;
		Shake = CameraShake;
		ShockWave = ShockwaveEffect;
		ChromaticAberration = ChromaticAberrationEffect;
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

	private void ChromaticAberrationEffect()
	{
		StartCoroutine(ChromaticAberrationEffectRoutine());
	}

	IEnumerator ChromaticAberrationEffectRoutine()
	{
		for (float t = 0; t < 1; t += Time.deltaTime)
		{
			chromatic.SetFloat("_Bias", (1 - 4 * (t - 0.5f) * (t - 0.5f)) / 100);
			yield return null;
		}
		chromatic.SetFloat("_Bias", 0);
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
		float oriRadius = shockwave.GetFloat("_Radius");

		float t = 0;
		float x = 0;
		float waveRadius = 0;
		while (x < 1)
		{
			x += Time.deltaTime;
			t = 1 - (x - 1) * (x - 1);
			shockwave.SetFloat("_Thickness", oriThickness * ((x - 1) * (x - 1)));
			waveRadius = Mathf.Lerp(-0.2f, oriRadius, t);
			shockwave.SetFloat("_Radius", waveRadius);
			yield return null;
		}
		shockwave.SetFloat("_Thickness", oriThickness);
		shockwave.SetFloat("_Radius", oriRadius);

		shockwave.SetFloat("_CenterX", 0.5f);
		shockwave.SetFloat("_CenterY", 0.5f);
	}

	
	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		RenderTexture tmp = RenderTexture.GetTemporary(source.width, source.height);
		Graphics.Blit(source, tmp, shockwave);
		Graphics.Blit(tmp, destination, chromatic);
		RenderTexture.ReleaseTemporary(tmp);
	}
	
}
