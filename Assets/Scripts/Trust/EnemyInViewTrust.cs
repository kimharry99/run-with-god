using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInViewTrust : Trust
{
    public float limitTime;
    public int limitEnemycount;
    private float inRangeTime;
    private int enemyCount;
    private bool isDone;

    public override string GetDescription()
    {
        string desc = "";
        foreach (var substring in description.Split(' ', '\n'))
        {
            if (substring == "%limit_time")
            {
                desc += limitTime;
            }
            else if (substring == "%limit_count")
            {
                desc += limitEnemycount;   
            }
            else
            {
                desc += substring;
            }
            desc += " ";
        }
        return desc;
    }

    public override bool IsDone { get { return isDone; } }

    public override void Init()
    {
        isDone = true;
        inRangeTime = 0f;


    }

    public override string TrustToText()
    {
        string time = "Time : " + Mathf.Min(inRangeTime, limitTime) + " / " + limitTime;
        string count = "Count : " + Mathf.Min(enemyCount, limitEnemycount) + " / " + limitEnemycount;

        return time + "\n" + count;
    }
}
