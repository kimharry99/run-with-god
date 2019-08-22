using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneKillTrust : Trust
{
    public int needKillCount;
    private int killCount;
    private Coroutine reset = null;
    private bool isDone;

    public IEnumerator ResetKillCount()
    {
        yield return new WaitForSeconds(.1f);
        if (killCount < needKillCount)
        {
            killCount = 0;
            reset = null;
            InGameUIManager.inst.UpdateTrustUI(this);
        }
        else
        {
            isDone = true;
        }
    }

    public override bool IsDone { get { return isDone; } }

    public override string GetDescription()
    {
        return "몬스터를 한 번에 " + needKillCount.ToString() + "마리 처치하시오.";
    }

    public override string GetName()
    {
        return "몬스터 한 번에 처치";
    }

    public override void Init()
    {
        killCount = 0;
        isDone = false;

        GameManager.inst.OnEnemyKilled += delegate
        {
            if (reset == null)
                reset = GameManager.inst.StartCoroutine(ResetKillCount());
        };
        GameManager.inst.OnEnemyKilled += KillCheck;
        GameManager.inst.OnEnemyKilled += delegate { InGameUIManager.inst.UpdateTrustUI(this); };
    }

    private void KillCheck(EnemyType type)
    {
        ++killCount;
        Debug.Log("OneKill Count : " + killCount);
    }

    public override string TrustToText()
    {
        return Mathf.Min(killCount, needKillCount) + " / " + needKillCount;
    }
}
