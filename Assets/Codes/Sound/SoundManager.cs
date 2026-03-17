using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-200)]
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    [Header("Audio Sources")]
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private List<SoundData> soundDatas;



    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
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
