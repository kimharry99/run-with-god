using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat : NormalEnemy
{
	public override EnemyType Type { get { return EnemyType.BAT; } }

    //private int cnt = 1;
    private bool direction;
    private float x;
    private Vector3 initialPosition;
    public ParticleSystem dead;


    protected override void Start()
    {
        base.Start();
    }

    protected override void InitEnemy()
	{
        State dead = new State();
        State roamc = new State();
        //State roamcc = new State();

        dead.StateUpdate += Dead;
        roamc.StateUpdate += RoamC;
        //roamcc.StateUpdate += RoamCC;

        stateMachine.AddNewState("roamc",roamc);
        //stateMachine.AddNewState("roamcc", roamcc);
		stateMachine.AddNewState("dead", dead);
        /*
        if (!direction)
        {
            stateMachine.Transition("roamc");
            Flip();
        }
        else
        {
            stateMachine.Transition("roamcc");
            Flip();
        }
        */
        initialPosition = transform.position;
        x = Random.Range(0f, 2f);
        direction = x < 1f;
        if (!direction)
        {
            Flip();
        }
        stateMachine.Transition("roamc");
        
       
    }
    protected void RoamC()
    {
        if(Health <= 0)
            stateMachine.Transition("dead");
        /*
        if (x < 2 * Mathf.PI)
        {

            transform.position = new Vector2(transform.position.x + Time.deltaTime, transform.position.y + Mathf.Cos(x));
            x += Time.deltaTime;
        }
        else
        {
            if (cnt == 1)
            {
                Flip();
                cnt--;
            }
            transform.position = new Vector2(transform.position.x - 0.01f, transform.position.y + (0.4f * Mathf.Sin(x + 0.05f) - 0.4f * Mathf.Sin(x)) / 2);
            x += Time.deltaTime;
            if (x > 4 * Mathf.PI)
            {
                Flip();
                x -= 4 * Mathf.PI;
                cnt++;
            }
        }
        */

        if (x < 1)
        {
            if (!direction)
            {
                Flip();
                direction = true;
            }
            transform.position = new Vector3(initialPosition.x + 0.8f*(x - 0.5f), initialPosition.y + 0.2f * Mathf.Sin(2 * (x - 0.5f) * Mathf.PI), 0);
            x += Time.deltaTime / 2;
        }
        else
        {
            if (direction)
            {
                Flip();
                direction = false;
            }
            transform.position = new Vector3(initialPosition.x - 0.8f*(x - 1.5f), initialPosition.y + 0.2f * Mathf.Sin(2 * (x-0.5f) * Mathf.PI), 0);
            x += Time.deltaTime / 2;
            if (x >= 2)
            {
                x -= 2;
            }
        }

    }
    /*
    protected void RoamCC()
    {
  
        if (Health <= 0)
            stateMachine.Transition("dead");

        if (x > -2 * Mathf.PI)
        {
            transform.position = new Vector3(transform.position.x + 0.01f, transform.position.y + (0.4f*Mathf.Sin(x + 0.05f) - 0.4f*Mathf.Sin(x)) / 2, transform.position.z);
            x -= 0.07f;
        }
        else
        {
            if(cnt == 1)
            {
                Flip();
                cnt--;
            }
            transform.position = new Vector3(transform.position.x - 0.01f, transform.position.y + (0.4f*Mathf.Sin(x + 0.05f) - 0.4f*Mathf.Sin  (x)) / 2, transform.position.z);
            x -= 0.07f;
            if (x < -4 * Mathf.PI)
            {
                Flip();
                cnt++;
                x += 4 * Mathf.PI;
            }
        }
        if (x < 1)
        {
            if (!direction)
            {
                Flip();
                direction = true;
            }
            transform.position = new Vector3(initialPosition.x + (x - 0.5f), initialPosition.y - 0.2f * Mathf.Sin(2 * (x - 0.5f) * Mathf.PI), 0);
            x += Time.deltaTime / 2;
        }
        else
        {
            if (direction)
            {
                Flip();
                direction = false;
            }
            transform.position = new Vector3(initialPosition.x - (x - 1.5f), initialPosition.y - 0.2f * Mathf.Sin(2 * (x - 0.5f) * Mathf.PI), 0);
            x += Time.deltaTime / 2;
            if (x >= 2)
            {
                x -= 2;
                Debug.Log(transform.position);
            }
        }
    }
    */

    protected void Dead()
    {
        
    }
}