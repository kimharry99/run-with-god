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
        StopUpdateTrustInfoText();
        StartCoroutine(UpdateTrustInfoTextRoutine(trust, 0.05f));
	}

    public void UpdateTrustInfoText(Trust trust, float interval)
    {
        //Start Cotroutine to update description, name text
        StopUpdateTrustInfoText();
        StartCoroutine(UpdateTrustInfoTextRoutine(trust, interval));
    }

    public void StopUpdateTrustInfoText()
	{
        //Stop Coroutine and Empty the text
        StopAllCoroutines();
        trustNameText.text = "";
        trustDescriptionText.text = "";
	}

	public void OpenConfirmPanel()
	{
		ConfirmPanel.SetActive(true);
	}

	public void CloseConfirmPanel()
	{
		ConfirmPanel.SetActive(false);
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

    IEnumerator UpdateTrustInfoTextRoutine(Trust trust, float interval)
	{
        if (trust != null)
        {
            trustNameText.text = trust.trustName;
            string descript = trust.GetDescription();
            yield return new WaitForSeconds(0.05f);
            for (int i = 0; i <= descript.Length; i++)
            {
                trustDescriptionText.text = descript.Substring(0, i);

                yield return new WaitForSeconds(interval);
            }
        }
    }
}
