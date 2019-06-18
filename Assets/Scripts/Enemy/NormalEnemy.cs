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
	protected virtual void GetDamaged()
	{
		Health--;
	}

	/// <summary>
	/// Called when health is lower or equal 0
	/// </summary>
	protected virtual void OnDead()
	{
		//Play dead anim
		//Destroy
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