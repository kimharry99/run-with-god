using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VampireBoss : NormalEnemy
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

    [SerializeField]
    private GameObject alterPrefab;
    [SerializeField]
    private GameObject bloodPillarPrefab;

    private Vector3 RandomInsideMap { get { return new Vector2(Random.Range(mapBounds.min.x / 2, mapBounds.max.x / 2), Random.Range(mapBounds.min.y / 2, mapBounds.max.y / 2)); } }

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
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            StartCoroutine(BloodLaserRoutine());
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            StartCoroutine(TeleportRoutine(BloodLaserRoutine()));
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            StartCoroutine(MakeAlterRoutine());
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            StartCoroutine(MakeBloodPillarRoutine());
        }
    }

    protected override void InitEnemy()
    {

    }

    public override void GetDamagedToDeath()
    {
        
    }

    private IEnumerator MakeAlterRoutine()
    {
        yield return new WaitForSeconds(1);
        for (int i = 0; i < 4; i++)
        {
            StartCoroutine(MoveTo(Instantiate(alterPrefab, transform.position, transform.rotation).transform, RandomInsideMap, 3));
        }
    }

    private IEnumerator MoveTo(Transform transform, Vector3 to, float time)
    {
        Vector3 oriPos = transform.position;
        for (float t = 0; t < time; t += Time.deltaTime)
        {
            transform.position = Vector3.Lerp(oriPos, to, moveCurve.Evaluate(t / time));
            yield return null;
        }
        
    }

    private IEnumerator TeleportRoutine(IEnumerator nextRoutine)
    {
        col.enabled = false;
        const float teleportTimer = 0.2f;

        Color color = sr.color;
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
        const float laserTime = 1;
        bloodLaser.enabled = true;
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
                bloodLaser.SetPosition(1, hit.point - new Vector2(transform.position.x, transform.position.y));
                bloodEffect.transform.position = hit.point;
                bloodEffect.Play();
            }
            yield return new WaitForFixedUpdate();
        }
        bloodLaser.enabled = false;
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
    }
}
