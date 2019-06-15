using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	private const float maxSpeed = 5;
	private float speed = 0;
	private const float jump = 5;
	private int jumpCount = 2;

	private Rigidbody2D rb;

	private void Start()
	{
		rb = GetComponent<Rigidbody2D>();
	}

	private void Update()
	{
		float vertical = Input.GetAxis("Vertical"); //left, right input
		float horizontal = Input.GetAxis("Horizontal"); //up, down input
		float jump = Input.GetAxis("Jump"); //jump input
		float fire = Input.GetAxis("Fire"); //attack input
	}
}