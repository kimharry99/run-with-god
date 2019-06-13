using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
	private Dictionary<string, State> states = new Dictionary<string, State>();
	public State CurState { get; private set; }

	public StateMachine()
	{
		CurState = null;
	}

	public void AddNewState(string newStateName, State newState)
	{
		states.Add(newStateName, newState);
	}

	public void Transtion(string stateName)
	{
		if (CurState != null)
		{
			CurState.Exit?.Invoke();
		}
		CurState = states[stateName];
		CurState.Enter?.Invoke();
	}

	public void UpdateStateMachine()
	{
		CurState.StateUpdate?.Invoke();
	}
}
