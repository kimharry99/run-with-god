using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shielder : NormalEnemy
{
	public override EnemyType Type { get { return EnemyType.SHIELDER; } }

	public GameObject Shield;
    private SpriteRenderer Child;
    protected override void Start()
    {
        base.Start();
       // Shield = transform.Find("Shield");
        Child = Shield.GetComponent<SpriteRenderer>();
    }
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