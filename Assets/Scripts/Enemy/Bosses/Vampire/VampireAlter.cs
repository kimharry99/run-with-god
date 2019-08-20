using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VampireAlter : NormalEnemy
{
    public override EnemyType Type { get { return EnemyType.ALL; } }

    private float shotTimer = 5;

    [SerializeField]
    private GameObject projectilePrefab;
    
    protected override void InitEnemy()
    {

    }

    protected override void Update()
    {
        base.Update();
        if (shotTimer > 0)
            shotTimer -= Time.deltaTime;
        if (shotTimer <= 0)
        {
            ShotProjectile();
            shotTimer = 2f;
        }
    }

    private void ShotProjectile()
    {
        Transform player = PlayerController.inst.transform;
        Instantiate(projectilePrefab,transform.position,transform.rotation)
            .GetComponent<Projectile>().Init((player.position - transform.position).normalized * 3, 100, 1, ProjectileType.ENEMY);
    }
}
