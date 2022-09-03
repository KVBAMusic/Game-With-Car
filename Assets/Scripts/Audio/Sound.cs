using UnityEngine;
using UnityEngine.Audio;
[System.Serializable]
public class Sound
{
    public enum SoundType { Music, SFX };

    public string name;

    public AudioClip clip;
    public SoundType soundType;
    [Range(0f, 1f)] public float volume = 0f;
    [Range(1f, 5f)] public float pitch = 1f;
    public float volumeMultiplier = 1f;
    public bool loop;
    [HideInInspector] public AudioSource source;
    public AudioMixerGroup group;
}