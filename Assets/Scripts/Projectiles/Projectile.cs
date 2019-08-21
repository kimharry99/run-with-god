using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ProjectileType
{
	PLAYER,
	ENEMY
}

public class Projectile : MonoBehaviour
{
	[SerializeField]
	protected int attack;
	[SerializeField]
	private float reach;
	[SerializeField]
	public ProjectileType type;

	protected virtual void Start()
	{
		StartCoroutine(DestroyRoutine(reach / GetComponent<Rigidbody2D>().velocity.magnitude));
	}

	public void Init(float reach)
	{
		this.reach = reach;
	}

	public void Init(float reach, int attack, ProjectileType type)
	{
		this.reach = reach;
		this.attack = attack;
		this.type = type;
	}

    public void Init(Vector2 velocity, float reach, int attack, ProjectileType type)
    {
        GetComponent<Rigidbody2D>().velocity = velocity;
        Init(reach, attack, type);
    }

	protected virtual void OnTriggerEnter2D(Collider2D collision)
	{
		switch (type)
		{
			case ProjectileType.PLAYER:
				if (collision.tag.Contains("Enemy"))
					collision.GetComponent<NormalEnemy>()?.GetDamaged(attack, transform.position, GetComponent<Rigidbody2D>().velocity);
				if (collision.tag != "Player" && collision.tag != "Projectile" && collision.tag != "Ground Passable" &&!collision.isTrigger)
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

	protected virtual IEnumerator DestroyRoutine(float time)
	{
		yield return new WaitForSeconds(time);
		Destroy(gameObject);
	}
}
