using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class CameraController : MonoBehaviour
{
	public static Action<float, float> Shake;
	public static Action<Vector2> ShockWave;
	public static Action ChromaticAberration;
    public static Action HitEffect;
    public bool isFollowingPlayer;

	private Transform target;
    private Rigidbody2D targetRb;
	private float offsetX = 0.5f, offsetY = 0f, offsetZ = -9;
    private float timeCount;

	public Material shockwave, chromatic;

    private Coroutine shockwaveRoutine, chromaticRoutine;

    [SerializeField]
    private ParticleSystem hitEffect;

 	private void Start()
	{
		target = PlayerController.inst.transform;
        targetRb = target.GetComponent<Rigidbody2D>();
		Shake = CameraShake;
		ShockWave = ShockwaveEffect;
		ChromaticAberration = ChromaticAberrationEffect;
        HitEffect = PlayHitEffect;
        timeCount = 0;
        isFollowingPlayer = true;
	}

	private void LateUpdate()
	{
        //offsetY = Input.GetAxis("Vertical") * 0.5f;
        offsetX = Input.GetAxis("HorizontalCamera") * 0.5f;
        timeCount += Time.deltaTime;
        //isFollowingPlayer = offsetX * offsetX >= 1;
        if (isFollowingPlayer)
        {
            transform.position = Vector3.Lerp(transform.position, target.position + new Vector3(offsetX, offsetY, offsetZ), 0.1f);
        }
        /*
        else
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x,target.position.y,target.position.z) + new Vector3(offsetX, offsetY, offsetZ), 0.1f);
        }
        */
	}

    public void SetCameraOffset(float x, float y)
    {
        offsetX = x;
        offsetY = y;
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
        if (chromaticRoutine != null)
            StopCoroutine(chromaticRoutine);
        chromaticRoutine = StartCoroutine(ChromaticAberrationEffectRoutine());
	}

	IEnumerator ChromaticAberrationEffectRoutine()
	{
		for (float t = 0; t < 1; t += Time.deltaTime)
		{
			chromatic.SetFloat("_Bias", (1 - 4 * (t - 0.5f) * (t - 0.5f)) / 200);
			yield return null;
		}
		chromatic.SetFloat("_Bias", 0);
	}

	private void ShockwaveEffect(Vector2 center)
	{
        if (shockwaveRoutine != null)
            StopCoroutine(shockwaveRoutine);
		shockwaveRoutine = StartCoroutine(ShockWaveEffectRoutine(center));
	}

	IEnumerator ShockWaveEffectRoutine(Vector2 center)
	{
		shockwave.SetFloat("_CenterX", center.x);
		shockwave.SetFloat("_CenterY", center.y);

        //float oriThickness = shockwave.GetFloat("_Thickness");
        //float oriRadius = shockwave.GetFloat("_Radius");
        float oriThickness = 0.2f;
        float oriRadius = 1f;

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
		shockwave.SetFloat("_Thickness", 0.2f);
		shockwave.SetFloat("_Radius", 1f);

		shockwave.SetFloat("_CenterX", 0.5f);
		shockwave.SetFloat("_CenterY", 0.5f);
	}

	
	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		RenderTexture tmp = RenderTexture.GetTemporary(source.width, source.height);
		Graphics.Blit(source, tmp, chromatic);
		Graphics.Blit(tmp, destination, shockwave);
		RenderTexture.ReleaseTemporary(tmp);
	}
	
    private void PlayHitEffect()
    {
        hitEffect.Play();
    }
}
