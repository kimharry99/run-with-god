using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrustSelectionUIManager : SingletonBehaviour<TrustSelectionUIManager>
{
	public Text trustNameText;
	public Text trustDescriptionText;


    IEnumerator UpdateTrustInfoText(Trust trust)
	{
        trustNameText.text = trust.trustName.ToString();
        string descript = trust.description.ToString();
        yield return new WaitForSeconds(0.05f);
        for(int i =0;i<=descript.Length;i++)
        {
            trustDescriptionText.text = descript.Substring(0, i);

            yield return new WaitForSeconds(0.15f);
        }
    }
}
