using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Zombie : NormalEnemy
{
	public override EnemyType Type { get { return EnemyType.ZOMBIE; } }

	protected override void InitEnemy()
	{
        State idle = new State();
        State move = new State();

        idle.StateUpdate += MonitorAndTransition;
        move.StateUpdate += Moving;

        stateMachine.AddNewState("idle", idle);
		stateMachine.AddNewState("move", move);

		stateMachine.Transition("idle");
	}

    protected override void MonitorAndTransition(string nextState = "move")
    {
        SeePlayer();
        if (DetectPlayer(size))
        {
            stateMachine.Transition(nextState);
        }
    }
}
