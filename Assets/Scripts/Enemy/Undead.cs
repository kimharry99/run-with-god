using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Undead : NormalEnemy
{
    public override EnemyType Type { get { return EnemyType.UNDEAD; } }
    public Sprite UndeadSecond;
    public Sprite UndeadThird;

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

        stateMachine.Transition("idle");
    }

    protected void CheckDurability()
    {
        if(Health < 2 * maxHealth / 3)
        {
            sr.sprite = UndeadSecond;//상체 파괴
        }
        if (Health < maxHealth / 3)
        {
            sr.sprite = UndeadThird;
            speed = 0.4f;//하체 파괴
        }
    }
}
