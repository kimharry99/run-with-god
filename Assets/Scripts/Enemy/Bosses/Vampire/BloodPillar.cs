using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodPillar : MonoBehaviour
{
    [SerializeField]
    private HitRange hitRange;
    private SpriteRenderer sr;
    private Collider2D col;
    private Rigidbody2D rb;

    private Vector3 velocity;

    private bool isActivated = false;
    private float lifeTimer;



    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        sr.enabled = col.enabled = false;
        
    }

    private void Update()
    {
        lifeTimer -= Time.deltaTime;
        if (lifeTimer <= 0)
        {
            Destroy(gameObject);
        }

        if (isActivated)
            transform.position += velocity * Time.deltaTime;
    }

    public void Initialize(Vector2 velocity, float lifeTime)
    {
        StartCoroutine(InitializeRoutine());
        this.velocity = velocity;
        lifeTimer = lifeTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.GetComponent<PlayerController>().GetDamaged();
        }
    }

    private IEnumerator InitializeRoutine()
    {
        yield return hitRange.Activate(2);
        CameraController.Shake(0.2f, 0.5f);
        sr.enabled = col.enabled = isActivated = true;
    }
}
