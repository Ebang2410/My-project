using System.Collections.Generic;
using UnityEngine;

public class AudioManagerCharacter : MonoBehaviour
{
    public GunScript gunScript;
    [Header("Audio")]
    public List<AudioClip> snFoot = new List<AudioClip>();
    public List<AudioClip> snVoix = new List<AudioClip>();
    int idSnFoot;

    [SerializeField] List<AudioSource> audioSources = new List<AudioSource>();
    int idAudioSource ;
    [Range(0,1)]    [SerializeField] float VsnRun;
    [Range(0,1)]    [SerializeField] float VsnWalk;
    [Range(0,1)]    [SerializeField] float VsnCrouch;
    public int ValueModeMove;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ValueModeMove = 0;
        idSnFoot =0 ;
        idAudioSource = 0; 

        if(audioSources.Count == 0 || snFoot.Count == 0 ) 
        {
            this.enabled = false;
        }

    }

    public void SnGun()
    {
        gunScript.SnGunPlay();
    }

    public void SnFootPlay()
    {
        float audioVol;
        if (snFoot.Count == 0) return;

        switch (ValueModeMove) // permet de connaitre le niv du volume de de Sfx foot 
        {
            case 0 :
                audioVol = VsnWalk;
            break;
            case 1 :
                audioVol = VsnCrouch;
            break;
            case 2:
                audioVol = VsnRun;
            break;
            
            default:
                audioVol = VsnWalk;
            break;
        }

        if(idAudioSource >= 2)
        {
            idAudioSource = 0;
        }

        if(idSnFoot >= snFoot.Count)
        {
            idSnFoot = 0;
        }

        audioSources[idAudioSource].PlayOneShot(snFoot[idSnFoot],audioVol);
        idAudioSource ++;        
        idSnFoot++;
    }

}
