using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NormalMonster : MonoBehaviour
{
	private int _health;
	public int Health
	{
		get
		{
			return _health;
		}
		set
		{
			if (Health <= 0)
			{
				OnDead();
			}
		}
	}
	
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
	protected virtual void GetDamaged(int damage)
	{

	}
	protected virtual void OnDead()
	{

	}

	#endregion

	#region Monster AI Functions

	protected void FollowPlayer()
	{
		//Follow the player
	}

	protected void Moving()
	{
		//Wander
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