using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : Shielder
{
   protected override void InitEnemy()
    {

    }

    protected override void OnDead()
    {
        Destroy(gameObject);
    }
}
