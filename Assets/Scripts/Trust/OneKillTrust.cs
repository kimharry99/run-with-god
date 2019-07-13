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
        string desc = "";
        foreach (var substring in description.Split(' ', '\n'))
        {
            if (substring == "%monster_killcount")
            {
                desc += needKillCount;
            }
            else
            {
                desc += substring;
            }
            desc += " ";
        }
        return desc;
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
