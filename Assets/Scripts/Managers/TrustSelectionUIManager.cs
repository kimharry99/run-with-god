using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TrustSelectionUIManager : SingletonBehaviour<TrustSelectionUIManager>
{
	public Text trustNameText;
	public Text trustDescriptionText;
	public GameObject ConfirmPanel;


    public void UpdateTrustInfoText(Trust trust)
	{
		//Start Cotroutine to update description, name text
	}

	public void StopUpdateTrustInfoText()
	{
		//Stop Coroutine and Empty the text
	}

	public void OpenConfirmPanel()
	{
		ConfirmPanel.SetActive(true);
		//SetActive true ConfirmPanel
	}

	public void CloseConfirmPanel()
	{
		ConfirmPanel.SetActive(false);
		//SetActive false ConfirmPanel
	}

	public void TrustConfirmYes()
	{
		if (GameManager.inst.SelectedTrust == null)
		{
			Debug.LogWarning("SelectedTrust is null");
			return;
		}
		SceneManager.LoadScene("InGameScene");
	}

	public void TrustConfirmNo()
	{
		CloseConfirmPanel();
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
