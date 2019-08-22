﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleUIManager : MonoBehaviour
{
	public Slider difficultySlider;

	public void SetGalleryDifficulty()
	{
		GameManager.inst.galleryDifficulty = (int)difficultySlider.value;
	}

	public void SetGalleryBossType(int type)
	{
		GameManager.inst.galleryType = (TrustType)type;
	}

    public void OnSelectBossButtonClicked()
	{
		SceneManager.LoadScene("Boss_Gallery");
	}
}
