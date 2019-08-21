using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftMoveTrust : Trust
{
    public float needTime;
    private float leftMoveTime;
    private bool isDone;
    private Coroutine checkDirection;

    public IEnumerator CheckMoveDirection()
    {
        while (true)
        {
            if (Input.GetAxis("Horizontal") < 0) // Move Left
            {
                Debug.Log("Move Left");
                leftMoveTime += Time.deltaTime;

                if (leftMoveTime >= needTime)
                {
                    isDone = true;
                    GameManager.inst.StopCoroutine(checkDirection);
                }
            }
            else
            {
                leftMoveTime = 0f;
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
            if (substring == "%need_time")
            {
                desc += needTime;
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
        int leftTime = leftMoveTime < needTime ? (int)leftMoveTime : (int)needTime;
        int rightTime = (int)needTime;

        return leftTime.ToString() + " / " + rightTime.ToString();
    }
    public override void Init()
    {
        leftMoveTime = 0f;
        isDone = false;
        
        checkDirection = GameManager.inst.StartCoroutine(CheckMoveDirection());
    }
}
