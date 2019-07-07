using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseBulletTrust : Trust
{
    public int needUseBullet;
    private int useBulletCount;

    public override bool IsDone { get { return useBulletCount >= needUseBullet; } }

    public override string GetDescription()
    {
        string desc = "";
        foreach (var substring in description.Split(' ', '\n'))
        {
            if (substring == "%need_bullet")
            {
                desc += needUseBullet;
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
        useBulletCount = 0;
        PlayerController.inst.OnShotBullet += BulletCount;
        PlayerController.inst.OnShotBullet += delegate { InGameUIManager.inst.UpdateTrustUI(this); };
    }

    private void BulletCount()
    {
        useBulletCount++;
        Debug.Log("Use Bullet : " + useBulletCount);
    }

    public override string TrustToText()
    {
        return Mathf.Min(useBulletCount, needUseBullet) + " / " + needUseBullet;
    }
}
