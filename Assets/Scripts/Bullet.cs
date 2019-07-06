using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*public class Bullet : MonoBehaviour
{
    int attack = 1;
    public bool isPlayersBullet;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy" && isPlayersBullet)
        {
            //collision.GetComponent<NormalMonster>().hitEffect.transform.position = transform.position;
            collision.GetComponent<NormalEnemy>()?.hitEffect.Play();
            collision.GetComponent<NormalEnemy>()?.GetDamaged(attack);
            Destroy(gameObject);
        }

        if (collision.tag == "Player" && !isPlayersBullet)
        {
            collision.GetComponent<PlayerController>()?.GetDamaged();
            Destroy(gameObject);
        }
    }
}*/