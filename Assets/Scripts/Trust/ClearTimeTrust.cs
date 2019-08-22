using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearTimeTrust : Trust
{
    public float needClearTime;

    public override bool IsDone { get { return GameManager.inst.Playtime <= needClearTime; } }

    public override string GetDescription()
    {
        return ((int)needClearTime / 60).ToString() + "분 이내로 클리어 하시오.";
    }

    public override string GetName()
    {
        return "시간 내에 클리어";
    }

    public override void Init()
    {
        GameManager.inst.OnPlayTimeChanged += delegate { InGameUIManager.inst.UpdateTrustUI(this); };
    }

    public override string TrustToText()
    {
        string minutes = ((int)needClearTime / 60).ToString("00");
        string seconds = (needClearTime % 60).ToString("00");

        if (GameManager.inst.Playtime >= needClearTime)
        {
            return minutes + ":" + seconds + " / " + minutes + ":" + seconds;
        }
        else
        {
            return InGameUIManager.inst.playtimeText+ " / " + minutes + ":" + seconds;
        }
    }
}
