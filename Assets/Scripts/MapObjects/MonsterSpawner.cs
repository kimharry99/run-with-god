using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public GameObject monster;  //소환할 몬스터
    public float term;          //주기(초)
    public int range;

    void Start()
    {
        monster.transform.position = transform.position;

        if(term != 0)
            InvokeRepeating("Spawn", term, term);
    }

    public void Spawn()
    {
        if(monster != null)
            Instantiate(monster);
    }

    public void StopSpawn()
    {
        CancelInvoke("Spawn");
    }

    public void SetMonster(GameObject newMonster)
    {
        monster = newMonster;
        newMonster.transform.position = transform.position;
    }

    /*IEnumerator SpawnRoutine()
    {
        Spawn();

        yield return new WaitForSeconds(term * Time.deltaTime);
    }*/
}
