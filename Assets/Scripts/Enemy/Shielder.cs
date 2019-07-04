using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shielder : NormalEnemy
{
    public Transform Shield;

    protected override void Start()
    {
        base.Start();
        Shield = transform.Find("Shield");
    }
    protected override void InitEnemy()
    {
        State idle = new State();
        State move = new State();

        idle.StateUpdate += MonitorAndTransition;
        move.StateUpdate += FollowPlayer;

        stateMachine.AddNewState("idle", idle);
        stateMachine.AddNewState("move", move);

        stateMachine.Transtion("idle");

    }

    protected override void OnDead()
    {
        GameManager.inst.KillCount++;
        Destroy(gameObject);
        Destroy(transform.Find("Shield"));
    }

    protected override void Flip()
    {
        base.Flip();
        Vector3 pos = Shield.transform.localPosition;
        pos.x = -pos.x;
        Shield.transform.localPosition = pos;
    }
}