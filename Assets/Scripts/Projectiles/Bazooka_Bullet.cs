using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bazooka_Bullet : Projectile
{
    public float explodeRange;
    public AudioClip boomSFX = null;
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        switch (type)
        {
            case ProjectileType.PLAYER:
                if (collision.tag.Contains("Enemy"))
                    collision.GetComponent<NormalEnemy>()?.GetDamaged(attack, transform.position, GetComponent<Rigidbody2D>().velocity);
                if (collision.tag != "Player" && collision.tag != "Projectile" && !collision.isTrigger)
                {
                    StopAllCoroutines();
                    Explode();
                    Destroy(gameObject);
                }
                break;
        }
    }

    protected override IEnumerator DestroyRoutine(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
        Explode();
    }

    private void Explode()
    {
        CameraController.Shake(0.1f, 0.5f);
        CameraController.ShockWave(transform.position);
        SoundManager.inst.PlaySFX(gameObject, boomSFX);

        foreach (var enemy in Physics2D.OverlapCircleAll(transform.position, explodeRange, 1 << LayerMask.NameToLayer("Enemy") | 1 << LayerMask.NameToLayer("Enemy Ghost")))
        {
            enemy.GetComponent<NormalEnemy>()?.GetDamaged(1);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, explodeRange);
    }
}
