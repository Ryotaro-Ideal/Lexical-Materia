using UnityEngine;

[System.Serializable]
public class SoundData
{
    public AudioClip clip;
    public SoundName soundName;
}
public enum SoundName
{
    MenuOpen,
    MenuClose,
    ItemPickUp,
}
