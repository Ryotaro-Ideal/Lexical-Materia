using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    [Header("Audio Sources")]
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private List<SoundData> soundDatas;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Instance = this;

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void PlaySE(SoundName soundName)
    {
        foreach (var soundData in soundDatas)
        {
            if (soundData.soundName == soundName)
            {
                audioSource.PlayOneShot(soundData.clip);
                return;
            }
        }
    }
}
