using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpCountTrust : Trust
{
    public int needJumpCount;
    private int jumpCount;
    
    public void UpJumpCount()
    {
        jumpCount++;
        Debug.Log("Jump Count : " + jumpCount);
    }

    public override string GetDescription()
    {
        return "점프를 " + needJumpCount.ToString() + "번 하시오.";
    }

    public override string GetName()
    {
        return "점프하기";
    }

    public override bool IsDone { get { return jumpCount >= needJumpCount; } }

    public override void Init()
    {
		jumpCount = 0;
        PlayerController.inst.OnJump += UpJumpCount;
        PlayerController.inst.OnJump += delegate { InGameUIManager.inst.UpdateTrustUI(this); };
    }

    public override string TrustToText()
    {
        return Mathf.Min(jumpCount, needJumpCount) + " / " + needJumpCount;
    }
}
