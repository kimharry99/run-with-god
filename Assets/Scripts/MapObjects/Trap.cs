using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
	private Vector3 oriPos;

	[SerializeField]
	private AnimationCurve moveCurve;

	private float moveTime = 5f;
	[SerializeField]
	private float moveTimer = 0f;

	[SerializeField]
	private float moveDelta = 1f;

	private void Start()
	{
		oriPos = transform.position;
	}

	private void Update()
	{
		moveTimer += Time.deltaTime;
		if (moveTimer > moveTime)
			moveTimer = 0;
		transform.position = new Vector3(oriPos.x, oriPos.y + moveDelta * moveCurve.Evaluate(moveTimer / moveTime));
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		if (collision.tag == "Player")
		{
			PlayerController pc = collision.GetComponent<PlayerController>();
			if (pc != null && pc.IsDamagable)
			{
				pc.GetDamaged();
			}
		}
	}
}
