using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpCountTrust : Trust
{
    public const int needJumpCount = 20;
    private int jumpCount;
    
    public void UpJumpCount()
    {
        jumpCount++;
        Debug.Log(jumpCount);
    }

    public override bool IsDone { get { return jumpCount >= needJumpCount; } }

    public override void Init()
    {
		jumpCount = 0;
        PlayerController.inst.OnJump += delegate { InGameUIManager.inst.UpdateTrustUI(this); };
        PlayerController.inst.OnJump += UpJumpCount;
    }

    public override string TrustToText()
    {
        return Mathf.Min(jumpCount, needJumpCount) + " / " + needJumpCount;
    }
}
