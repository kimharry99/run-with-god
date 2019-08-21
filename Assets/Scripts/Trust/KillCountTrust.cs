using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillCountTrust : Trust
{
	public EnemyType enemyType;
	public int needKillCount;
	private int killCount;

	public override bool IsDone { get { return killCount >= needKillCount; } }

	public override string GetDescription()
    {
        return NormalEnemy.TypeToName(enemyType) + "를(을) " + needKillCount.ToString() + "마리 처치하시오.";
	}

    public override string GetName()
    {
        return NormalEnemy.TypeToName(enemyType) + " 처치";
    }

    public override void Init()
	{
		killCount = 0;
		GameManager.inst.OnEnemyKilled += KillCheck;
		GameManager.inst.OnEnemyKilled += delegate { InGameUIManager.inst.UpdateTrustUI(this); };
	}

	private void KillCheck(EnemyType type)
	{
		if (this.enemyType == EnemyType.ALL || this.enemyType == type)
			++killCount;
		Debug.Log("Kill Count : " + killCount);
	}

	public override string TrustToText()
	{
		return Mathf.Min(killCount, needKillCount) + " / " + needKillCount;
	} 
}
