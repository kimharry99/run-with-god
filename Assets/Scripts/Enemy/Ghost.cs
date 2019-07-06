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
}
