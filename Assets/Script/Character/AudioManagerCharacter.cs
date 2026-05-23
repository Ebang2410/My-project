using System.Collections.Generic;
using UnityEngine;

public class AudioManagerCharacter : MonoBehaviour
{
    [Header("Audio")]
    public List<AudioClip> snFoot = new List<AudioClip>();
    public List<AudioClip> snVoix = new List<AudioClip>();
    int idSnFoot;

    [SerializeField] List<AudioSource> audioSources = new List<AudioSource>();
    int idAudioSource ;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        idSnFoot =0 ;
        idAudioSource = 0; 
    }

    public void SnFootPlay()
    {
        if (snFoot.Count == 0) return;

        if (idSnFoot < snFoot.Count)
        {
            audioSources[idAudioSource].PlayOneShot(snFoot[idSnFoot]);
            idAudioSource ++;
            idSnFoot++;
        }
        else
        {
            idSnFoot = 0;
            SnFootPlay();
        }
    }

}
