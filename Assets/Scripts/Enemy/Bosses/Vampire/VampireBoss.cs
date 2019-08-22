using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VampireBoss : Boss
{
    public Bounds mapBounds;

    public AnimationCurve moveCurve;

    [SerializeField]
    private LayerMask bloodLaserMask;
    public LineRenderer bloodLaser;
    public AnimationCurve laserCurve;

    [SerializeField]
    private ParticleSystem bloodEffect;
    
    private Collider2D col;

	private float nextPatternTimer;

    [SerializeField]
    private GameObject alterPrefab;
    [SerializeField]
    private GameObject bloodPillarPrefab;
	[SerializeField]
	private GameObject bloodProjectilePrefab;

    private Vector3 RandomInsideMap { get { return new Vector2(Random.Range(mapBounds.min.x / 2, mapBounds.max.x / 2), Random.Range(mapBounds.min.y / 2, mapBounds.max.y / 2)); } }

	private int phase = 1;

	private Vector3 destination;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(mapBounds.center, mapBounds.extents);
    }

#endif
    public override EnemyType Type { get { return EnemyType.ALL; } }

    protected override void Start()
    {
        base.Start();
        col = GetComponent<Collider2D>();
    }

    protected override void Update()
    {
        base.Update();
		nextPatternTimer -= Time.deltaTime;
    }

    protected override void InitEnemy()
    {
		State phase1 = new State();
		State phase2 = new State();
		State pattern1 = new State();
		State pattern2 = new State();
		State pattern3 = new State();
		State pattern4 = new State();

		State p1to2 = new State();

		State dead = new State();

		phase1.Enter += delegate { nextPatternTimer = 6; };
		phase1.StateUpdate += delegate {
			if (nextPatternTimer <= 0)
			{
				stateMachine.Transition("pattern" + Random.Range(1, 5).ToString());
			}
			if (Health <= maxHealth / 2)
			{
				stateMachine.Transition("p1to2");
			}
			transform.position += (destination - transform.position).normalized * Time.deltaTime;
			if (Vector3.Distance(transform.position, destination) < 0.01f)
				destination = RandomInsideMap;
		};
		phase2.Enter += delegate { nextPatternTimer = 3; };
		phase2.StateUpdate += delegate {
			if (nextPatternTimer <= 0)
			{
				stateMachine.Transition("pattern" + Random.Range(1, 5).ToString());
			}
		};

		pattern1.Enter += delegate { StartCoroutine(MakeAlterRoutine()); };
		pattern2.Enter += delegate { StartCoroutine(TeleportRoutine(MakeBloodPillarRoutine())); };
		pattern3.Enter += delegate { StartCoroutine(TeleportRoutine(MakeBloodProjectileRoutine())); };
		pattern4.Enter += delegate { StartCoroutine(TeleportRoutine(BloodLaserRoutine())); };

		p1to2.Enter += delegate { StartCoroutine(Phase1To2Routine()); };

		stateMachine.AddNewState("phase1", phase1);
		stateMachine.AddNewState("phase2", phase2);
		stateMachine.AddNewState("pattern1", pattern1);
		stateMachine.AddNewState("pattern2", pattern2);
		stateMachine.AddNewState("pattern3", pattern3);
		stateMachine.AddNewState("pattern4", pattern4);

		stateMachine.AddNewState("p1to2", p1to2);

		stateMachine.AddNewState("dead", dead);

		stateMachine.Transition("phase1");
	}

	public override void GetDamagedToDeath()
    {
        
    }

    private IEnumerator MakeAlterRoutine()
    {
        yield return new WaitForSeconds(1);
        for (int i = 0; i < 2; i++)
        {
			GameObject alter = Instantiate(alterPrefab, transform.position, transform.rotation);
			alter.GetComponent<VampireAlter>().vampire = this;
			StartCoroutine(MoveTo(alter.transform, RandomInsideMap, 3));
        }
		stateMachine.Transition("phase" + phase.ToString());
	}

    private IEnumerator MoveTo(Transform transform, Vector3 to, float time)
    {
        Vector3 oriPos = transform.position;
        for (float t = 0; t < time; t += Time.deltaTime)
        {
			try
			{
				transform.position = Vector3.Lerp(oriPos, to, moveCurve.Evaluate(t / time));
			}
			catch
			{
				yield break;
			}
			yield return null;
		}
    }

    private IEnumerator TeleportRoutine(IEnumerator nextRoutine)
    {
        col.enabled = false;
        const float teleportTimer = 0.2f;

        Color color = baseColor;
        for (float t= 0; t<teleportTimer; t += Time.deltaTime)
        {
            sr.color = new Color(color.r, color.g, color.b, Mathf.Lerp(1, 0, t / teleportTimer));
            yield return null;
        }

        transform.position = new Vector2(Random.Range(mapBounds.min.x / 2, mapBounds.max.x / 2), Random.Range(mapBounds.min.y / 2, mapBounds.max.y / 2));
        for (float t = 0; t < teleportTimer; t += Time.deltaTime)
        {
            sr.color = new Color(color.r, color.g, color.b, Mathf.Lerp(0, 1, t / teleportTimer));
            yield return null;
        }

        col.enabled = true;

        if (nextRoutine != null)
            StartCoroutine(nextRoutine);
    }

    private IEnumerator BloodLaserRoutine()
    {
		const float laserTime = 5;
		yield return new WaitForSeconds(2);
		bloodLaser.enabled = true;
		bloodLaser.SetPosition(0, transform.position);
		for (float t = 0; t < laserTime; t += Time.deltaTime)
        {
            float rotationZ = 360 * laserCurve.Evaluate(t / laserTime);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Quaternion.Euler(0, 0, rotationZ) * Vector2.up, 50, bloodLaserMask);
            if (hit.collider != null)
            {
                PlayerController pc = hit.collider.GetComponent<PlayerController>();
                if (pc != null && pc.IsDamagable)
                {
                    hit.collider.GetComponent<PlayerController>().GetDamaged();
                }
                bloodLaser.SetPosition(1, hit.point);
                bloodEffect.transform.position = hit.point;
                bloodEffect.Play();
            }
            yield return new WaitForFixedUpdate();
        }
        bloodLaser.enabled = false;
		stateMachine.Transition("phase" + phase.ToString());
	}

    private IEnumerator MakeBloodPillarRoutine()
    {
        yield return new WaitForSeconds(2);
        for (int i = 0; i < 3; i++)
        {
            Instantiate(bloodPillarPrefab, new Vector3(Random.Range(mapBounds.min.x / 2, mapBounds.max.x / 2), 0), transform.rotation)
                .GetComponent<BloodPillar>().Initialize(new Vector2(Random.Range(-1f, 1f), 0), 10);
            yield return new WaitForSeconds(Random.Range(0.2f, 0.5f));
        }
		stateMachine.Transition("phase" + phase.ToString());
    }

	private IEnumerator MakeBloodProjectileRoutine()
	{
		yield return new WaitForSeconds(1);
		for (int i = 0; i < 4; i++)
		{
			GameObject projectile = Instantiate(bloodProjectilePrefab, transform.position, transform.rotation);
			projectile.GetComponent<BloodProjectile>().SetStartValues(transform.position, 90 * i);
			Destroy(projectile, 2);
		}
		stateMachine.Transition("phase" + phase.ToString());
	}

	private IEnumerator Phase1To2Routine()
	{
		const float moveTime = 3f;
		const float changeColorTime = 1f;
		Vector3 oriPos = transform.position;
		Color oriColor = sr.color;
		col.enabled = false;
		for (float t = 0; t < moveTime; t += Time.deltaTime)
		{
			transform.position = Vector3.Lerp(oriPos, mapBounds.center, moveCurve.Evaluate(t / moveTime));
			yield return null;
		}
		for (float t = 0; t < changeColorTime; t += Time.deltaTime)
		{
			sr.color = baseColor = Color.Lerp(oriColor, Color.red, t / changeColorTime);
			yield return null;
		}
		col.enabled = true;
		phase = 2;
		stateMachine.Transition("phase" + phase.ToString());
	}

	protected override void OnDead()
	{
		base.OnDead();
		stateMachine.Transition("dead");
		GameManager.inst.GameClear();
	}
}
