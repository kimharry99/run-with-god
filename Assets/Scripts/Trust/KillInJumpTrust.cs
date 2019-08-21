using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillInJumpTrust : Trust
{
    public int needKillCount;
    private int killCount;

    public override bool IsDone { get { return killCount >= needKillCount; } }

    public override string GetDescription()
    {
        return "공중에서 " + needKillCount.ToString() + "마리의 몬스터를 처치하시오.";
    }

    public override string GetName()
    {
        return "공중에서 몬스터 처치";
    }
    public override void Init()
    {
        killCount = 0;
        GameManager.inst.OnEnemyKilled += KillCheck;
        GameManager.inst.OnEnemyKilled += delegate { InGameUIManager.inst.UpdateTrustUI(this); };
    }

    private void KillCheck(EnemyType type)
    {
        if (!PlayerController.inst.IsGround)
            ++killCount;
        Debug.Log("Kill Count(in Jump) : " + killCount);
    }

    public override string TrustToText()
    {
        return Mathf.Min(killCount, needKillCount) + " / " + needKillCount;
    }
}
