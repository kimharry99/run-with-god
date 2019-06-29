using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillCountTrust : Trust
{
	public const int needKillCount = 30;

	public override bool IsDone { get { return GameManager.inst.KillCount >= needKillCount; } }

	public override void Init()
	{
		GameManager.inst.OnKillCountChanged += delegate { InGameUIManager.inst.UpdateTrustUI(this); };
	}

	public override string TrustToText()
	{
		return Mathf.Min(GameManager.inst.KillCount, needKillCount) + " / " + needKillCount;
	}
}
