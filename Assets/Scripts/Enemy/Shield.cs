using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : NormalEnemy
{
    public override EnemyType Type { get { return EnemyType.SHIELDER; } }
   protected override void InitEnemy()
    {

    }

    protected override void OnDead()
    {
        Destroy(gameObject);
    }
}
