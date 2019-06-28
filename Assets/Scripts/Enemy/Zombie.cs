using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Zombie : NormalEnemy
{
    protected override void InitEnemy()
	{
        State idle = new State();
        State move = new State();

        idle.StateUpdate += Idle;
        //idle.StateUpdate += AttackTouch;

		move.StateUpdate += Moving;
        move.StateUpdate += AttackTouch;

        stateMachine.AddNewState("idle", idle);
		stateMachine.AddNewState("move", move);

		stateMachine.Transtion("idle");
	}
}
