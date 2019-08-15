using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : NormalEnemy
{
    public override EnemyType Type { get { return EnemyType.SHIELD; } }
   protected override void InitEnemy()
    {

    }

    protected override void OnDead()
    {
        gameObject.SetActive(false);
        //Destroy(gameObject);
    }
}
