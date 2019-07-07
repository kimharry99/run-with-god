using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : NormalEnemy
{
	public override EnemyType Type { get { return EnemyType.GHOST; } }

	protected override void InitEnemy()
	{
		State idle = new State();
		State move = new State();

		idle.StateUpdate += MonitorAndTransition;
		move.StateUpdate += Flying;

		stateMachine.AddNewState("idle", idle);
		stateMachine.AddNewState("move", move);

		stateMachine.Transition("idle");
	}
}
