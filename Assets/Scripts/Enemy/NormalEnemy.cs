using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NormalEnemy : MonoBehaviour
{
    float unit = 1f;//0.1f;          //거리의 임시 단위입니다. 플레이어의 가로가 확정되면 그 값을 배정합니다. 굳이 여기에 있을 필요는 없습니다.
    
    //변수들
    #region Monster Variables 
    private int jumpCount = 1;

    /*private Vector3 _direction = Vector3.left;  //몹이 바라보고 있는 방향입니다.
    protected Vector3 direction {
        get
        {
            return _direction;
        }
        set
        {
            _direction = value;
            if ((_direction.normalized == Vector3.left && !sr.flipX) ||       
                (_direction.normalized == Vector3.right && sr.flipX))
            {
                Flip();
            }
        }
    }*/
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
        }
    }

    [SerializeField]
    protected int maxHealth;
    [SerializeField]
    protected int speed;           //'목표 속력' 입니다.
    [SerializeField]
    protected int acceleration;    //'가속도의 크기'입니다.
    [SerializeField]
    protected int range;           //'시야 범위'입니다.
    [SerializeField]
    protected int attack = 1;      //'공격력'입니다.
    //[SerializeField]
    //protected int rangeAttack = 0;  //'근거리 공격 범위'입니다.

    public ParticleSystem hitEffect = null;
	public Shader dissolve;
	protected StateMachine stateMachine = new StateMachine();
    protected SpriteRenderer sr;
    private Rigidbody2D rb;

    protected bool isTouched = false;   //플레이어와의 접촉 여부를 보여주는 변수입니다.
    private Transform landChecker;

    private Transform shotPosition;
    public GameObject bulletPrefab;
    public AudioClip shotSFX;
    //protected int jumpSpeed = 0;

    /*private bool IsGround
    {
        get { return Physics2D.OverlapPoint(landChecker.position, 1 << LayerMask.NameToLayer("Ground")) != null; }
    }*/

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
        if (hitEffect != null)
            hitEffect.Play();
        Health -= damage;   //피해만큼 체력을 낮춥니다.
	}
	public virtual void GetDamagedToDeath()
	{
        if (hitEffect != null)
		    hitEffect.Play();
		Health = 0;
	}

	protected virtual void OnDead()
	{
		GameManager.inst.KillCount++;
        Destroy(gameObject);            //이 오브젝트를 파괴합니다.
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
	}

    protected virtual void OnCollisionEnter2D(Collision2D collision)//노말몹과 플레이어 충돌판정함수
    {
        PlayerController pc = collision.gameObject.GetComponent<PlayerController>();
		if (pc != null && pc.IsDamagable)
            pc?.GetDamaged();
    }

    #endregion


    #region Monster AI Functions

    protected void FollowPlayer()   //플레이어를 따라 움직이는 함수
	{
        if (DetectPlayer(range))
        {
            SeePlayer();    //플레이어가 시야 내에 있다면 플레이어를 보도록 합니다.
            Moving();       //움직입니다.
            if (!DetectPlayer(range))//플레이어가 시야를 벗어난다면
            {
                stateMachine.Transtion("idle");//보통 상태로 전환합니다
            }
        }
	}

    protected void Moving()         //몹이 자신이 보는 방향으로 움직이는 함수
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

    protected void Idle()           //감시하는 함수
	{
        if (DetectPlayer(range))
        {
            stateMachine.Transtion("move");
        }
    }

    /*protected void AttackTouch()    //접촉 공격
    {
        if(isTouched && PlayerController.inst.IsDamagable)
        {
            PlayerController.inst.GetDamaged();
        }
    }

    protected void AttackMelee()    //근거리 공격
	{
        //if (DistanceWithPlayer() <= rangeAttack && PlayerController.inst.IsDamagable)
        //{
        //    PlayerController.inst.GetDamaged();
        //}
    }*/

	protected void AttackProjectile()   //원거리 공격. 총알이 없어서 제대로 구현되지 않았습니다.
	{
        GameObject bullet = Instantiate(bulletPrefab);
        bullet.transform.position = shotPosition.position + new Vector3(0, Random.Range(-0.05f, 0.05f));
        bullet.GetComponent<Rigidbody2D>().velocity = ShotDirection() * 1f;
        //CameraController.Shake(0.02f, 0.05f);
        SoundManager.inst.PlaySFX(gameObject, shotSFX);
        //Long range attack
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

    /*protected void Jump() //보스 전용 임시 점프 함수
    {
        if (jumpCount > 0)
        {
            rb.velocity += new Vector2(0, jumpSpeed - rb.velocity.y);

            jumpCount--;
        }
    }*/

    #endregion

    #region Usual Data Functions
    //자료 확인용 함수입니다.
    protected bool DetectPlayer(int range)  //플레이어가 시야 범위 내에 있는지를 반환하는 함수입니다.
    {
        bool ret = (DistanceWithPlayer() <= range * unit);

        return ret;
    }

    protected float DistanceWithPlayer()    //플레이어와의 거리를 반환합니다.
    {
        return Vector3.Distance(PlayerController.inst.PlayerPosition, transform.position);
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

    protected Vector2 ShotDirection()
    {
        Vector2 ret = Vector2.zero;
        return ret;
    }
    #endregion
}