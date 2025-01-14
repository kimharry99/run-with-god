﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : SingletonBehaviour<SoundManager>
{

    Dictionary<string, AudioClip> audioClips;

    void Awake()
    {
		/*
        audioClips = new Dictionary<string, AudioClip>();
        AudioClip[] clips = Resources.LoadAll<AudioClip>("Sounds");
        foreach (AudioClip clip in clips)
        {
            if (audioClips.ContainsKey(clip.name + clip.length))
                continue;
            audioClips.Add(clip.name+clip.length, clip);
        }
		*/
    }

    public void PlaySFX(GameObject target, AudioClip audio)
    {
        if (audio == null)
            return;
        StartCoroutine(Play(target, audio));
    }

    public void PlaySFX(GameObject target, AudioClip audio, float volume)
    {
        if (audio == null)
            return;
        StartCoroutine(Play(target, audio, volume));
    }

    public void PlaySFX(GameObject target, string clip)
    {
        StartCoroutine(Play(target, audioClips[clip]));
    }

    IEnumerator Play(GameObject target, AudioClip audio)
    {
        AudioSource audioSource = target.AddComponent<AudioSource>();
        audioSource.volume = 1.0f;
        audioSource.clip = audio;
        audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
        audioSource.spatialBlend = 1;
        audioSource.maxDistance = 200f;
        audioSource.Play();
        yield return new WaitForSeconds(audioSource.clip.length);
        Destroy(audioSource);
    }

    IEnumerator Play(GameObject target, AudioClip audio, float volume)
    {
        AudioSource audioSource = target.AddComponent<AudioSource>();
        audioSource.volume = volume;
        audioSource.clip = audio;
        audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
        audioSource.spatialBlend = 1;
        audioSource.maxDistance = 200f;
        audioSource.Play();
        yield return new WaitForSeconds(audioSource.clip.length);
        Destroy(audioSource);
    }

}