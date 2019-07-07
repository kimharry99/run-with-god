using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockProjectile : Projectile
{
    private Rigidbody2D rb;
    private Vector3 oriVelocity;

    protected override void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        oriVelocity = rb.velocity;
    }

    private void Update()
    {
        rb.velocity = oriVelocity * ClockBoss.timeSpeed;
    }
}
