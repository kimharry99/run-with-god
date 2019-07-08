using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Butcher : NormalEnemy
{
    public override EnemyType Type => throw new System.NotImplementedException();
    public Transform sickle;

    public Collider2D hitRange;

    protected override void InitEnemy()
    {
        State move = new State();
        State attack = new State();
        State pattern1 = new State();
        State pattern2 = new State();
        State pattern3 = new State();
        State pattern4 = new State();
        State pattern5 = new State();

        move.StateUpdate += FollowPlayer;
    }

    private IEnumerator AttackRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        if (hitRange.IsTouchingLayers(1 << LayerMask.NameToLayer("Player")))
        {
            PlayerController.inst.GetDamaged();
        }
    }

    protected override void Flip()
    {
        base.Flip();
        foreach(Transform child in transform)
        {
            child.localPosition = new Vector3(-child.localPosition.x, child.localPosition.y, child.localPosition.z);
        }
    }

    private void ThrowSickle()
    {
        StartCoroutine(ThrowSickleRoutine());
    }

    private IEnumerator ThrowSickleRoutine()
    {
        throw new System.NotImplementedException();
    }
}
