using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Butcher : NormalEnemy
{
    public override EnemyType Type { get { return EnemyType.ALL; } }
    public Transform anchor;

    public Collider2D hitRange;
	private Collider2D player;

	[SerializeField]
	private DistanceJoint2D distanceJoint;

	private float nextPatternTimer;

    [SerializeField]
    private GameObject anchorPrefab;

	protected override void Update()
	{
		base.Update();
		if (nextPatternTimer > 0)
			nextPatternTimer -= Time.deltaTime;
	}

	protected override void InitEnemy()
    {
		player = PlayerController.inst.GetComponent<Collider2D>();

        State move = new State();
        State attack = new State();
        State pattern1 = new State();
        State pattern2 = new State();
        State pattern3 = new State();
        State pattern4 = new State();
        State pattern5 = new State();

        //move.StateUpdate += FollowPlayer;
		move.StateUpdate += delegate
		{
			FollowPlayer();

			BoxCollider2D col = GetComponent<BoxCollider2D>();
			if (Physics2D.OverlapBox(col.bounds.center, col.bounds.size, 0, 1 << LayerMask.NameToLayer("Player")) != null)
			{
				stateMachine.Transition("attack");
			}
			else if (nextPatternTimer <= 0)
			{
				stateMachine.Transition("pattern3");
			}
		};

		attack.Enter += delegate { StartCoroutine(AttackRoutine()); };

		pattern1.Enter += delegate { StartCoroutine(ThrowAnchorRoutine()); };

        pattern3.Enter += delegate { StartCoroutine(AnchorAttackRoutine()); Debug.Log("p3"); };

		stateMachine.AddNewState("move", move);
		stateMachine.AddNewState("attack", attack);
		stateMachine.AddNewState("pattern1", pattern1);
		stateMachine.AddNewState("pattern2", pattern2);
		stateMachine.AddNewState("pattern3", pattern3);
		stateMachine.AddNewState("pattern4", pattern4);
		stateMachine.AddNewState("pattern5", pattern5);

		stateMachine.Transition("move");

		nextPatternTimer = Random.Range(5f, 10f);
	}

	private IEnumerator AttackRoutine()
    {
		SpriteRenderer sr = hitRange.gameObject.GetComponent<SpriteRenderer>();

		for (int i = 0; i < 20; i++)
		{
			sr.enabled = !sr.enabled;
			yield return new WaitForSeconds(0.025f);
		}
		
        if (hitRange.IsTouchingLayers(1 << LayerMask.NameToLayer("Player")))
        {
            PlayerController.inst.GetDamaged();
        }
		yield return new WaitForSeconds(0.2f);
		stateMachine.Transition("move");
	}

    protected override void Flip()
    {
        base.Flip();
        foreach(Transform child in transform)
        {
            child.localPosition = new Vector3(-child.localPosition.x, child.localPosition.y, child.localPosition.z);
        }
    }

	public override void GetDamaged(int damage)
	{
		base.GetDamaged(damage);
		InGameUIManager.inst.UpdateBossHelthUI((float)Health / maxHealth);
	}

	public override void GetDamagedToDeath()
	{
		
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


			/*
			for (float t = 0; t < oridi; t += Time.deltaTime)
			{
				distanceJoint.distance = Mathf.Lerp(oriDistance, 0, t);
				yield return null;
			}
			*/	

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
		nextPatternTimer = Random.Range(5f,10f);
	}

    private IEnumerator AnchorAttackRoutine()
    {
        Vector3 playerPos = PlayerController.inst.transform.position;
        Vector3 oriLocalPos = anchor.localPosition;
        Vector3 oriPos = anchor.position;

        Vector2 direction = (playerPos - oriPos).normalized;
        float angle = -Vector2.Angle(Vector2.up, direction);

        GameObject tmp = Instantiate(anchorPrefab, transform.position, Quaternion.Euler(0,0,angle));
        tmp.GetComponent<Rigidbody2D>().velocity = direction * 5;
        tmp = Instantiate(anchorPrefab, transform.position, Quaternion.Euler(0, 0, angle + 15));
        tmp.GetComponent<Rigidbody2D>().velocity = Quaternion.Euler(0, 0, 15) * direction * 5;
        tmp = Instantiate(anchorPrefab, transform.position, Quaternion.Euler(0, 0, angle - 15));
        tmp.GetComponent<Rigidbody2D>().velocity = Quaternion.Euler(0, 0, -15) * direction * 5;
        stateMachine.Transition("move");
        nextPatternTimer = Random.Range(5f, 10f);
        yield return null;
    }
}
