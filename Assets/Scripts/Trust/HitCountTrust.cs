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
        return needHitCount.ToString() + "번 맞으시오.";
    }

    public override string GetName()
    {
        return "맞기";
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
