using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State
{
	public Action Enter = null;
	public Action Exit = null;
	public Action StateUpdate = null;
}
