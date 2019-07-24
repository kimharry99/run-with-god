using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Dingo : NormalEnemy
{
	public override EnemyType Type { get { return EnemyType.DINGO; } }

    private Animator dingoAnimator;

    protected override void Start()
    {
        base.Start();
        dingoAnimator = GetComponent<Animator>();
    }

    protected override void InitEnemy()
    {
        State idle = new State();
        State move = new State();

        idle.Enter += delegate
        {
            dingoAnimator.SetBool("isRunning", false);
        };
        idle.StateUpdate += MonitorAndTransition;
        
        move.Enter += delegate
        {
            dingoAnimator.SetBool("isRunning", true);
        };
        move.StateUpdate += FollowPlayer;

        stateMachine.AddNewState("idle", idle);
        stateMachine.AddNewState("move", move);

        stateMachine.Transition("idle");
    }
}
