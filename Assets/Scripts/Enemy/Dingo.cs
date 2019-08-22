using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Dingo : NormalEnemy
{
	public override EnemyType Type { get { return EnemyType.DINGO; } }
    public AudioSource run;
    [SerializeField]
    private Transform landChecker;
    private Collider2D col;
    public bool IsGround
    {
        get
        {
            //Debug.Log((Physics2D.Linecast(landChecker.position + new Vector3(-col.bounds.size.x / 2 - 0.01f, 0), landChecker.position + new Vector3(col.bounds.size.x / 2 + 0.01f, 0), 1 << LayerMask.NameToLayer("Ground")).transform != null)+"\nx:"+PlayerPosition.x+" y: "+PlayerPosition.y);
            return Physics2D.OverlapArea(landChecker.position + new Vector3(-col.bounds.size.x / 2, 0), landChecker.position + new Vector3(col.bounds.size.x / 2, 0), 1 << LayerMask.NameToLayer("Ground") | 1 << LayerMask.NameToLayer("Ground Passable"))?.transform != null;
        }
    }

    [SerializeField]
    private Animator dingoAnimator;

    protected override void Start()
    {
        Health = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        InitEnemy();
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
            if (run != null)
                run.Play();
            dingoAnimator.SetBool("isRunning", true);
        };

        move.Exit += delegate
        {
            if (run != null)
                run.Stop();
        };
        move.StateUpdate += FollowPlayer;

        stateMachine.AddNewState("idle", idle);
        stateMachine.AddNewState("move", move);

        stateMachine.Transition("idle");
    }

    protected override void Moving()
    {
        /*
        if (rb.velocity.magnitude >= speed)
        {
            rb.velocity = speed * rb.velocity.normalized;
        }
        else if ((rb.velocity.normalized != Direction)&&IsGround) //목표 속력보다 현재 속력이 작을때 또는 현재 속도의 방향과 자신의 방향이 다를때
        {
            rb.AddForce(acceleration * Direction); //가속도만큼 속도에 더합니다.
            
        }
        */
        if ((rb.velocity.magnitude < speed || rb.velocity.normalized != Direction) && IsGround)
        { //목표 속력보다 현재 속력이 작을때 또는 현재 속도의 방향과 자신의 방향이 다를때
            rb.velocity += new Vector2(acceleration * Time.deltaTime, 0)*Direction;
            Debug.Log(new Vector2(acceleration * Time.deltaTime, 0) * Direction);
        }
    }
}
