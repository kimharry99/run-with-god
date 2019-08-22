using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Butcher : Boss
{
    public override EnemyType Type { get { return EnemyType.ALL; } }
    public Transform anchor;

    public HitRange attackRange, shockwaveRange;

	[SerializeField]
	private DistanceJoint2D distanceJoint;

	private float nextPatternTimer;

    [SerializeField]
    private GameObject anchorPrefab;

	[SerializeField]
	private Tombstone[] tombstones = new Tombstone[2];

    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    protected override void Update()
	{
		base.Update();
		if (nextPatternTimer > 0)
			nextPatternTimer -= Time.deltaTime;
	}

	protected override void InitEnemy()
    {

        State move = new State();
		State attack = new State();
        State pattern1 = new State();
        State pattern2 = new State();
        State pattern3 = new State();
        State pattern4 = new State();
        State pattern5 = new State();

		State dead = new State();

		move.Enter += delegate { nextPatternTimer = Random.Range(3f, 5f); anim.SetBool("isWalking", true); };

		move.StateUpdate += delegate
		{
			SeePlayer();
			Moving();

			BoxCollider2D col = GetComponent<BoxCollider2D>();
			if (Physics2D.OverlapBox(col.bounds.center, col.bounds.size, 0, 1 << LayerMask.NameToLayer("Player")) != null)
			{
				stateMachine.Transition("attack");
			}
			else if (nextPatternTimer <= 0)
			{
				int rand = Random.Range(2, 6);
				stateMachine.Transition("pattern" + rand.ToString());
			}
		};

		move.Exit += delegate { rb.velocity = Vector2.zero; anim.SetBool("isWalking", false); };

		attack.Enter += delegate { StartCoroutine(AttackRoutine()); };

		pattern1.Enter += delegate { StartCoroutine(ThrowAnchorRoutine());  };
		pattern2.Enter += delegate { stateMachine.Transition("move"); };
        pattern3.Enter += delegate { StartCoroutine(AnchorAttackRoutine()); };
		pattern4.Enter += delegate { StartCoroutine(RushRoutine()); };
		pattern5.Enter += delegate { StartCoroutine(SummonTombstonesRoutine()); };

		stateMachine.AddNewState("move", move);
		stateMachine.AddNewState("attack", attack);
		stateMachine.AddNewState("pattern1", pattern1);
		stateMachine.AddNewState("pattern2", pattern2);
		stateMachine.AddNewState("pattern3", pattern3);
		stateMachine.AddNewState("pattern4", pattern4);
		stateMachine.AddNewState("pattern5", pattern5);


		stateMachine.AddNewState("dead", dead);

		stateMachine.Transition("move");
	}
	public override void GetDamagedToDeath()
	{
		
	}

	public override void GetHealed(int amount)
	{
		base.GetHealed(amount);
		InGameUIManager.inst.UpdateBossHelthUI((float)Health / maxHealth);
	}

	private IEnumerator AttackRoutine()
	{
        yield return attackRange.Activate(0.3f, true);
        anim.SetTrigger("Attack2");
        yield return new WaitForSeconds(0.2f);
		stateMachine.Transition("move");
	}

	private IEnumerator ThrowAnchorRoutine()
    {
       
        Vector3 playerPos = PlayerController.inst.transform.position;
		Vector3 oriLocalPos = anchor.localPosition;
		Vector3 oriPos = anchor.position;

		Vector2 direction = (playerPos - oriPos).normalized;

		anchor.rotation = Quaternion.Euler(0,0,-Vector2.Angle(Vector2.up, direction));
		Collider2D col = anchor.GetComponent<Collider2D>();

		anchor.gameObject.SetActive(true);

        anim.SetTrigger("Throw");

        while (!col.IsTouchingLayers(1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Ground")))
		{
			anchor.position += new Vector3(direction.x, direction.y, 0) * 20 * Time.deltaTime;
			yield return null;
		}

		if (col.IsTouchingLayers(1 << LayerMask.NameToLayer("Player")))
		{
			distanceJoint.connectedBody = PlayerController.inst.GetComponent<Rigidbody2D>();
			distanceJoint.enabled = true;
			distanceJoint.distance = DistanceWithPlayer();
			while (distanceJoint.distance > 0.01f)
			{
				distanceJoint.distance -= Time.deltaTime * 10;
				anchor.transform.position = PlayerController.inst.transform.position;
				yield return null;
			}
            
            Debug.Log("Player Hit");
		}
		else
		{
			distanceJoint.connectedAnchor = new Vector2(anchor.position.x, transform.position.y + 0.01f);
			distanceJoint.enabled = true;
			float oriDistance = distanceJoint.distance = Vector2.Distance(transform.position, anchor.position);

			while (distanceJoint.distance > 0.01f)
			{
				distanceJoint.distance -= Time.deltaTime * 10;
				yield return null;
			}
		}
		distanceJoint.connectedBody = null;
		distanceJoint.enabled = false;
		anchor.gameObject.SetActive(false);
		anchor.localPosition = oriLocalPos;
		stateMachine.Transition("move");
	}

    private IEnumerator AnchorAttackRoutine()
    {

        

        Vector3 playerPos = PlayerController.inst.transform.position;
        Vector3 oriLocalPos = anchor.localPosition;
        Vector3 oriPos = anchor.position;

        Vector2 direction = (playerPos - oriPos).normalized;
        float angle = -Vector2.Angle(Vector2.up, direction);

        anim.SetTrigger("Throw");

        GameObject tmp = Instantiate(anchorPrefab, transform.position, Quaternion.Euler(0,0,angle));
        tmp.GetComponent<Rigidbody2D>().velocity = direction * 5;
        tmp = Instantiate(anchorPrefab, transform.position, Quaternion.Euler(0, 0, angle + 15));
        tmp.GetComponent<Rigidbody2D>().velocity = Quaternion.Euler(0, 0, 15) * direction * 5;
        tmp = Instantiate(anchorPrefab, transform.position, Quaternion.Euler(0, 0, angle - 15));
        tmp.GetComponent<Rigidbody2D>().velocity = Quaternion.Euler(0, 0, -15) * direction * 5;

		yield return new WaitForSeconds(1);
		stateMachine.Transition("move");
    }

	private IEnumerator RushRoutine()
	{
		yield return new WaitForSeconds(2f);
		gameObject.layer = LayerMask.NameToLayer("Enemy");
		BoxCollider2D col = GetComponent<BoxCollider2D>();
		while (!col.IsTouchingLayers(1 << LayerMask.NameToLayer("Wall")))
		{
			rb.AddForce(40 * Direction);
			yield return null;
		}
		gameObject.layer = LayerMask.NameToLayer("Enemy Passable");
		CameraController.Shake(0.1f, 0.5f);
		yield return new WaitForSeconds(2f);
		stateMachine.Transition("move");
	}

	private IEnumerator SummonTombstonesRoutine()
	{

        anim.SetTrigger("Attack1");

        yield return shockwaveRange.Activate(1f, true);
		CameraController.Shake(0.2f, 0.5f);
		yield return new WaitForSeconds(2f);

        tombstones[0].Summon();
		tombstones[1].Summon();
		stateMachine.Transition("move");
	}

	protected override void OnDead()
	{
		base.OnDead();
		stateMachine.Transition("dead");
		GameManager.inst.GameClear();
	}
}
