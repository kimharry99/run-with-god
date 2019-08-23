using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TrustType
{
	ATTACK,
	MOVE,
	ACTION
}

public class TrustSelector : MonoBehaviour
{
	public static Trust SelectedTrust;

	public TrustType type;
	[SerializeField]
	public Trust Trust { get; private set; }

	[SerializeField]
	private ParticleSystem spotlight;
    public Light Light;
    private float time;

    private void Start()
    {
		Trust = GameManager.inst.PickTrust(type);
    }

    private void OnTriggerEnter2D(Collider2D collision)
	{
        if(collision.CompareTag("Player"))
		    OnHighlighted();
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
        if (collision.CompareTag("Player"))
            if (Input.GetAxis("Vertical") > 0)
		{
			OnSelected();
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
        if (collision.CompareTag("Player"))
            OnUnhighlighted();
    }

	public void InitTrustSelector(Trust trust)
	{
        this.Trust = trust;
        if (trust == null)
        {
            gameObject.SetActive(false);
        }
	}
    /// <summary>
    /// Called when player is nearby
    /// </summary>
    private void OnHighlighted()
    {
        //get brighter & start spotlight effect
        spotlight.Play();
        StopCoroutine("FadeOut");
        StartCoroutine("FadeIn");
        TrustSelectionUIManager.inst.UpdateTrustInfoText(Trust);
    }

    private void OnSelected()
	{
		SelectedTrust = Trust;
		TrustSelectionUIManager.inst.OpenConfirmPanel();
	}

	private void OnUnhighlighted()
	{
		//get darker & stop spotlight effect
		spotlight.Clear();
        spotlight.Stop();
        StopCoroutine("FadeIn");
        StartCoroutine("FadeOut");
        TrustSelectionUIManager.inst.StopUpdateTrustInfoText();
    }

    IEnumerator FadeIn()
    {
        while(Light.intensity < 180f)
        {
            Light.intensity += 18;
            yield return new WaitForSeconds(0.02f);
        }  
    }

    IEnumerator FadeOut()
    {
        while (Light.intensity > 0.0f)
        {
            Light.intensity -= 30f;
            yield return new WaitForSeconds(0.02f);
        }
    }
}
