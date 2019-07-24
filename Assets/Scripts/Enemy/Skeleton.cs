using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Skeleton : NormalEnemy
{
	public override EnemyType Type { get { return EnemyType.SKELETON; } }

    private Animator skeletonAnimator;

    protected override void Start()
    {
        base.Start();
        skeletonAnimator = GetComponent<Animator>();
    }
    protected override void InitEnemy()
    {
        State idle = new State();
        State move = new State();

        idle.Enter += delegate
        {
            skeletonAnimator.SetBool("isRunning", false);
        };
        idle.StateUpdate += MonitorAndTransition;

        move.Enter += delegate
        {
            skeletonAnimator.SetBool("isRunning", true);
        }; 
        move.StateUpdate += FollowPlayer;

        stateMachine.AddNewState("idle", idle);
        stateMachine.AddNewState("move", move);

        stateMachine.Transition("idle");
    }
}