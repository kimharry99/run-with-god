using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DummyMonster : NormalEnemy
{
	protected override void InitMonster()
	{
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
	}
}
