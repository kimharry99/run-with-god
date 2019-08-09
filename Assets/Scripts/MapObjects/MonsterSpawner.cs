using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public GameObject monster;  //소환할 몬스터
    private bool isSummoned = false;

    private void Update()
    {
        if (ThisIsInBack)
            Spawn();
    }

    private bool ThisIsInBack
    {
        get
        {
            Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
            return pos.x < 0.0f;
        }
    }

    public void Spawn()
    {
        if (monster != null && !isSummoned)
        {
            Instantiate(monster, transform.position, Quaternion.identity, transform);
            isSummoned = true;
        }
    }
}
