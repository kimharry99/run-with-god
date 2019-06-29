﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DummyMonster : NormalEnemy
{
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
		GameManager.inst.KillCount++;
		dead.Play();
		StartCoroutine(DissolveEffectRoutine(1));
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		PlayerController pc = collision.gameObject.GetComponent<PlayerController>();
		if (pc != null && pc.IsDamagable)
			pc?.GetDamaged();
	}
}
