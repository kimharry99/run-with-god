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
	private int attack;
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

	private void OnTriggerEnter2D(Collider2D collision)
	{
		switch (type)
		{
			case ProjectileType.PLAYER:
				if (collision.tag == "Enemy")
					collision.GetComponent<NormalEnemy>()?.GetDamaged(attack);
				if (collision.tag != "Player" && collision.tag != "Projectile")
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

	protected IEnumerator DestroyRoutine(float time)
	{
		yield return new WaitForSeconds(time);
		Destroy(gameObject);
	}

}
