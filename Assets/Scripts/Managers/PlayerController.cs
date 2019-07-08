using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : SingletonBehaviour<PlayerController>
{
	private const float maxSpeed = 6;
	private const float jumpSpeed = 5;
	private const float explodeRange = 5;
	private const int maxLife = 10;

	//private float speed = 0;
	private int jumpCount = 2;
	private int shotCount;
	private float shotCooltime;

	private int _life;
	public int Life {
		get { return _life; }

		private set {
			_life = value;
			InGameUIManager.inst.UpdateLifeUI(_life);
		}
	}

    private Collider2D col;
	private Rigidbody2D rb;
	private SpriteRenderer sr;
	private Transform landChecker;
	private Transform shotPosition;

	private const float gracePeriod = 2.5f;
	private float graceTimer = 0;
	private float hitTimer;
	private float dashTimer;
	public bool IsDamagable { get { return graceTimer <= 0; } }

	[SerializeField]
	private AudioClip shotSFX = null, hitSFX = null, boomSFX = null;

	private bool IsGround
	{
		get {
            return Physics2D.Linecast(landChecker.position + new Vector3(-col.bounds.size.x / 2 - 0.01f, 0), landChecker.position + new Vector3(col.bounds.size.x / 2 + 0.01f, 0), 1 << LayerMask.NameToLayer("Ground")).transform != null;
        }
	}

	public Vector3 PlayerPosition
	{
		get { return transform.position; }
	}

	[SerializeField]
	private GameObject bulletPrefab = null;
	private StateMachine gunState = new StateMachine();
	private StateMachine playerState = new StateMachine();

    private Animator playerAnimator;

    public Action OnJump;

	private void Start()
	{
		rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
		landChecker = transform.Find("LandChecker");
		shotPosition = transform.Find("ShotPosition");
		sr = GetComponent<SpriteRenderer>();
        playerAnimator = GetComponent<Animator>();
		InitGunStateMachine();
		InitPlayerStateMachine();
		Life = 3;

		OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
	}

	private void Update()
	{
		gunState.UpdateStateMachine();
		playerState.UpdateStateMachine();

		if (graceTimer > 0)
			graceTimer -= Time.deltaTime;
		if (hitTimer > 0)
			hitTimer -= Time.deltaTime;
		if (dashTimer > 0)
			dashTimer -= Time.deltaTime;
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (scene.name == "InGameScene" || scene.name == "Boss")
		{
			gameObject.SetActive(true);
			playerState.Transition("idle");
		}
		else if (scene.name == "TrustSelection")
		{
			gameObject.SetActive(true);
			playerState.Transition("trustSelect");
		}
		else
		{
			//gameObject.SetActive(false);
		}
	}

	private void PlayerMovementControl()
	{
		float vertical = Input.GetAxis("Vertical"); //left, right input
		float horizontal = Input.GetAxis("Horizontal"); //up, down input
		float jump = Input.GetAxis("Jump"); //jump input
		float fire = Input.GetAxis("Fire"); //attack input

        playerAnimator.SetBool("isRunning", horizontal != 0);
        playerAnimator.SetBool("isGround", IsGround);
        transform.position += new Vector3(horizontal * maxSpeed * Time.deltaTime, 0, 0);

		if (IsGround)
		{
			jumpCount = 2;
		}
		else if (jumpCount > 1)
		{
			jumpCount = 1;
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
            OnJump?.Invoke();
		}

		if (Input.GetButtonDown("Dash") && dashTimer <= 0)
		{
			playerState.Transition("dash");
		}
	}

	private void PlayerAttackControl()
	{
		if (Input.GetButtonDown("Fire"))
		{
			gunState.Transition("fire");
		}
	}

	private void InitPlayerStateMachine()
	{
		State idle = new State();
		idle.StateUpdate += PlayerMovementControl;
		idle.StateUpdate += PlayerAttackControl;

		State hit = new State();
		hit.Enter += delegate
		{
			graceTimer = gracePeriod;
			hitTimer = 0.0f;
			StartCoroutine(GraceTimeRoutine());
			//StartCoroutine(HitRoutine());
		};

		hit.StateUpdate += delegate
		{
			if (hitTimer <= 0)
			{
				playerState.Transition("idle");
			}
		};

		State dash = new State();
		dash.Enter += delegate
		{
			graceTimer = 0.3f;
			gameObject.layer = LayerMask.NameToLayer("Player Grace");
			StartCoroutine(DashRoutine())
;		};

		dash.Exit += delegate
		{
			gameObject.layer = LayerMask.NameToLayer("Player");
			dashTimer = 0.5f;
		};

		State trustSelect = new State();
		trustSelect.StateUpdate += PlayerMovementControl;

		playerState.AddNewState("idle", idle);
		playerState.AddNewState("hit", hit);
		playerState.AddNewState("dash", dash);
		playerState.AddNewState("trustSelect", trustSelect);

		playerState.Transition("idle");
	}

	private IEnumerator GraceTimeRoutine()
	{
		Color oriColor = sr.color;
		gameObject.layer = LayerMask.NameToLayer("Player Grace");
		while (graceTimer > 0)
		{
			sr.color = new Color(oriColor.r, oriColor.g, oriColor.b, Mathf.Round(5 * graceTimer - (int)(5 * graceTimer)));
			yield return null;
		}
		sr.color = oriColor;
		gameObject.layer = LayerMask.NameToLayer("Player");
	}

    /*
	private IEnumerator HitRoutine()
	{
		Color oriColor = sr.color;
		for (float t = 0; t <= hitTimer; t += Time.deltaTime)
		{
			Vector2 force = new Vector2(hitTimer * (sr.flipX ? 1 : -1), 0);
			rb.AddForce(force * 10);

			yield return null;
		}
	}
    */

	private IEnumerator DashRoutine()
	{
		float oriGraceTimer = graceTimer;
		Vector3 oriPosition = transform.position;
		Vector3 destination = transform.position + (sr.flipX ? new Vector3(-3, 0) : new Vector3(3, 0));
		rb.simulated = false;
		sr.color = sr.color - new Color(0, 0, 0, 0.5f);

		while (graceTimer > 0)
		{
			transform.position = Vector3.Lerp(oriPosition, destination, 1 - Mathf.Pow(graceTimer / oriGraceTimer, 3));
			yield return null;
		}

		rb.simulated = true;
		//transform.position = destination;
		playerState.Transition("idle");
		sr.color = sr.color + new Color(0, 0, 0, 0.5f);
	}

	private void InitGunStateMachine()
	{
		State idle = new State();
		idle.StateUpdate += delegate
		{

		};
		State fire = new State();

		fire.Enter += delegate
		{
			shotCount = 3;
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
				gunState.Transition("idle");
			}
			if (Input.GetButtonDown("Fire"))
			{
				shotCount = 3;
			}
		};
		gunState.AddNewState("idle", idle);
		gunState.AddNewState("fire", fire);
		gunState.Transition("idle");
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
		bullet.transform.position = shotPosition.position + new Vector3(0, UnityEngine.Random.Range(-0.05f, 0.05f));
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

    public Boolean IsKill;
	public void GetDamaged()
	{
		graceTimer = gracePeriod;
		Life--;
		SoundManager.inst.PlaySFX(gameObject, hitSFX);
		playerState.Transition("hit");
		gunState.Transition("idle");
		Explode();
	}

	private void Explode()
	{
		CameraController.Shake(0.1f, 0.5f);

		Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
		Vector2 center = new Vector2(screenPos.x / Screen.width, screenPos.y / Screen.height);
		CameraController.ShockWave(center);
		CameraController.ChromaticAberration();
		SoundManager.inst.PlaySFX(gameObject, boomSFX);

		foreach (var enemy in Physics2D.OverlapCircleAll(transform.position,explodeRange,1 << LayerMask.NameToLayer("Enemy") | 1 << LayerMask.NameToLayer("Enemy Ghost")))
		{
            if(IsKill)
			enemy.GetComponent<NormalEnemy>()?.GetDamagedToDeath();
		}
	}

	private void OnDead()
	{
		
	}
}