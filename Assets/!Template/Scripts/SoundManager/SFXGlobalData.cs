using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "SFXGlobalData", menuName = "SGames/SFXGlobalData")]
public class SFXGlobalData : ScriptableObject
{
    public SFXPlayPreset StandardClickOpen;
    public SFXPlayPreset StandardClickClose;
    public SFXPlayPreset StandardClickNonInteract;

    [Space]
    public SFXPlayPreset[] sfxPresets;

    private static SFXGlobalData instanceGlobal;
    public static SFXGlobalData InstanceGlobal
    {
        get
        {
            if (instanceGlobal == null)
                instanceGlobal = Resources.Load<SFXGlobalData>("SFXGlobalData");
            return instanceGlobal;
        }
    }

    public SFXPlayPreset GetAtID(string ID)
    {
        foreach (var preset in sfxPresets)
        {
            if (preset.ID == ID)            
                return preset;            
        }
        return null;
    }
}

[Serializable]
public class SFXPlayPreset
{
    public string ID;
    public AudioClip clip;

    [Space]
    public float volume = 1f;
    public float pitch = 1f;

    [Space]
    public float pitchOffserRnd = 0f;
    public float volumeOffserRnd = 0f;

    [Space]
    public bool IsUseReverb = false;
    public AudioReverbPreset reverbPreset;

    public void ApplyToSource(AudioSource source)
    {
        source.volume = volume + UnityEngine.Random.Range(-volumeOffserRnd, volumeOffserRnd);
        source.pitch = pitch + UnityEngine.Random.Range(-pitchOffserRnd, pitchOffserRnd);

        AudioReverbFilter audioReverbFilter = source.GetComponent<AudioReverbFilter>();
        if (IsUseReverb) {
            if (audioReverbFilter == null)
                audioReverbFilter = source.AddComponent<AudioReverbFilter>();
            audioReverbFilter.reverbPreset = reverbPreset;
        }
        else if (audioReverbFilter != null)          
            UnityEngine.Object.Destroy(audioReverbFilter);                
    }
}