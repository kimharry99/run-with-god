using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shielder : NormalEnemy
{
    public ParticleSystem dead;

    protected override void InitEnemy()
    {
        State stand = new State();
        State move = new State();

        stand.StateUpdate += Stand;
        move.StateUpdate += FollowPlayer;

        stateMachine.AddNewState("stand", stand);
        stateMachine.AddNewState("move", move);

        stateMachine.Transtion("stand");

    }

    public override void GetDamaged(int damage)
    {
        if (sr.flipX)
        {
            if(PlayerController.inst.transform.position.x > transform.position.x)
            {
                hitEffect.Play();
                Health -= damage;
            }
        }
        else
        {
            if(PlayerController.inst.transform.position.x < transform.position.x)
            {
                hitEffect.Play();
                Health -= damage;
            }
        }
    }

    private void Stand()
    {
        if (DetectPlayer(range))
        {
            stateMachine.Transtion("move");
        }
   
    }

}