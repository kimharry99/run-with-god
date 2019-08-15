using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonArcher : NormalEnemy
{
	public override EnemyType Type { get { return EnemyType.SKELETONARCHER; } }

	[SerializeField]
    float shotCooltime;
    float currentCooltime = 0.2f;

    protected override void InitEnemy()
    {
        State idle = new State();
        State attack = new State();
        State dead = new State();

        idle.StateUpdate += delegate
        {
            SeePlayer();
            MonitorAndTransition("attack");
        };

        attack.StateUpdate += delegate
        {
            SeePlayer();
            currentCooltime -= Time.deltaTime;
            if (currentCooltime <= 0)
            {
                AttackProjectile();
                currentCooltime = shotCooltime;
            }
        };

        stateMachine.AddNewState("idle", idle);
        stateMachine.AddNewState("attack", attack);
        stateMachine.AddNewState("dead", dead);

        stateMachine.Transition("idle");
    }

    protected override void Flip()
    {
        base.Flip();
        Vector3 pos = shotPosition.localPosition;
        pos.x = -pos.x;
        shotPosition.localPosition = pos;
    }

    protected override void OnDead()
    {
        base.OnDead();
        stateMachine.Transition("dead");
    }
}