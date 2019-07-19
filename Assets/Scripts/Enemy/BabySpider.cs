using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BabySpider : NormalEnemy
{
    public override EnemyType Type => throw new System.NotImplementedException();

    protected override void InitEnemy()
    {
        State move = new State();

        move.StateUpdate += FollowPlayer;

        stateMachine.AddNewState("move", move);

        stateMachine.Transition("move");
    }

    protected void SelfDestruct()
    {
        AttackMelee(2.5f);
        GetDamagedToDeath();
    }
}
