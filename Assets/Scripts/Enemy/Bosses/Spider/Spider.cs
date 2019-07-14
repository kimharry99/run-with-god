using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//bullet이 일직선으로 발사하는 거미줄
public class Spider : NormalEnemy
{
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
        //waitSomeSeconds;
        stateMachine.Transition("patternCycle");
    }

    protected void GetDownWithLeg()
    {

    }

    #region Trust Win Spider Function
    protected void ShotWeb()
    {

    }

    protected void SetTrap()
    {

    }

    protected void SprayWeb()
    {

    }

    protected void HealFromCocoon()
    {

    }
    #endregion

    #region Trust Lose Spider Function
    protected void SummonBaby()
    {

    }

    protected void BreakBaby()
    {

    }

    protected void AttackRange()
    {

    }

    protected void MovingAttack()
    {

    }

    #endregion
}
