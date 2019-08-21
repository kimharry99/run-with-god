using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodProjectile : Projectile
{
	private Vector3 oriPos;
	[SerializeField]
	private float radius = 0;
	private float distance = 0;
	
	public void SetStartValues(Vector3 startPosition, float startRadius)
	{
		oriPos = startPosition;
		radius = startRadius;
	}

	private void FixedUpdate()
	{
		radius += 720 * Time.deltaTime;
		distance += 5f * Time.deltaTime;
		transform.position = oriPos + Quaternion.Euler(0, 0, radius) * Vector2.right * distance;
	}

	protected override void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.tag == "Player" && PlayerController.inst.IsDamagable)
		{
			collision.GetComponent<PlayerController>()?.GetDamaged();
			StopAllCoroutines();
			Destroy(gameObject);
		}
	}
}
