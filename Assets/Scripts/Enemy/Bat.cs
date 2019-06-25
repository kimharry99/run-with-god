using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Bat : NormalEnemy
{

    private double coordinate = 0;
    public ParticleSystem dead;
    protected void Roam()
    {
        
        if (coordinate < 2 * Math.PI) {
            transform.position = new Vector3(transform.position.x + 0.01f, transform.position.y + ((float)Math.Sin(coordinate + 0.05) - (float)Math.Sin(coordinate))/2, transform.position.z);
            coordinate += 0.05;
        }
        else
        {
            transform.position = new Vector3(transform.position.x - 0.01f, transform.position.y + ((float)Math.Sin(coordinate + 0.05) - (float)Math.Sin(coordinate))/2, transform.position.z);
            coordinate += 0.05;
            if (coordinate > 4 * Math.PI)
            {
                coordinate -= 4 * Math.PI;
            }
        }
    }

    protected override void InitEnemy()
	{

        State roam = new State();

        roam.StateUpdate += Roam;

        stateMachine.AddNewState("roam", roam);

        stateMachine.Transtion("roam");
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

