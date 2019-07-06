﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Dingo : NormalEnemy
{
	public override EnemyType Type { get { return EnemyType.DINGO; } }

	protected override void InitEnemy()
    {
        State idle = new State();
        State move = new State();

        idle.StateUpdate += MonitorAndTransition;

        move.StateUpdate += FollowPlayer;

        stateMachine.AddNewState("idle", idle);
        stateMachine.AddNewState("move", move);

        stateMachine.Transition("idle");
    }
 
}
