using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//bullet이 일직선으로 발사하는 거미줄
public class Spider : NormalEnemy
{
    public GameObject webTrap;
    public GameObject baby;

    public override EnemyType Type { get { return EnemyType.BOSS_SPIDER; } }

    State patternCycle = new State();

    protected override void InitEnemy()
    {
        patternCycle.Enter += NextPattern;
        stateMachine.AddNewState("patternCycle", patternCycle);
        stateMachine.Transition("patternCycle");
    }

    protected void NextPattern()
    {
        isInvincibe = false;
        Action[] patterns;
        //if (Trust.isDone)// check Trust is completed
        if (true)
        {
            patterns = new Action[]{
            ShotWeb,
            SetTrap,
            SprayWeb,
            GetDownWithLeg,
            HealFromCocoon
            };
        } else
        {
            patterns = new Action[]{
            SummonBaby,
            BreakBaby,
            AttackRange,
            GetDownWithLeg,
            MovingAttack
            };
        }

        int choice = UnityEngine.Random.Range(0, patterns.Length);

        patterns[choice]();
        WaitSeconds(3f);
        stateMachine.Transition("patternCycle");
    }

    protected IEnumerator WaitSeconds(float length)
    {
        yield return new WaitForSeconds(length);
    }

    protected void GetDownWithLeg()
    {

    }

    #region Trust Win Spider Function
    protected void ShotWeb()
    {
        AttackProjectile();
    }

    protected void SetTrap()
    {
        GameObject trap = Instantiate(webTrap) as GameObject;

        StartCoroutine(WaitSeconds(1f));
    }

    protected void SprayWeb()
    {


        StartCoroutine(WaitSeconds(1f));
    }

    protected void HealFromCocoon()
    {
        isInvincibe = true;
        Health += 20;   //치유량
    }
    #endregion

    #region Trust Lose Spider Function
    protected void SummonBaby()
    {
        StartCoroutine(WaitSeconds(1f));
    }

    protected void BreakBaby()
    {
        StartCoroutine(WaitSeconds(1f));
    }

    protected void AttackRange()
    {
        StartCoroutine(WaitSeconds(1f));
    }

    protected void MovingAttack()
    {
        StartCoroutine(WaitSeconds(1f));
    }

    #endregion
}
