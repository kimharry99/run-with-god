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

    protected override void Start()
    {
        base.Start();
        startPoint = transform.position;
    }
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        switch (type)
        {
            case ProjectileType.PLAYER:
                if (collision.tag.Contains("Enemy"))
                    collision.GetComponent<NormalEnemy>()?.GetDamaged(attack, transform.position, GetComponent<Rigidbody2D>().velocity);
                if (collision.tag != "Player" && collision.tag != "Projectile" && !collision.isTrigger)
                {
                    StopAllCoroutines();
                    Destroy(gameObject);
                }
                break;
            case ProjectileType.ENEMY:
                if (collision.tag == "Player" && PlayerController.inst.IsDamagable)
                {
                    collision.GetComponent<PlayerController>()?.GetDamaged();
                    StopAllCoroutines();
                    Destroy(gameObject);
                }
                if (collision.tag == "Ground" || collision.tag == "Wall")
                {
                    StopAllCoroutines();
                    Destroy(gameObject);
                }
                break;
        }
    }

    /*private void Update()
    {
        if(DistanceFromStart > inertiaReach)
        {
            GetComponent<Rigidbody2D>().AddForce(Vector2.down * mass);
        }
    }
    */

    protected float DistanceFromStart {
        get { return Vector3.Distance(startPoint, transform.position); }
    }
}
