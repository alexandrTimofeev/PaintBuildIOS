using System;
using UnityEngine;
using System.Collections.Generic;

public enum UiSfxType { ClickOpen, ClickClose, NonInteract }
public static class SoundManager
{
    private static List<AudioSource> sourcesUI = new List<AudioSource>();
    private static Dictionary<string, AudioSource> sourcesSFXDic = new Dictionary<string, AudioSource>();

    public static void Init()
    {
        sourcesUI.Clear();
        sourcesSFXDic.Clear();
    }

    public static AudioSource GetAvilibleAudioSourceUI()
    {
        // 1. »щем свободный источник
        for (int i = 0; i < sourcesUI.Count; i++)
        {
            if (!sourcesUI[i].isPlaying)
            {
                return sourcesUI[i];
            }
        }

        // 2. Ќет свободных Ч создаЄм новый
        return CreateNewUISource();
    }

    private static AudioSource CreateNewUISource()
    {
        Camera cam = Camera.main;

        if (cam == null)
        {
            Debug.LogError("Camera.main not found for SoundManager");
            return null;
        }

        GameObject go = new GameObject("UI_AudioSource");
        go.transform.position = cam.transform.position;

        AudioSource source = go.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 0f; // UI Ч всегда 2D звук
        source.outputAudioMixerGroup = GameSettings.GlobalMixer.FindMatchingGroups("Master")[0];

        sourcesUI.Add(source);
        return source;
    }

    public static void PlayInterfaceSFX(UiSfxType sfxType)
    {
        if (SFXGlobalData.InstanceGlobal == null)
            return;

        switch (sfxType)
        {
            case UiSfxType.ClickOpen:
                if(SFXGlobalData.InstanceGlobal.StandardClickOpen != null)
                    PlayInterfaceSFX(SFXGlobalData.InstanceGlobal.StandardClickOpen);
                break;
            case UiSfxType.ClickClose:
                if (SFXGlobalData.InstanceGlobal.StandardClickClose != null)
                    PlayInterfaceSFX(SFXGlobalData.InstanceGlobal.StandardClickClose);
                break;
            case UiSfxType.NonInteract:
                if (SFXGlobalData.InstanceGlobal.StandardClickNonInteract != null)
                    PlayInterfaceSFX(SFXGlobalData.InstanceGlobal.StandardClickNonInteract);
                break;
            default:
                break;
        }
    }

    public static void PlayInterfaceSFX(string ID)
    {
        SFXPlayPreset playPreset = SFXGlobalData.InstanceGlobal.GetAtID(ID);
        PlayInterfaceSFX(playPreset);
    }

    public static void PlayInterfaceSFX(SFXPlayPreset presetClick)
    {
        if (GameSettings.GlobalMixer == null)
            return;

        AudioSource source = GetAvilibleAudioSourceUI();
        presetClick.ApplyToSource(source);
        source.PlayOneShot(presetClick.clip);
    }

    public static void PlaySFXNotOverlap(SFXPlayPreset presetClick, float minTimePlay = Mathf.Infinity)
    {
        if (GameSettings.GlobalMixer == null)
            return;

        if (sourcesSFXDic.ContainsKey(presetClick.ID))
        {
            if (sourcesSFXDic[presetClick.ID].isPlaying && sourcesSFXDic[presetClick.ID].clip == presetClick.clip && 
                sourcesSFXDic[presetClick.ID].time <= minTimePlay)
                return;

            sourcesSFXDic.Remove(presetClick.ID);
        }

        AudioSource source = GetAvilibleAudioSourceUI();
        presetClick.ApplyToSource(source);

        source.clip = presetClick.clip;
        source.loop = false;
        source.Play();

        sourcesSFXDic.Add(presetClick.ID, source);
    }
}
