using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NormalMonster : MonoBehaviour
{
	public int Health { get; private set; }
	
	protected StateMachine stateMachine;

    protected virtual void Start()
    {
		stateMachine = new StateMachine();
		InitMonster();
    }

    protected virtual void Update()
    {
		stateMachine.UpdateStateMachine();
    }

	protected abstract void InitMonster();

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
}