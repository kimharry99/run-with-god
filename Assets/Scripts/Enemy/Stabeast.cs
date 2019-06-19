using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stabeast : NormalMonster
{
    public int height;
    private int rushCount;
    public int maxRush;

    protected override void InitMonster()
    {
        State idle = new State();
        State fly = new State();
        State rush = new State();

        idle.StateUpdate += Prepare;
        fly.StateUpdate += Fly;
        rush.StateUpdate += Rush;

        stateMachine.AddNewState("idle", idle);
        stateMachine.AddNewState("fly", fly);
        stateMachine.AddNewState("rush", rush);

        stateMachine.Transtion("idle");
    }

    protected void Prepare()
    {
        stateMachine.Transtion("fly");
    }

    protected void Fly()
    {
        Vector3 destination = new Vector3(transform.position.x, height, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, destination, Time.deltaTime * speed * (rushCount + 1));

        if (Vector3.Distance(transform.position, destination) < 0.5)
        {
            stateMachine.Transtion("rush");
            rushTo = GetPlayer().transform.position;
        }
    }

    Vector3 rushTo;
    protected void Rush()
    {
        transform.position = Vector3.Lerp(transform.position, rushTo, Time.deltaTime * speed * 6);

        if (Vector3.Distance(transform.position, rushTo) < 0.5)
        {
            CameraController.Shake(0.02f * rushCount, 0.05f * rushCount);
            rushCount++;
            if (rushCount > maxRush)
                rushCount = 0;
            stateMachine.Transtion("fly");
        }
    }
}
