﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrustSelector : MonoBehaviour
{   
	public static Trust SelectedTrust { get; private set; }

	[SerializeField]
	private Trust trust;

	[SerializeField]
	private ParticleSystem spotlight;
    public Light Light;
    private float time;

    private void Start()
    {
        spotlight.Stop();
    }

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
        spotlight.Play();
        StopCoroutine("FadeOut");
        StartCoroutine("FadeIn");
    }

    private void OnSelected()
	{
		//Update SelectedTrust
		//Selection confirm UI Opened
	}

	private void OnUnhighlighted()
	{
        //get darker & stop spotlight effect
        spotlight.Stop();
        StopCoroutine("FadeIn");
        StartCoroutine("FadeOut");
    }

    IEnumerator FadeIn()
    {
        while(Light.intensity < 30f)
        {
            Light.intensity += 2;
            yield return new WaitForSeconds(0.02f);
        }  
    }

    IEnumerator FadeOut()
    {
        while (Light.intensity > 0.0f)
        {
            Light.intensity -= 2.5f;
            yield return new WaitForSeconds(0.02f);
        }
    }
}
