using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrustSelector : MonoBehaviour
{
	public static Trust SelectedTrust { get; private set; }

	[SerializeField]
	private Trust trust;

	[SerializeField]
	private ParticleSystem spotlight;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		OnHighlighted();
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		if (Input.GetAxis("Vertical") > 0)
		{
			OnSelected();
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		OnUnhighlighted();
	}

	/// <summary>
	/// Called when player is nearby
	/// </summary>
	private void OnHighlighted()
	{
		//get brighter & start spotlight effect
		//Trust info UI Update
	}

	private void OnSelected()
	{
		//Update SelectedTrust
		//Selection confirm UI Opened
	}

	private void OnUnhighlighted()
	{
		//get darker & stop spotlight effect
	}
}
