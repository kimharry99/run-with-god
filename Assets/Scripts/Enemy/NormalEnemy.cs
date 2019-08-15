using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType
{
	ALL,
	BAT,
	DINGO,
	GHOST,
	SHIELDER,
	SKELETON,
	SKELETONARCHER,
	SLIME,
	STABEAST,
    UNDEAD,
	ZOMBIE,
	BOSS_CLOCK,
    BOSS_SPIDER,
    SHIELD
}

public abstract class NormalEnemy : MonoBehaviour
{
    float unit = 1f;//0.1f;          //거리의 임시 단위입니다. 플레이어의 가로가 확정되면 그 값을 배정합니다. 굳이 여기에 있을 필요는 없습니다.

	//변수들
	#region Monster Variables 

	public abstract EnemyType Type { get; }

    protected Vector2 Direction
    {
        get { return sr.flipX ? Vector2.left : Vector2.right; }
        //set { if (Direction != value.normalized) Flip(); }
    }

    //[SerializeField]
    private int _health;
    public int Health
    {
        get
        {
            return _health;
        }
        set
        {
            _health = value;

            if (Health <= 0)
            {
                OnDead();
            }
            if(_health > maxHealth)
            {
                _health = maxHealth;
            }
        }
    }

    [SerializeField]
    protected int maxHealth;
    [SerializeField]
    protected float speed;           //'목표 속력' 입니다.
    [SerializeField]
    protected float acceleration;    //'가속도의 크기'입니다.
    [SerializeField]
    public Vector2 size;           //'시야 범위'입니다.
    [SerializeField]
    protected int attack = 1;      //'공격력'입니다.
    //[SerializeField]
    //protected int rangeAttack = 0;  //'근거리 공격 범위'입니다.

    public ParticleSystem hitEffect = null;
	public Shader dissolve;
	protected StateMachine stateMachine = new StateMachine();
    protected SpriteRenderer sr;
    protected Rigidbody2D rb;

    protected bool isTouched = false;   //플레이어와의 접촉 여부를 보여주는 변수입니다.
    protected bool isInvincibe = false;

    [SerializeField]
    protected Transform shotPosition;
    [SerializeField]
    protected int shotSpeed;
    public GameObject bulletPrefab;
    [SerializeField]
    private AudioClip shotSFX, deathSFX, hitSFX;
    

    #endregion

    protected virtual void Start()
    {
        Health = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        InitEnemy();
    }

    protected virtual void Update()
    {
		stateMachine.UpdateStateMachine();
    }

    /*노말몹은 따로 isTouched 판정안하는 걸로 결정함.
    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerController pc = collision.gameObject.GetComponent<PlayerController>();
        if (pc != null && pc.IsDamagable)
            isTouched = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController pc = collision.GetComponent<PlayerController>();
        if (pc != null && pc.IsDamagable)
            isTouched = true;
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject == PlayerController.inst.gameObject)
            isTouched = false;
    }*///이후 확장성 고려


    //기본 함수들
    #region Monster Basic Functions
    protected abstract void InitEnemy();
	public virtual void GetDamaged(int damage)
	{
        if (hitSFX != null)
            SoundManager.inst.PlaySFX(gameObject, hitSFX, 1);
        if (isInvincibe)
            return;

        if (hitEffect != null)
            hitEffect.Play();
		StartCoroutine(HitEffectRoutine());
		Health -= damage;   //피해만큼 체력을 낮춥니다.
	}
    public virtual void GetDamaged(int damage, Vector3 hitPos, Vector2 velocity)
    {
        if (hitEffect != null)
        {
            hitEffect.transform.position = hitPos;
            
            var main = hitEffect.main;
            main.startSpeedMultiplier = velocity.x > 0 ? -1 : 1;
            
            hitEffect.Play();
        }
        GetDamaged(damage);
    }

	public virtual void GetDamagedToDeath()
	{
        if (hitEffect != null)
		    hitEffect.Play();
		Health = 0;
	}

	public virtual void GetHealed(int amount)
	{
		Health = Mathf.Min(maxHealth, Health + amount);
	}

	protected virtual void OnDead()
	{
        if (rb != null)
            rb.simulated = false;
        if (GetComponent<Collider2D>() != null)
            GetComponent<Collider2D>().enabled = false;
		GameManager.inst.OnEnemyKill(Type);
        if (deathSFX != null)
            SoundManager.inst.PlaySFX(gameObject, deathSFX, 0.5f);
        StartCoroutine(DissolveEffectRoutine(1));
        //Destroy(gameObject);            //이 오브젝트를 파괴합니다.
	}

	protected IEnumerator DissolveEffectRoutine(float time)
	{
		Material mat = new Material(dissolve);
		GetComponent<SpriteRenderer>().material = mat;
		Texture2D noise = new Texture2D(100, 100);

		float scale = Random.Range(20, 50);
		for (int i = 0; i < noise.width; ++i)
		{
			for (int j = 0; j < noise.height; ++j)
			{
				float noiseVal = Mathf.PerlinNoise(scale * i / noise.width, scale * j / noise.height);
				noise.SetPixel(i, j, new Color(noiseVal, noiseVal, noiseVal, 1));
			}
		}
		noise.Apply();
		mat.SetTexture("_NoiseTex", noise);

		for (float t = 0; t < time; t += Time.deltaTime)
		{
			//print(t / time);
			mat.SetFloat("_Threshold", t / time);
			yield return null;
		}
		mat.SetFloat("_Threshold", 1);
        Destroy(gameObject);
	}

	protected IEnumerator HitEffectRoutine()
	{
        sr.color = new Color(1, 0.75f, 0);
		yield return null;
        sr.color = Color.white;
	}

    protected virtual void OnCollisionEnter2D(Collision2D collision)//노말몹과 플레이어 충돌판정함수
    {
        PlayerController pc = collision.gameObject.GetComponent<PlayerController>();
        if (pc != null && pc.IsDamagable)
        {
            pc?.GetDamaged();
        }
    }

    private void OnBecameVisible()
    {
        GameManager.inst.EnemyInCamera?.Invoke();
    }

    private void OnBecameInvisible()
    {
        GameManager.inst.EnemyOutCamera?.Invoke();
    }

    #endregion


    #region Monster AI Functions

    protected virtual void FollowPlayer()   //플레이어를 따라 움직이는 함수
	{
        if (DetectPlayer(size))
        {
            SeePlayer();    //플레이어가 시야 내에 있다면 플레이어를 보도록 합니다.
            Moving();       //움직입니다.
        }
        else//플레이어가 시야를 벗어난다면
        {
            stateMachine.Transition("idle");//보통 상태로 전환합니다
        }
    }

    protected virtual void Moving()         //몹이 자신이 보는 방향으로 움직이는 함수
    {
		if (acceleration == 0)      //가속이 없으면,
			rb.velocity = speed * Direction + new Vector2(0, rb.velocity.y); //지정 속력 대로만 움직입니다.
		else
		{
			if (rb.velocity.magnitude < speed || rb.velocity.normalized != Direction) //목표 속력보다 현재 속력이 작을때 또는 현재 속도의 방향과 자신의 방향이 다를때
				rb.AddForce(acceleration * Direction); //가속도만큼 속도에 더합니다.
		}

        //transform.position += rb.velocity * unit * Time.deltaTime;
        
    }

	protected void Flying()
	{
		Vector2 direction = PlayerController.inst.transform.position - transform.position;
		direction = direction.normalized;

		rb.velocity = speed * direction;
	}

    protected void MonitorAndTransition()
    {
        MonitorAndTransition("move");
    }
    protected virtual void MonitorAndTransition(string nextState = "move")
    {
        if(DetectPlayer(size))
        {
            stateMachine.Transition(nextState);
        }
    }

    protected void AttackMelee(float range)    //근거리 공격
	{
        if (DistanceWithPlayer() <= range && PlayerController.inst.IsDamagable)
        {
            PlayerController.inst.GetDamaged();
        }
    }

	protected void AttackProjectile()   //원거리 공격.
	{
        bulletPrefab.GetComponent<Projectile>().type = ProjectileType.ENEMY;

        GameObject bullet = Instantiate(bulletPrefab) as GameObject;
        bullet.transform.position = shotPosition.position;
        bullet.GetComponent<Rigidbody2D>().velocity = ShotDirection * shotSpeed;
        bullet.transform.rotation = Quaternion.Euler(0,0, Vector2.SignedAngle(Vector2.right, ShotDirection));

        //SoundManager.inst.PlaySFX(gameObject, shotSFX);
    }

    protected void SeePlayer()  //플레이어를 보는 함수
    {
        if (PlayerController.inst.PlayerPosition.x > transform.position.x == sr.flipX)
            Flip();
        //sr.flipX = PlayerController.inst.PlayerPosition.x < transform.position.x;
    }

    protected virtual void Flip() //방향을 바꾸는 함수
    {
        sr.flipX = !sr.flipX;
    }

    #endregion

    #region Usual Data Functions
    //자료 확인용 함수입니다.
    protected bool DetectPlayer(Vector2 size)  //플레이어가 시야 범위 내에 있는지를 반환하는 함수입니다.
    {
        bool ret = false;
        Collider2D find = Physics2D.OverlapBox(transform.position, size, 0, 1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Player Grace"));
        //(DistanceWithPlayer() <= range * unit);
        if (find != null)
                ret = true;

        return ret;
    }

    protected float DistanceWithPlayer()    //플레이어와의 거리를 반환합니다.
    {
        return Vector2.Distance(PlayerController.inst.PlayerPosition, transform.position);
    }

    protected Vector3 DirectionToPlayer()   //플레이어를 바라보는 방향의 표준 벡터를 반환하는 함수입니다.(x축 한정으로만 작동)
    {
        Vector3 ret = Vector3.zero;

        if (PlayerController.inst.PlayerPosition.x < transform.position.x) //x 좌표가 더 크면(더 오른쪽에 있으면)
            ret = Vector3.left;
        else
            ret = Vector3.right;

        return ret;
    }

    protected Vector2 ShotDirection
    {
        get {
            Vector2 difference = PlayerController.inst.transform.position - shotPosition.position;
            Vector2 direction = difference.normalized;
            
            return direction;
        }
    }

	public static string TypeToName(EnemyType type)
	{
		switch (type)
		{
			case EnemyType.ALL:
				return "모든 적들";
			case EnemyType.BAT:
				return "박쥐";
			case EnemyType.DINGO:
				return "들개";
			case EnemyType.GHOST:
				return "유령";
			case EnemyType.SHIELDER:
				return "방패병";
            case EnemyType.SHIELD:
                return "방패";
			case EnemyType.SKELETON:
				return "해골";
			case EnemyType.SKELETONARCHER:
				return "해골 궁수";
			case EnemyType.SLIME:
				return "슬라임";
			case EnemyType.STABEAST:
				return "이건뭐지";
			case EnemyType.ZOMBIE:
				return "좀비";
			case EnemyType.BOSS_CLOCK:
				return "시계 보스";
			default:
				return "정보없는 몬스터";
		}
	}

    #endregion

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, size);
    }
#endif

}