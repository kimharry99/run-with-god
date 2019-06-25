﻿using System.Collections;
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
	ProjectileType type;

	private void Start()
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
		Debug.Log("A");
		switch (type)
		{
			case ProjectileType.PLAYER:
				if (collision.tag == "Enemy")
					collision.GetComponent<NormalEnemy>()?.GetDamaged(attack);
				if (collision.tag != "Player")
				{
					StopAllCoroutines();
					Destroy(gameObject);
				}
				break;
			case ProjectileType.ENEMY:
				if (collision.tag == "Player")
					collision.GetComponent<PlayerController>()?.GetDamaged();
				StopAllCoroutines();
				Destroy(gameObject);
				break;
		}
	}

	private IEnumerator DestroyRoutine(float time)
	{
		Debug.Log(time);
		yield return new WaitForSeconds(time);
		Destroy(gameObject);
	}

}
