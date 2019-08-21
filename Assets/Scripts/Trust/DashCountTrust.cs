using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashCountTrust : Trust
{
    public int needDashCount;
    private int dashCount;

    public void UpDashCount()
    {
        dashCount++;
        Debug.Log("Dash Count : " + dashCount);
    }

    public override string GetDescription()
    {
        return "대쉬를 " + needDashCount.ToString() + "번 하시오.";
    }

    public override string GetName()
    {
        return "대쉬하기";
    }

    public override bool IsDone { get { return dashCount >= needDashCount; } }

    public override void Init()
    {
        dashCount = 0;
        PlayerController.inst.OnDash += UpDashCount;
        PlayerController.inst.OnDash += delegate { InGameUIManager.inst.UpdateTrustUI(this); };
    }

    public override string TrustToText()
    {
        return Mathf.Min(dashCount, needDashCount) + " / " + needDashCount;
    }

}
