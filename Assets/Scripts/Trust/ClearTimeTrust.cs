﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearTimeTrust : Trust
{
    public float needClearTime;

    public override bool IsDone { get { return GameManager.inst.Playtime <= needClearTime; } }

    public override string GetDescription()
    {
        string desc = "";
        foreach (var substring in description.Split(' ', '\n'))
        {
            if (substring == "%need_minute")
            {
                desc += needClearTime / 60;
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
            return InGameUIManager.inst.playtimeText.text+ " / " + minutes + ":" + seconds;
        }
    }
}
