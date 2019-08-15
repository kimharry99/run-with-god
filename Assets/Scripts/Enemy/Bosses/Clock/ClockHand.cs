using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockHand : MonoBehaviour
{
    [SerializeField]
    private Transform shotOrigin;
    private float shotTimer;
    [SerializeField]
    private GameObject projectilePrefab;

    private bool isShot = false;
    private bool isRotate = false;

    [SerializeField]
    private float rotateSpeed;

    private void Update()
    {
        if (isShot)
        {
            if (shotTimer > 0)
                shotTimer -= Time.deltaTime;
            if (shotTimer <= 0)
            {
                ShotProjectile();
            }
        }
        if (isRotate)
        {
            transform.Rotate(Vector3.back, rotateSpeed * ClockBoss.timeSpeed * Time.deltaTime);
        }
    }

    private void ShotProjectile()
    {
        float angle = Mathf.Deg2Rad * (transform.rotation.eulerAngles.z + 90);
        GameObject projectile = Instantiate(projectilePrefab, shotOrigin.position, transform.rotation);
        projectile.GetComponent<Rigidbody2D>().velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;
        shotTimer = 0.3f;
    }

    public void EnableHand()
    {
        isRotate = true;
        isShot = true;
    }

    public void DisableHand()
    {
        isRotate = false;
        isShot = false;
    }

    public void Shake()
    {
        StartCoroutine(ShakeHandRoutine());
    }

    private IEnumerator ShakeHandRoutine()
    {
        Vector3 randPos = Random.insideUnitCircle * 2;
        Vector3 destination = PlayerController.inst.transform.position + randPos;
        Vector3 oriPos = transform.position;

        for (float t = 0; t < 2f; t += Time.deltaTime)
        {
            transform.position = Vector3.Lerp(oriPos, destination, 1 - (t / 2 - 1) * (t / 2 - 1));
            yield return null;
        }

        Quaternion oriRot = transform.rotation;
        for (float t = 0; t <= 1f; t += Time.deltaTime)
        {
            transform.rotation = Quaternion.Euler(0, 0, oriRot.eulerAngles.z + 360 * t);
            yield return null;
        }
        transform.rotation = oriRot;

        for (float t = 0; t < 2f; t += Time.deltaTime)
        {
            transform.position = Vector3.Lerp(destination, oriPos, 1 - (t / 2 - 1) * (t / 2 - 1));
            yield return null;
        }
    }

    public void MoveToOrigin(Transform origin)
    {
        StopAllCoroutines();
        StartCoroutine(MoveToOriginRoutine(origin));
    }

    private IEnumerator MoveToOriginRoutine(Transform origin)
    {
        Vector3 oriPos = transform.position;
        Quaternion oriRot = transform.rotation;
        for (float t = 0; t <= 2.5f; t += Time.deltaTime)
        {
            transform.position = Vector3.Lerp(oriPos, origin.position, t / 2.5f);
            transform.rotation = Quaternion.Lerp(oriRot, origin.rotation, t / 2.5f);
            yield return null;
        }
        EnableHand();
    }
}
