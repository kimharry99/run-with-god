using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Trust : ScriptableObject
{
	public TrustType trustType;
	public string trustName = null;
	public string description = null;
	public int tier = 0;
	public abstract bool IsDone { get; }
	public virtual string GetDescription()
	{
		return description;
	}
	/// <summary>
	/// 신탁의 진행 상황을 string으로 변환합니다.
	/// </summary>
	/// <returns></returns>
	public abstract string TrustToText();
	public abstract void Init();
}
