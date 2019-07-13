﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillInJumpTrust : Trust
{
    public int needKillCount;
    private int killCount;

    public override bool IsDone { get { return killCount >= needKillCount; } }

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
