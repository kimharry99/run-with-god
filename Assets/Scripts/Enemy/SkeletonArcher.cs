using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonArcher : NormalEnemy
{
    [SerializeField]
    float shotCooltime;
    float currentCooltime = 0;

    protected override void InitEnemy()
    {
        State idle = new State();
        State attack = new State();

        idle.StateUpdate += delegate
        {
            SeePlayer();
            MonitorAndTransition("attack");
        };

        attack.StateUpdate += delegate
        {
            currentCooltime -= Time.deltaTime;
            if (currentCooltime <= 0)
            {
                AttackProjectile();
                currentCooltime = shotCooltime;
            }
        };

        stateMachine.AddNewState("idle", idle);
        stateMachine.AddNewState("attack", attack);

        stateMachine.Transtion("idle");
    }
}