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
        return "총알을 " + needUseBullet.ToString() + "발 사용하시오.";
    }

    public override string GetName()
    {
        return "총알 사용하기";
    }

    public override void Init()
    {
        useBulletCount = 0;
        PlayerController.inst.gun.OnShotBullet += BulletCount;
        PlayerController.inst.gun.OnShotBullet += delegate { InGameUIManager.inst.UpdateTrustUI(this); };
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
