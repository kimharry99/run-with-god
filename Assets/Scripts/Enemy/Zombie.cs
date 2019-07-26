using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Zombie : NormalEnemy
{
	public override EnemyType Type { get { return EnemyType.ZOMBIE; } }

    private Animator zombieAnimator;

    protected override void Start()
    {
        base.Start();
        zombieAnimator = GetComponent<Animator>();
    }

    protected override void InitEnemy()
	{
        State idle = new State();
        State move = new State();

        idle.Enter += delegate
        {
            zombieAnimator.SetBool("isRunning", false);
        };
        idle.StateUpdate += MonitorAndTransition;

        move.Enter += delegate
        {
            zombieAnimator.SetBool("isRunning", true);
        };

        move.StateUpdate += Moving;

        stateMachine.AddNewState("idle", idle);
		stateMachine.AddNewState("move", move);

		stateMachine.Transition("idle");
	}
 
}
