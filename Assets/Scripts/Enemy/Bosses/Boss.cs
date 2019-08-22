using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Boss : NormalEnemy
{
	public int difficulty;

	protected override void Start()
	{
		base.Start();
		difficulty = GetComponentInParent<MapBlock>().difficulty;
	}

	public override void GetDamaged(int damage)
	{
		base.GetDamaged(damage);
		InGameUIManager.inst.UpdateBossHelthUI(Health / maxHealth);
	}

	public override void GetDamagedToDeath()
	{
		
	}
}
