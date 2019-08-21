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

    Camera camera;

    public IEnumerator TimeCheck() // enemyCount >= limit Enemycount 일때
    {
        while (true)
        {
            if (limitEnemycount > enemyCount)
            {
                inRangeTime = 0;
                InGameUIManager.inst.UpdateTrustUI(this);
                break;
            }

            inRangeTime += Time.deltaTime;
            InGameUIManager.inst.UpdateTrustUI(this);

            if(inRangeTime >= limitTime)
            {
                isDone = false;
                break;
            }

            yield return null;
        }
    }

    public void EnemyInCamera()
    {
        enemyCount++;
        InGameUIManager.inst.UpdateTrustUI(this);

        if (enemyCount >= limitEnemycount)
            GameManager.inst.StartCoroutine(TimeCheck());
    }

    public void EnemyOutCamera()
    {
        enemyCount--;
        InGameUIManager.inst.UpdateTrustUI(this);
    }

    public bool IsInCamera(NormalEnemy enemy)
    {
        Vector3 enemyPosition = camera.WorldToScreenPoint(enemy.transform.position);

        if (enemyPosition.x >= 0 && enemyPosition.x <= camera.pixelWidth && enemyPosition.y >= 0 && enemyPosition.y <= camera.pixelHeight)
            return true;
        else
            return false;
    }

    public override string GetDescription()
    {
        return "시야 안에 몬스터 " + limitEnemycount.ToString() + "마리가 " + ((int)limitTime).ToString() + "초 이상 보이지 않게 하시오.";
    }

    public override string GetName()
    {
        return "시야 내 몬스터";
    }

    public override bool IsDone { get { return isDone; } }

    public override void Init()
    {
        isDone = true;
        inRangeTime = 0f;
        camera = Camera.main;

        GameManager.inst.EnemyInCamera += EnemyInCamera;
        GameManager.inst.EnemyOutCamera += EnemyOutCamera;
    }

    public override string TrustToText()
    {
        string time = "Time : " + Mathf.Min(inRangeTime, limitTime) + " / " + limitTime;
        string count = "Count : " + Mathf.Min(enemyCount, limitEnemycount) + " / " + limitEnemycount;

        return time + "\n" + count;
    }
}
