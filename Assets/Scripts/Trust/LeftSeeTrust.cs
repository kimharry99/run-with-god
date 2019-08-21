using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftSeeTrust : Trust
{
    public float limitTime;
    private float leftSeeTime;
    private bool isDone;
    private Coroutine checkDirection;

    public IEnumerator CheckSeeDirection()
    {
        while (true)
        {
            if (Input.GetAxis("Horizontal") < 0) // input left button
            {
                while(Input.GetAxis("Horizontal") <= 0)
                {
                    leftSeeTime += Time.deltaTime;
                    InGameUIManager.inst.UpdateTrustUI(this);

                    if(leftSeeTime >= limitTime)
                    {
                        isDone = false;
                        GameManager.inst.StopCoroutine(checkDirection);
                    }

                    yield return null;
                }

                leftSeeTime = 0f;
            }
            else
            {
                leftSeeTime = 0f;
            }

            InGameUIManager.inst.UpdateTrustUI(this);
            yield return null;
        }
    }

    public override bool IsDone { get { return isDone; } }

    public override string GetDescription()
    {
        string desc = "";
        foreach (var substring in description.Split(' ', '\n'))
        {
            if (substring == "%limit_time")
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

    public override string TrustToText()
    {
        int leftTime = leftSeeTime < limitTime ? (int)leftSeeTime : (int)limitTime;
        int rightTime = (int)limitTime;

        return leftTime.ToString() + " / " + rightTime.ToString();
    }
    public override void Init()
    {
        leftSeeTime = 0f;
        isDone = true;

        checkDirection = GameManager.inst.StartCoroutine(CheckSeeDirection());
    }
}
