using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayAudio : MonoBehaviour
{
    [SerializeField]
    bool playAwake = false;
    [SerializeField]  
    List<AudioClip> audios = new List<AudioClip>();
    AudioSource     audioSource;

    bool _canChange = true;

    private void Awake() 
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.mute = true;
    }

    public void AddAudioClip(AudioClip audio,bool clearList=false,bool play=false)
    {
        if(clearList)
            audios.Clear();

        audios.Add(audio);

        if(play)
            PlayAudioClip(audios.Count-1);
    }

    public void PlayAudioClip(int index=-1)
    {  

        if(index == -1)
            index = 0;

        if(index < 0 || index > audios.Count)
        {
            Debug.LogError("PlayAudioClip("+index+") - Erro Index not fould");
            return;
        }

        if(!_canChange && audioSource.isPlaying && audioSource.clip != audios[index])
            return;
        else
            _canChange = true;       

        if(audioSource.mute && playAwake)
            audioSource.mute = false;
        else
            playAwake = true;

        audioSource.clip = audios[index];

        audioSource.Play();       
    }

    public void PlayAudioClip(int index=-1,bool canChange=true)
    {    
        if(index == -1)
            index = 0;

        if(index < 0 || index > audios.Count)
        {
            Debug.LogError("PlayAudioClip("+index+") - Erro Index not fould");
            return;
        }

        if(!_canChange && audioSource.isPlaying && audioSource.clip != audios[index])
            return;
        else
            _canChange = true;

        if(audioSource.mute && playAwake)
            audioSource.mute = false;
        else
            playAwake = true;

        audioSource.clip = audios[index];

        audioSource.Play();  

        _canChange = canChange;
    }

}
