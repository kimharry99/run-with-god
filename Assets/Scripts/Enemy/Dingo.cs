using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Dingo : NormalMonster
{
    protected override void InitMonster()
    {
        State idle = new State();
        State move = new State();

        idle.StateUpdate += Idle;

        move.StateUpdate += FollowPlayer;

        stateMachine.AddNewState("idle", idle);
        stateMachine.AddNewState("move", move);

        stateMachine.Transtion("idle");
    }
}
