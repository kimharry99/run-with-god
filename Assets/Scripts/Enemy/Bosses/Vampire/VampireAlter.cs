using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VampireAlter : NormalEnemy
{
    public override EnemyType Type { get { return EnemyType.ALL; } }

    private float shotTimer = 5;
	private float healTimer = 5;

	public VampireBoss vampire;

    [SerializeField]
    private GameObject projectilePrefab;

    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    protected override void InitEnemy()
    {

    }

    protected override void Update()
    {
        base.Update();
		shotTimer -= Time.deltaTime;
		healTimer -= Time.deltaTime;

        if (shotTimer <= 1.5f)
        {
            anim.SetTrigger("Attack1");
        }

        if (shotTimer <= 0)
        {
            ShotProjectile();
            shotTimer = 2f;
        }

		if (healTimer <= 0)
		{
			vampire.GetHealed(10);
			healTimer = 2f;
		}
    }

    private void ShotProjectile()
    {
        Transform player = PlayerController.inst.transform;
        Instantiate(projectilePrefab,transform.position,transform.rotation)
            .GetComponent<Projectile>().Init((player.position - transform.position).normalized * 3, 100, 1, ProjectileType.ENEMY);
    }
}
