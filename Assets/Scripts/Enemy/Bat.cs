using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat : NormalEnemy
{
	public override EnemyType Type { get { return EnemyType.BAT; } }

    private int cnt = 1;
    private int direction;
    private float coordinate;
    public ParticleSystem dead;


    protected override void Start()
    {
        base.Start();
        direction = Random.Range(0, 2);
        coordinate = Random.value * Mathf.PI + Random.value*Mathf.PI;
        transform.position = new Vector2(transform.position.x + coordinate * 0.2f, transform.position.y+0.2f * Mathf.Sin(coordinate));
    }

    protected override void InitEnemy()
	{
        State dead = new State();
        State roamc = new State();
        State roamcc = new State();

        dead.StateUpdate += Dead;
        roamc.StateUpdate += RoamC;
        roamcc.StateUpdate += RoamCC;

        stateMachine.AddNewState("roamc",roamc);
        stateMachine.AddNewState("roamcc", roamcc);
		stateMachine.AddNewState("dead", dead);

        if (direction == 1)
        {
            stateMachine.Transition("roamcc");
            Flip();
        }
        else stateMachine.Transition("roamc");
       
    }
    protected void RoamC()
    {
        if(Health <= 0)
            stateMachine.Transition("dead");
        
        if (coordinate < 2 * Mathf.PI)
        {

            transform.position = new Vector2(transform.position.x + 0.01f, transform.position.y + (0.4f * Mathf.Sin(coordinate + 0.05f) - 0.4f * Mathf.Sin(coordinate)) / 2);
            coordinate += 0.07f;
        }
        else
        {
            if (cnt == 1)
            {
                Flip();
                cnt--;
            }
            transform.position = new Vector2(transform.position.x - 0.01f, transform.position.y + (0.4f * Mathf.Sin(coordinate + 0.05f) - 0.4f * Mathf.Sin(coordinate)) / 2);
            coordinate += 0.07f;
            if (coordinate > 4 * Mathf.PI)
            {
                Flip();
                coordinate -= 4 * Mathf.PI;
                cnt++;
            }
        }
    }
    protected void RoamCC()
    {
        if (Health <= 0)
            stateMachine.Transition("dead");

        if (coordinate > -2 * Mathf.PI)
        {
            transform.position = new Vector3(transform.position.x + 0.01f, transform.position.y + (0.4f*Mathf.Sin(coordinate + 0.05f) - 0.4f*Mathf.Sin(coordinate)) / 2, transform.position.z);
            coordinate -= 0.07f;
        }
        else
        {
            if(cnt == 1)
            {
                Flip();
                cnt--;
            }
            transform.position = new Vector3(transform.position.x - 0.01f, transform.position.y + (0.4f*Mathf.Sin(coordinate + 0.05f) - 0.4f*Mathf.Sin  (coordinate)) / 2, transform.position.z);
            coordinate -= 0.07f;
            if (coordinate < -4 * Mathf.PI)
            {
                Flip();
                cnt++;
                coordinate += 4 * Mathf.PI;
            }
        }
    }

    protected void Dead()
    {
        
    }
}