using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : NormalEnemy
{
	protected override void InitEnemy()
	{
		State idle = new State();
		State move = new State();

		idle.StateUpdate += MonitorAndTransition;
		move.StateUpdate += Flying;

		stateMachine.AddNewState("idle", idle);
		stateMachine.AddNewState("move", move);

		stateMachine.Transtion("idle");
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		PlayerController pc = collision.gameObject.GetComponent<PlayerController>();
		if (pc != null && pc.IsDamagable)
			pc?.GetDamaged();
	}
}
