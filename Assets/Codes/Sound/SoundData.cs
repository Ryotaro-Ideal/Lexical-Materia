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
    ItemPickUp,
    ItemMove,
    Click,
}
