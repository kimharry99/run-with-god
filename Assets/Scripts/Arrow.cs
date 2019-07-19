using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : Projectile
{
    Vector3 startPoint;
    [SerializeField]
    float inertiaReach;
    [SerializeField]
    float mass;

    private void Start()
    {
        base.Start();
        startPoint = transform.position;
    }

    private void Update()
    {
        if(DistanceFromStart > inertiaReach)
        {
            GetComponent<Rigidbody2D>().AddForce(Vector2.down * mass);
        }
    }

    protected float DistanceFromStart {
        get { return Vector3.Distance(startPoint, transform.position); }
    }
}
