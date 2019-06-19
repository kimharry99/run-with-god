using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    int attack = 1;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.tag == "Enemy")
		{
			//collision.GetComponent<NormalMonster>().hitEffect.transform.position = transform.position;
			collision.GetComponent<NormalMonster>().hitEffect.Play();
            collision.GetComponent<NormalMonster>().GetDamaged(attack);
            Destroy(gameObject);
		}
	}
}
