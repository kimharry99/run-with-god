using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrustSelectionUIManager : SingletonBehaviour<TrustSelectionUIManager>
{
	public Text trustNameText;
	public Text trustDescriptionText;

	public Transform ConfirmPanel;


    public void UpdateTrustInfoText(Trust trust)
	{
		//TODO
	}

	public void OpenConfirmPanel()
	{
		//SetActive true ConfirmPanel
	}

	public void CloseConfirmPanel()
	{
		//SetActive false ConfirmPanel
	}

	public void TrustConfirmYes()
	{
		//
	}

	public void TrustConfirmNo()
	{

	}
}
