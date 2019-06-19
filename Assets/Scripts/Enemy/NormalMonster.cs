using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NormalMonster : MonoBehaviour
{
    float unit = 1f;//0.1f;          //거리의 임시 단위입니다. 플레이어의 가로가 확정되면 그 값을 배정합니다. 굳이 여기에 있을 필요는 없습니다.

    //protected Vector3 velocity;   //'현재 속도'입니다. 가속도가 없으면 _speed = speed입니다.
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
    private Vector2 Direction
    {
        get { return sr.flipX ? Vector2.left : Vector2.right; }
        //set { if (Direction != value.normalized) Flip(); }
    }

    [SerializeField]
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
    protected int speed;           //'목표 속력' 입니다.
    [SerializeField]
    protected int acceleration;    //'가속도의 크기'입니다.
    [SerializeField]
    protected int range;           //'시야 범위'입니다.
    [SerializeField]
    protected int attack = 1;      //'공격력'입니다.
    [SerializeField]
    protected int jumpSpeed = 0;

    public ParticleSystem hitEffect;
	protected StateMachine stateMachine = new StateMachine();
    private SpriteRenderer sr;
    private Rigidbody2D rb;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
		InitMonster();
    }

    protected virtual void Update()
    {
		stateMachine.UpdateStateMachine();
    }

	#region Monster Basic Functions
	protected abstract void InitMonster();
	public virtual void GetDamaged(int damage)
	{
        Health -= damage;   //피해만큼 체력을 낮춥니다.
	}
	protected virtual void OnDead()
	{
        Destroy(gameObject);            //이 오브젝트를 파괴합니다.
	}

    #endregion

    #region Monster AI Functions

	protected void FollowPlayer()   //플레이어를 따라 움직이는 함수입니다.
	{
        SeePlayer();    //항상 플레이어를 보도록 합니다.
        Moving();       //움직입니다.
	}

    protected void Moving()         //보는 방향으로 움직이는 함수입니다.
    {
        if (acceleration == 0)      //가속이 없으면,
            rb.velocity = speed * Direction; //지정 속력 대로만 움직입니다.
        else
        {
            if (rb.velocity.magnitude < speed || rb.velocity.normalized != Direction) //목표 속력보다 현재 속력이 작을때 또는 현재 속도의 방향과 자신의 방향이 다를때
                rb.AddForce(acceleration * Direction); //가속도만큼 속도에 더합니다.
        }

        //transform.position += rb.velocity * unit * Time.deltaTime;
    }

    protected void Idle()           
	{
        SeePlayer();    //항상 플레이어를 보도록 합니다.
        if (DetectPlayer(range))
        {
            stateMachine.Transtion("move");
        }
    }

    private void OnTrigger(Collision2D collision)
    {
        


    }

    /*protected void Jump()
    {
        if (jumpCount > 0)
        {
            rb.velocity += new Vector2(0, jumpSpeed - rb.velocity.y);

            jumpCount--;
        }
    }*/

    protected void AttackMelee()
	{


        int range = 1;

        bool ret = (DistanceWithPlayer() <= range * unit);

        //PlayerController.inst.Life--;  //참조 방법 필요
    }

	protected void AttackProjectile()
	{
		//Long range attack
	}

    protected void SeePlayer()  //플레이어를 보는 함수입니다.
    {
        //direction = DirectionToPlayer();
        //Vector3 ret = Vector3.zero;
        sr.flipX = GetPlayer().transform.position.x < this.transform.position.x;
    }

    private void Flip()
    {
        sr.flipX = !sr.flipX;
    }

    #endregion

    #region Usual Variables Terms

    protected GameObject GetPlayer()
    {
        return GameObject.FindGameObjectWithTag("Player");
    }

    protected bool DetectPlayer(int range)  //플레이어가 시야 범위 내에 있는지를 반환하는 함수입니다.
    {
        bool ret = (DistanceWithPlayer() <= range * unit);

        return ret;
    }

    protected float DistanceWithPlayer()    //플레이어와의 거리를 반환합니다.
    {
        return Vector3.Distance(GetPlayer().transform.position, transform.position);
    }

    protected Vector3 DirectionToPlayer()   //플레이어를 바라보는 방향의 표준 벡터를 반환하는 함수입니다.(x축 한정으로만 작동)
    {
        Vector3 ret = Vector3.zero;

        if (GetPlayer().transform.position.x < this.transform.position.x) //x 좌표가 더 크면(더 오른쪽에 있으면)
            ret = Vector3.left;
        else
            ret = Vector3.right;

        return ret;
    }
	#endregion
}