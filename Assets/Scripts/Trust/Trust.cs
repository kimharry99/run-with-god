using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Trust : ScriptableObject
{
	public string trustName = null;
	public string description = null;
	public abstract bool IsDone { get; }
	public abstract string TrustToText();
	public abstract void Init();
}
