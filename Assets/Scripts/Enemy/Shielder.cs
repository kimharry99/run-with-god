using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shielder : NormalEnemy
{
    public ParticleSystem dead;

    protected override void InitEnemy()
    {
        State idle = new State();
        State move = new State();

        idle.StateUpdate += Idle;
        move.StateUpdate += Moving;

        stateMachine.AddNewState("idle", idle);
        stateMachine.AddNewState("move", move);

        stateMachine.Transtion("idle");
    }

    public override void GetDamaged(int damage)
    {
        if (sr.flipX)
        {
            if(PlayerController.inst.transform.position.x > transform.position.x)
            {
                Health -= damage;
            }
        }
        else
        {
            if(PlayerController.inst.transform.position.x < transform.position.x)
            {
                Health -= damage;
            }
        }
    }

    protected override void OnDead()
    {
        dead.Play();
        StartCoroutine(DissolveEffectRoutine(2));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerController pc = collision.gameObject.GetComponent<PlayerController>();
        if (pc != null && pc.IsDamagable)
            pc?.GetDamaged();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController pc = collision.GetComponent<PlayerController>();
        if (pc != null && pc.IsDamagable)
            pc?.GetDamaged();
    }

}