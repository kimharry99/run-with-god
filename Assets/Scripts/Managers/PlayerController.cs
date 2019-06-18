using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : SingletonBehaviour<PlayerController>
{
	private const float maxSpeed = 5;
	private float speed = 0;
	private const float jumpSpeed = 5;
	private int jumpCount = 2;
	private int shotCount;
	private float shotCooltime;

	private const int maxLife = 10;
	private int _life;
	public int Life { get { return _life; } private set { _life = value; } }

	private Rigidbody2D rb;
	private SpriteRenderer sr;
	private Transform landChecker;
	private Transform shotPosition;

	private const float gracePeriod = 2.5f;
	private float graceTimer = 0;
	public bool IsDamagable { get { return graceTimer <= 0; } }

	[SerializeField]
	private AudioClip shotSFX;

	private bool IsGround
	{
		get { return Physics2D.OverlapPoint(landChecker.position, 1 << LayerMask.NameToLayer("Ground")) != null; }
	}

	[SerializeField]
	private GameObject bulletPrefab;

	private StateMachine gunState = new StateMachine();

	private void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		landChecker = transform.Find("LandChecker");
		shotPosition = transform.Find("ShotPosition");
		sr = GetComponent<SpriteRenderer>();
		InitGunStateMachine();
	}

	private void Update()
	{
		PlayerMovementControl();
		gunState.UpdateStateMachine();
	}

	private void PlayerMovementControl()
	{
		float vertical = Input.GetAxis("Vertical"); //left, right input
		float horizontal = Input.GetAxis("Horizontal"); //up, down input
		float jump = Input.GetAxis("Jump"); //jump input
		float fire = Input.GetAxis("Fire"); //attack input

		transform.position += new Vector3(horizontal * maxSpeed * Time.deltaTime, 0, 0);

		if (IsGround)
		{
			jumpCount = 2;
		}

		if ((horizontal < 0 && !sr.flipX) || (horizontal > 0 && sr.flipX))
		{
			Flip();
		}

		if (Input.GetButtonDown("Jump") && jumpCount > 0)
		{
			rb.velocity += new Vector2(0, jumpSpeed - rb.velocity.y);
			//rb.velocity +=  rb.velocity.y < 0 ? new Vector2(0, jumpSpeed - rb.velocity.y) : new Vector2(0, jumpSpeed);
			jumpCount--;
		}
	}

	private void InitGunStateMachine()
	{
		State idle = new State();
		idle.StateUpdate += delegate
		{
			if (Input.GetButtonDown("Fire"))
			{
				gunState.Transtion("fire");
			}
		};
		State fire = new State();

		fire.Enter += delegate
		{
			shotCount = 5;
			shotCooltime = 0;
		};

		fire.StateUpdate += delegate
		{
			shotCooltime -= Time.deltaTime;
			if (shotCooltime <= 0)
			{
				ShotBullet();
			}
			if (shotCount <= 0)
			{
				gunState.Transtion("idle");
			}
			if (Input.GetButtonDown("Fire"))
			{
				shotCount = 5;
			}
		};
		gunState.AddNewState("idle", idle);
		gunState.AddNewState("fire", fire);
		gunState.Transtion("idle");
	}

	private void Flip()
	{
		sr.flipX = !sr.flipX;
		Vector3 pos = shotPosition.localPosition;
		shotPosition.localPosition = new Vector3(-pos.x, pos.y, pos.z);
	}

	private void ShotBullet()
	{
		GameObject bullet = Instantiate(bulletPrefab);
		bullet.transform.position = shotPosition.position + new Vector3(0, Random.Range(-0.05f, 0.05f));
		bullet.GetComponent<Rigidbody2D>().velocity = ShotDirection() * 1f;
		shotCount--;
		shotCooltime = 0.05f;
		CameraController.Shake(0.02f,0.05f);
		SoundManager.inst.PlaySFX(gameObject, shotSFX);
	}

	private Vector2 ShotDirection()
	{
		Vector2 direction = Vector2.zero;
		float vertical = Input.GetAxis("Vertical");

		if (!IsGround)
		{
			if (vertical != 0)
			{
				direction = new Vector2(0, 25 * (vertical > 0 ? 1 : -1));
			}
			else
			{
				direction = new Vector2(25 * (sr.flipX ? -1 : 1), 0);
			}
		}
		else
		{
			if (vertical > 0)
			{
				direction = new Vector2(0, 25 * (vertical > 0 ? 1 : -1));
			}
			else
			{
				direction = new Vector2(25 * (sr.flipX ? -1 : 1), 0);
			}
		}
		return direction;
	}

	public void GetDamaged()
	{
		graceTimer = gracePeriod;
		Life--;
		Explode();
	}

	private void Explode()
	{
		
	}

	private void OnDead()
	{
		
	}
}