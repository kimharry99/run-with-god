using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : NormalEnemy
{
	public override EnemyType Type { get { return EnemyType.SLIME; } }

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

    protected override void OnDead()
    {
        if (maxHealth > 1) {
            Split();
        } else {
            Destroy(gameObject);            //이 오브젝트를 파괴합니다.
        }
    }

    protected void Split()   //num개 만큼 분열합니다.
    {
        maxHealth--;
        Health = maxHealth;

        GameObject clone = GameObject.Instantiate(this.gameObject) as GameObject;

        this.transform.position += Vector3.left * maxHealth * 0.1f;
        clone.transform.position += Vector3.right * maxHealth * 0.1f;
    }
}