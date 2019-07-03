﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : NormalEnemy
{
    protected override void InitEnemy()
    {
        State idle = new State();
        State move = new State();

        idle.StateUpdate += Idle;
        //idle.StateUpdate += AttackTouch;

        move.StateUpdate += FollowPlayer;

        stateMachine.AddNewState("idle", idle);
        stateMachine.AddNewState("move", move);

        stateMachine.Transtion("idle");
    }
}