using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnGroundTrust : Trust
{
    public float limitTime;
    private float notGroundTime;
    private bool isDone;
    private Coroutine reset = null;

    public IEnumerator ResetTime()
    {
        notGroundTime += Time.deltaTime;

        if (PlayerController.inst.IsGround && notGroundTime < limitTime)
        {
            notGroundTime = 0f;
            reset = null;
            InGameUIManager.inst.UpdateTrustUI(this);
        }
        else if(notGroundTime >= limitTime)
            isDone = false;

        yield return null;
    }

    public override string GetDescription()
    {
        string desc = "";
        foreach (var substring in description.Split(' ', '\n'))
        {
            if (substring == "%need_time")
            {
                desc += limitTime;
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
        notGroundTime = 0f;
        isDone = true;

        PlayerController.inst.OnJump += delegate
        {
            if (reset == null)
                reset = GameManager.inst.StartCoroutine(ResetTime());
        };
        PlayerController.inst.OnJump += delegate { InGameUIManager.inst.UpdateTrustUI(this); };
    }

    public override string TrustToText()
    {
        return Mathf.Min(notGroundTime, limitTime) + " / " + limitTime;
    }
}
