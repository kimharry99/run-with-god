using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowderKeg : MonoBehaviour
{
	private const float range = 5; //Circle radius

	[SerializeField]
	private ParticleSystem explodeParticle; //explode effect

	private void OnTriggerEnter2D(Collider2D collision)
	{
		//if bullet, OnHit()
	}

	private void OnHit()
	{
		//BOOM!
		//Hint : Physics2D.CircleCastAll
	}
}
