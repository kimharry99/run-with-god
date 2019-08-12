using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : SingletonBehaviour<PlayerController>
{
	private const float maxSpeed = 5;
	private const float jumpSpeed = 4.7f;
	private const float explodeRange = 5;
	private const int maxLife = 10;

	public float speedScale = 1;
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

    private int _explodeItem;
    public int ExplodeItem
    {
        get { return _explodeItem; }
        private set
        {
            _explodeItem = value;
        }
    }

    private Collider2D col;
	private Rigidbody2D rb;
	private SpriteRenderer sr;
    private Transform arm;
	private Transform landChecker;
	private Transform shotPosition;

	private const float gracePeriod = 2.5f;
	private float graceTimer = 0;
	private float hitTimer;
    private float dashTimer;
	private float dashCoolTimer;
	public bool IsDamagable { get { return graceTimer <= 0; } }

	[SerializeField]
	private AudioClip shotSFX = null, hitSFX = null, boomSFX = null;
    [SerializeField]
    private Light gunFireLight;
	[SerializeField]
	private AudioSource move;
	[SerializeField]
	private AudioClip walkSFX, jumpSFX;

	public bool IsGround
	{
		get {
            return Physics2D.Linecast(landChecker.position + new Vector3(-col.bounds.size.x / 2 - 0.01f, 0),
                landChecker.position + new Vector3(col.bounds.size.x / 2 + 0.01f, 0),
                1 << LayerMask.NameToLayer("Ground") | 1 << LayerMask.NameToLayer("Ground Passable")).transform != null;
        }
	}

    public bool IsFlipped { get; private set; }

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
    public Action OnShotBullet;
    public Action OnDash;
    public Action GetHit;

	[SerializeField]
	private ParticleSystem walkEffect;

	#region Unity Functions
	private void Awake()
	{
		if (inst != this)
		{
			Destroy(gameObject);
			return;
		}
		else
			SetStatic();

		rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
		landChecker = transform.Find("LandChecker");
        arm = transform.Find("Arm");
		shotPosition = arm.Find("ShotPosition");
		sr = GetComponent<SpriteRenderer>();
        playerAnimator = GetComponent<Animator>();
		InitGunStateMachine();
		InitPlayerStateMachine();
		ResetPlayer();

        SceneManager.sceneLoaded += OnSceneLoaded;
		//OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
	}

	private void Update()
	{
		gunState.UpdateStateMachine();
		playerState.UpdateStateMachine();

		if (graceTimer > 0)
			graceTimer -= Time.deltaTime;
		if (hitTimer > 0)
			hitTimer -= Time.deltaTime;
        if (dashCoolTimer > 0)
            dashCoolTimer -= Time.deltaTime;
        if (dashTimer > 0)
            dashTimer -= Time.deltaTime;
        gunFireLight.intensity -= 100 * Time.deltaTime;
        gunFireLight.enabled = gunFireLight.intensity >= 0;
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

#if UNITY_EDITOR
	private void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawLine(landChecker.position + new Vector3(-col.bounds.size.x / 2 - 0.01f, 0), landChecker.position + new Vector3(col.bounds.size.x / 2 + 0.01f, 0));
	}
#endif
	#endregion

	#region Control Functions
	private void PlayerMovementControl()
	{
		float vertical = Input.GetAxis("Vertical"); //left, right input
		float horizontal = Input.GetAxis("Horizontal"); //up, down input
		//float horizontalJump = Input.GetAxis("Horizontal_Jump");
		float jump = Input.GetAxis("Jump"); //jump input
		float fire = Input.GetAxis("Fire"); //attack input

		playerAnimator.SetBool("isRunning", horizontal != 0);
		playerAnimator.SetBool("isGround", IsGround);
		playerAnimator.SetFloat("ShootUp", vertical);
		//transform.position += new Vector3(horizontal * maxSpeed * Time.deltaTime, 0, 0);
		/*
		if (IsGround)
            rb.velocity = new Vector2(horizontal * maxSpeed * speedScale, rb.velocity.y);
        else
            rb.velocity = new Vector2(horizontalJump * maxSpeed * speedScale, rb.velocity.y);
		*/
		rb.velocity = new Vector2(horizontal * maxSpeed * speedScale, rb.velocity.y);
		if (horizontal != 0 && IsGround)
			walkEffect.Play();

		if (IsGround)
		{
			jumpCount = 2;
		}
		else if (jumpCount > 1)
		{
			jumpCount = 1;
		}

		if ((horizontal < 0 && !IsFlipped) || (horizontal > 0 && IsFlipped))
		{
			Flip();
		}

		if (Input.GetButtonDown("Jump") && Input.GetButton("Vertical") && vertical <= -1)
		{
			if (IsGround)
				StartCoroutine(JumpOff());
			else
				playerState.Transition("stamp");
		}
		else if (Input.GetButtonDown("Jump") && jumpCount > 0)
		{
			rb.velocity += new Vector2(0, jumpSpeed - rb.velocity.y);
			//rb.velocity +=  rb.velocity.y < 0 ? new Vector2(0, jumpSpeed - rb.velocity.y) : new Vector2(0, jumpSpeed);
			jumpCount--;
			move.Stop();
			move.clip = jumpSFX;
			move.Play();
			OnJump?.Invoke();
			walkEffect.Play();
		}

		if (Input.GetButtonDown("Dash") && dashCoolTimer <= 0)
		{
			playerState.Transition("dash");
		}

		if (horizontal != 0 && IsGround && !move.isPlaying)
		{
			move.clip = walkSFX;
			move.Play();
		}

		Camera.main.GetComponent<CameraController>()?.SetCameraOffset(horizontal * 1f, vertical * 0.5f);
	}

	private void PlayerAttackControl()
	{
		if (Input.GetKeyDown(KeyCode.A) && ExplodeItem > 0)
		{
			ExplodeItem--;
			Explode();
		}

		if (Input.GetButtonDown("Fire"))
		{
			gunState.Transition("fire");
		}
	}

	#endregion

	#region Initialize Functions
	public void ResetPlayer()
	{
		foreach (var renderer in GetComponentsInChildren<SpriteRenderer>())
		{
			renderer.color = Color.white;
		}
		gameObject.layer = LayerMask.NameToLayer("Player");
		graceTimer = 0;
		Life = 3;
		ExplodeItem = 5;
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
			graceTimer = Mathf.Max(0.3f, graceTimer);
			dashTimer = 0.3f;
			gameObject.layer = LayerMask.NameToLayer("Player Grace");
			StartCoroutine(DashRoutine());
			OnDash?.Invoke();
			;
		};

		dash.Exit += delegate
		{
			if (graceTimer < 0)
				gameObject.layer = LayerMask.NameToLayer("Player");
			dashCoolTimer = 0.3f;
		};

		State stamp = new State();
		stamp.Enter += delegate { StartCoroutine(StampRoutine()); };

		State dead = new State();
		dead.Enter += OnDead;

		State trustSelect = new State();
		trustSelect.StateUpdate += PlayerMovementControl;

		playerState.AddNewState("idle", idle);
		playerState.AddNewState("hit", hit);
		playerState.AddNewState("dash", dash);
		playerState.AddNewState("stamp", stamp);
		playerState.AddNewState("dead", dead);
		playerState.AddNewState("trustSelect", trustSelect);

		playerState.Transition("idle");
	}

	private void InitGunStateMachine()
	{
		State idle = new State();
		idle.Enter += delegate
		{
			playerAnimator.SetBool("isShooting", false);
		};

		idle.StateUpdate += delegate
		{

		};
		State fire = new State();

		fire.Enter += delegate
		{
			shotCount = 3;
			shotCooltime = 0;
			playerAnimator.SetBool("isShooting", true);
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
	#endregion

	#region Action Functions
	private void Flip()
	{
		IsFlipped = !IsFlipped;
		transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
		ParticleSystem.ForceOverLifetimeModule fol = walkEffect.forceOverLifetime;
		fol.xMultiplier = -fol.xMultiplier;
		ParticleSystem.MainModule main = walkEffect.main;
		main.startSpeed = new ParticleSystem.MinMaxCurve(-main.startSpeed.constant);
		/*
        foreach (Transform child in transform)
        {
            if (child == transform)
            {
                Debug.Log("B");
                continue;
            }
                
            Vector3 scale = child.transform.localScale;
            child.transform.localScale = new Vector3(-scale.x,scale.y,scale.z);
            SpriteRenderer childSr = GetComponent<SpriteRenderer>();
            if (childSr != null)
            {
                childSr.flipX = !childSr.flipX;
            }
        }
        */
	}

	private void ShotBullet()
	{
		gunFireLight.intensity = 10;

		GameObject bullet = Instantiate(bulletPrefab);
		bullet.transform.position = shotPosition.position + new Vector3(0, UnityEngine.Random.Range(-0.01f, 0.01f));
		bullet.GetComponent<Rigidbody2D>().velocity = ShotDirection() * 25f;
		shotCount--;
		shotCooltime = 0.05f;
		CameraController.Shake(0.02f, 0.05f);
		SoundManager.inst.PlaySFX(gameObject, shotSFX);
		OnShotBullet?.Invoke();
	}

	public void GetDamaged()
	{
		graceTimer = gracePeriod;
		Life--;
		SoundManager.inst.PlaySFX(gameObject, hitSFX);
		playerState.Transition("hit");
		gunState.Transition("idle");
		//Explode();
		CameraController.Shake(0.2f, 0.5f);
		CameraController.ChromaticAberration();
		CameraController.HitEffect();
		GetHit?.Invoke();
		if (Life <= 0)
		{
			playerState.Transition("dead");
		}
	}

#if UNITY_EDITOR
	public Boolean IsKill;
#endif
	private void Explode()
	{
		CameraController.Shake(0.1f, 0.5f);

		Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
		Vector2 center = new Vector2(screenPos.x / Screen.width, screenPos.y / Screen.height);
		CameraController.ShockWave(center);
		CameraController.ChromaticAberration();
		SoundManager.inst.PlaySFX(gameObject, boomSFX);

		foreach (var enemy in Physics2D.OverlapCircleAll(transform.position, explodeRange, 1 << LayerMask.NameToLayer("Enemy") | 1 << LayerMask.NameToLayer("Enemy Ghost")))
		{
#if UNITY_EDITOR
			if (IsKill)
#endif
				enemy.GetComponent<NormalEnemy>()?.GetDamagedToDeath();
		}

		foreach (var projectile in Physics2D.OverlapCircleAll(transform.position, explodeRange))
		{
			if (projectile.tag == "Projectile")
			{
				if (projectile.GetComponent<Projectile>()?.type == ProjectileType.ENEMY)
				{
					Destroy(projectile.gameObject);
				}
			}
		}
	}

	private void OnDead()
	{
		StopAllCoroutines();
		StartCoroutine(DeadRoutine());
	}
	#endregion

	#region Action Routine Enumerators
	private IEnumerator GraceTimeRoutine()
	{
		Color oriColor = sr.color;
		gameObject.layer = LayerMask.NameToLayer("Player Grace");
		while (graceTimer > 0)
		{
			foreach (Transform tf in transform)
			{
				SpriteRenderer renderer = tf.GetComponent<SpriteRenderer>();
				if (renderer != null)
				{
					renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, Mathf.Round(5 * graceTimer - (int)(5 * graceTimer)));
				}
			}
			sr.color = new Color(oriColor.r, oriColor.g, oriColor.b, Mathf.Round(5 * graceTimer - (int)(5 * graceTimer)));
			yield return null;
		}
		foreach (Transform tf in transform)
		{
			SpriteRenderer renderer = tf.GetComponent<SpriteRenderer>();
			if (renderer != null)
			{
				renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, 1);
			}
		}
		sr.color = oriColor;
		gameObject.layer = LayerMask.NameToLayer("Player");
	}

	private IEnumerator DashRoutine()
	{
		float oriDashTimer = dashTimer;
		Vector3 oriPosition = transform.position;
		Vector3 destination = transform.position + (IsFlipped ? new Vector3(-3, 0) : new Vector3(3, 0));

		float offsetX = GetComponent<BoxCollider2D>().bounds.size.x / 2;
		float blockDistance = 3 + offsetX;
		foreach (var hit in Physics2D.BoxCastAll(oriPosition, GetComponent<BoxCollider2D>().bounds.size, 0, IsFlipped ? Vector2.left : Vector2.right, 3, 1 << LayerMask.NameToLayer("Ground") | 1 << LayerMask.NameToLayer("Wall")))
		{
			if (blockDistance > hit.distance)
			{
				blockDistance = hit.distance;
			}
		}
		blockDistance -= offsetX;


		rb.simulated = false;
		sr.color = sr.color - new Color(0, 0, 0, 0.5f);
		foreach (Transform tf in transform)
		{
			SpriteRenderer renderer = tf.GetComponent<SpriteRenderer>();
			if (renderer != null)
			{
				renderer.color -= new Color(0, 0, 0, 0.5f);
			}
		}

		while (dashTimer > 0)
		{
			Vector3 lerped = Vector3.Lerp(oriPosition, destination, 1 - Mathf.Pow(dashTimer / oriDashTimer, 3));

			if (Vector3.Distance(lerped, oriPosition) <= blockDistance)
				transform.position = lerped;
			else
				transform.position = oriPosition + (IsFlipped ? Vector3.left : Vector3.right) * blockDistance;
			yield return null;
		}

		rb.simulated = true;
		rb.velocity = new Vector2(rb.velocity.x, 0);
		playerState.Transition("idle");
		sr.color = sr.color + new Color(0, 0, 0, 0.5f);
		foreach (Transform tf in transform)
		{
			SpriteRenderer renderer = tf.GetComponent<SpriteRenderer>();
			if (renderer != null)
			{
				renderer.color += new Color(0, 0, 0, 0.5f);
			}
		}
	}

	private IEnumerator DeadRoutine()
	{
		yield return new WaitForSeconds(1f);
		GameManager.inst.GameOver();
	}

	private IEnumerator StampRoutine()
	{
		RaycastHit2D targetGround = Physics2D.Raycast(transform.position, Vector2.down, float.PositiveInfinity, 1 << LayerMask.NameToLayer("Ground") | 1 << LayerMask.NameToLayer("Ground Passable"));
		if (targetGround.collider != null)
		{
			gameObject.layer = LayerMask.NameToLayer("Player Grace");
			rb.velocity = Vector2.zero;
			rb.gravityScale = 100;
			//transform.position = targetGround.point + new Vector2(0, GetComponent<Collider2D>().bounds.extents.y);

			while (!IsGround)
				yield return null;

			playerAnimator.SetBool("isRunning", false);
			playerAnimator.SetBool("isGround", IsGround);
			rb.gravityScale = 1;
			CameraController.Shake(0.2f, 0.5f);
			SoundManager.inst.PlaySFX(gameObject, boomSFX);

			foreach (var enemy in Physics2D.OverlapCircleAll(transform.position, 0.5f, 1 << LayerMask.NameToLayer("Enemy") | 1 << LayerMask.NameToLayer("Enemy Ghost")))
			{
				enemy.GetComponent<NormalEnemy>()?.GetDamagedToDeath();
			}

			yield return new WaitForSeconds(1);
			if (graceTimer <= 0)
				gameObject.layer = LayerMask.NameToLayer("Player");
		}
		playerState.Transition("idle");
	}

	private IEnumerator JumpOff()
	{
		Collider2D other = Physics2D.OverlapPoint(landChecker.position, 1 << LayerMask.NameToLayer("Ground Passable"));
		if (other == null)
			yield break;
		Physics2D.IgnoreCollision(col, other, true);
		yield return new WaitForSeconds(0.3f);
		Physics2D.IgnoreCollision(col, other, false);
	}
	#endregion

	#region Utility Functions
	private Vector2 ShotDirection()
	{
		return (IsFlipped ? -1 : 1) * (arm.rotation * Vector2.right).normalized;
		/*
		Vector2 direction = Vector2.zero;
		float vertical = Input.GetAxis("Vertical");
        if (vertical != 0)
        {
            direction = new Vector2(0, 25 * (vertical > 0 ? 1 : -1));
        }
        else
        {
            direction = new Vector2(25 * (IsFlipped ? -1 : 1), 0);
        }
        
        return direction;
        */

	}
	#endregion

	#region Deprecated Functions
	/*
	private IEnumerator HitRoutine()
	{
		Color oriColor = sr.color;
		for (float t = 0; t <= hitTimer; t += Time.deltaTime)
		{
			Vector2 force = new Vector2(hitTimer * (isFlipped ? 1 : -1), 0);
			rb.AddForce(force * 10);

			yield return null;
		}
	}
    */
	#endregion
}