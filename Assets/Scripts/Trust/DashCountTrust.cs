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
        string desc = "";
        foreach (var substring in description.Split(' ', '\n'))
        {
            if (substring == "%need_dash")
            {
                desc += needDashCount;
            }
            else
            {
                desc += substring;
            }
            desc += " ";
        }
        return desc;
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
