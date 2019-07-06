using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Undead : NormalEnemy
{

    protected override void InitEnemy()
    {
        State idle = new State();
        State move = new State();

        idle.StateUpdate += MonitorAndTransition;
        idle.StateUpdate += CheckDurability;

        move.StateUpdate += FollowPlayer;
        move.StateUpdate += CheckDurability;

        stateMachine.AddNewState("idle", idle);
        stateMachine.AddNewState("move", move);

        stateMachine.Transtion("idle");
    }

    protected void CheckDurability()
    {
        if(Health < 2 * maxHealth / 3)
        {
            //상체 파괴
        }
        if (Health < maxHealth / 3)
        {
            speed = 3;//하체 파괴
        }
    }
}
