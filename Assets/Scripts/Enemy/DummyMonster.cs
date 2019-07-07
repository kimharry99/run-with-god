using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DummyMonster : NormalEnemy
{
	public override EnemyType Type { get { return EnemyType.ZOMBIE; } }

	public ParticleSystem dead;

	protected override void InitEnemy()
	{
		/*
		State idle = new State();
		idle.StateUpdate += Idle;

		State move = new State();
		idle.StateUpdate += Moving;

		State attack = new State();
		attack.StateUpdate += AttackMelee;

		stateMachine.AddNewState("idle", idle);
		stateMachine.AddNewState("move", move);
		stateMachine.AddNewState("attack", attack);

		stateMachine.Transtion("idle");
		*/
	}

	protected override void OnDead()
	{
		base.OnDead();
		dead.Play();
		StartCoroutine(DissolveEffectRoutine(1));
	}
}
