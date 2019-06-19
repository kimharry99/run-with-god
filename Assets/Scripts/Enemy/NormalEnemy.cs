using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NormalEnemy : MonoBehaviour
{
	private int _health;
	public int Health
	{
		get
		{
			return _health;
		}
		private set
		{
			_health = value;
			if (_health <= 0)
			{
				OnDead();
			}
		}
	}
	protected float speed;
	protected float eyesight;
	protected float power;

	public ParticleSystem hitEffect;
	protected StateMachine stateMachine = new StateMachine();

	[SerializeField]
	private Shader dissolve;

    protected virtual void Start()
    {
		InitMonster();
    }

    protected virtual void Update()
    {
		stateMachine.UpdateStateMachine();
    }

	#region Monster Basic Functions
	protected abstract void InitMonster();

	/// <summary>
	/// Called when hit by bullet
	/// </summary>
	public virtual void GetDamaged()
	{
		Health--;
	}

	public void GetDamagedToDeath()
	{
		Health = 0;
	}

	/// <summary>
	/// Called when health is lower or equal 0
	/// </summary>
	protected virtual void OnDead()
	{
		StartCoroutine(DissolveEffectRoutine(2));
		//Play dead anim
		//Destroy
	}

	protected IEnumerator DissolveEffectRoutine(float time)
	{
		Material mat = new Material(dissolve);
		GetComponent<SpriteRenderer>().material = mat;
		Texture2D noise = new Texture2D(100, 100);

		float scale = Random.Range(1, 10);
		for (int i = 0; i < noise.width; ++i)
		{
			for (int j = 0; j < noise.height; ++j)
			{
				float noiseVal = Mathf.PerlinNoise(scale * i / noise.width, scale * j / noise.height);
				noise.SetPixel(i, j, new Color(noiseVal, noiseVal, noiseVal, 1));
			}
		}
		noise.Apply();
		mat.SetTexture("_NoiseTex", noise);

		for (float t = 0; t < time; t += Time.deltaTime)
		{
			print(t / time);
			mat.SetFloat("_Threshold", t / time);
			yield return null;
		}
		mat.SetFloat("_Threshold", 1);
	}

	#endregion

	#region General Monster AI Functions

	protected void Chase()
	{
		//Follow the player
	}

	protected void Moving()
	{
		//Wander around
	}

	protected void Idle()
	{
		//Stop
	}

	protected void AttackMelee()
	{
		//Short range attack
	}

	protected void AttackProjectile()
	{
		//Long range attack
	}
	#endregion
}