using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnGroundTrust : Trust
{
    public float limitTime;
    private float inAirTime;
    private bool isDone;
    private Coroutine reset;

    public IEnumerator ResetTime()
    {
        while (true)
        {
            Debug.Log("reset Time");

            if (!PlayerController.inst.IsGround)
            {
                inAirTime += Time.deltaTime;
                InGameUIManager.inst.UpdateTrustUI(this);

                if (inAirTime >= limitTime)
                {
                    isDone = false;
                    Debug.Log("IsDone : " + isDone);
                    GameManager.inst.StopCoroutine(reset);
                }
            }
            else
            {
                inAirTime = 0.0f;
                InGameUIManager.inst.UpdateTrustUI(this);
            }

            yield return new WaitForSeconds(.01f);
        }
    }

    public override string GetDescription()
    {
        return "지면에서 " + limitTime.ToString() + "초 이상 떨어져 있지 마시오.";
    }

    public override string GetName()
    {
        return "땅에서 떨어지기 금지";
    }

    public override bool IsDone { get { return isDone; } }

    public override void Init()
    {
        inAirTime = 0.0f;
        isDone = true;
        reset = GameManager.inst.StartCoroutine(ResetTime());
    }

    public override string TrustToText()
    {
        return Mathf.Min(inAirTime , limitTime) + " / " + limitTime ;
    }
}
