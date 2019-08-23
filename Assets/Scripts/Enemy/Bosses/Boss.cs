using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Boss : NormalEnemy
{
	public int difficulty;
    public float[] healthCoifficient;

	protected override void Start()
	{
        base.Start();
        difficulty = GetComponentInParent<MapBlock>().difficulty;
        maxHealth = (int)(maxHealth * healthCoifficient[difficulty+3]);
        Health = maxHealth;
		InGameUIManager.inst.UpdateBossHelthUI(Health);
	}

	public override void GetDamaged(int damage)
	{
		base.GetDamaged(damage);
		InGameUIManager.inst.UpdateBossHelthUI((float)Health / maxHealth);
	}

	public override void GetDamagedToDeath()
	{
		
	}
}
