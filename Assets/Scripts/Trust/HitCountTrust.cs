using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitCountTrust : Trust
{
    private int hitCount;
    public int needHitCount;

    public void UpHitCount()
    {
        hitCount++;
        InGameUIManager.inst.UpdateTrustUI(this);
    }

    public override bool IsDone { get { return hitCount >= needHitCount; } }

    public override string GetDescription()
    {
        string desc = "";
        foreach (var substring in description.Split(' ', '\n'))
        {
            if (substring == "%need_hit")
            {
                desc += needHitCount;
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
        hitCount = 0;
        PlayerController.inst.GetHit += UpHitCount;
    }

    public override string TrustToText()
    {
        return Mathf.Min(hitCount, needHitCount) + " / " + needHitCount;
    }
}
