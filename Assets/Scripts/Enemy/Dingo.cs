using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Dingo : NormalEnemy
{
	public override EnemyType Type { get { return EnemyType.DINGO; } }
    private Transform landChecker;
    private Collider2D col;
    public bool IsGround
    {
        get
        {
            //Debug.Log((Physics2D.Linecast(landChecker.position + new Vector3(-col.bounds.size.x / 2 - 0.01f, 0), landChecker.position + new Vector3(col.bounds.size.x / 2 + 0.01f, 0), 1 << LayerMask.NameToLayer("Ground")).transform != null)+"\nx:"+PlayerPosition.x+" y: "+PlayerPosition.y);
            return Physics2D.Linecast(landChecker.position + new Vector3(-col.bounds.size.x / 2 - 0.01f, 0), landChecker.position + new Vector3(col.bounds.size.x / 2 + 0.01f, 0), 1 << LayerMask.NameToLayer("Ground")).transform != null;
        }
    }
    protected override void Start()
    {
        Health = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        landChecker = transform.Find("LandChecker");
        InitEnemy();
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

    protected override void Moving()
    {
        if (    (rb.velocity.x < speed || rb.velocity.normalized != Direction) && IsGround  ) //목표 속력보다 현재 속력이 작을때 또는 현재 속도의 방향과 자신의 방향이 다를때
        {
            Debug.Log("ASFDSAFSD");
            rb.AddForce(acceleration * Direction); //가속도만큼 속도에 더합니다.
        }
    }

}
