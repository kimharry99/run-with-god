﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tombstone : NormalEnemy
{
	[SerializeField]
	private bool isHeal;
	private const int healAmount = 5;
	private float healTimer = 5;

	[SerializeField]
	private Butcher butcher;

	public override EnemyType Type { get { return EnemyType.ALL; } }

	protected override void InitEnemy()
	{
		State active = new State();
		active.StateUpdate += delegate
		{
			if (healTimer > 0)
				healTimer -= Time.deltaTime;
			if (isHeal)
			{
				if(healTimer <= 0)
				{
					butcher.GetHealed(10);
					healTimer = 5;
				}
			}
			else
			{
				PlayerController.inst.speedScale = 0.5f;
			}
		};

		stateMachine.AddNewState("active", active);
		stateMachine.Transition("active");
	}

	public override void GetDamagedToDeath()
	{

	}

	protected override void OnDead()
	{
		if (!isHeal)
			PlayerController.inst.speedScale = 1;
		gameObject.SetActive(false);
	}
}
