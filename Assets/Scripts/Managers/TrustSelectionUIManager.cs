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

	}

	public void TrustConfirmNo()
	{

	}

    private void Start()
    {

    }

    IEnumerator UpdateTrustInfoText(string trust)
	{
        string descript = trust.ToString();
        yield return new WaitForSeconds(0.05f);
        for(int i =0;i<=descript.Length;i++)
        {
            trustDescriptionText.text = descript.Substring(0, i);

            yield return new WaitForSeconds(0.15f);
        }
    }
}
