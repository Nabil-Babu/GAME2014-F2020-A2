﻿/**
    SoundManager.cs
    Nabil Babu
    101214336
    Dec 13th 2020
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 
[System.Serializable]
public class Sound
{
    public string soundName;
    public AudioClip audioClip;
}
public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    public string[] playSoundName;

    [Header("SFX")]
    public AudioSource[] audioSourceSFX;
    public Sound[] sfxSounds;

    [Header("BGM")]
    public AudioSource audioSourceBGM;
    public Sound[] bgmSounds; // index 0: Game Scene BGM
    [SerializeField]
    private string sceneName; 
    private void Awake()
    {
        //singleton
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        Debug.Log("OnEnable called");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

     public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        sceneName = scene.name;
        if(sceneName == "GamePlayScene" || sceneName == "GameOverScene")
        {
            if(audioSourceBGM.isPlaying)
            {
                audioSourceBGM.Stop();
                PlayBGM();
            } 
        }
    }
    void Start()
    {
        Debug.Log("Start called");
        playSoundName = new string[audioSourceSFX.Length];
        PlayBGM();
    }
    public void PLaySE(string name)
    {
        for(int i = 0; i < sfxSounds.Length; ++i)
        {
            if(name == sfxSounds[i].soundName)
            {
                for(int j = 0; j < audioSourceSFX.Length; ++j)
                {
                    if (!audioSourceSFX[j].isPlaying)
                    {
                        playSoundName[j] = sfxSounds[i].soundName;
                        audioSourceSFX[j].clip = sfxSounds[i].audioClip;
                        audioSourceSFX[j].Play();

                        return;
                    }
                }
                return;
            }
        }
    }
    public void PlayBGM()
    {
        
        switch(sceneName)
        {
            case "GamePlayScene":
                audioSourceBGM.clip = bgmSounds[0].audioClip;
                audioSourceBGM.volume = 0.5f;
                audioSourceBGM.Play();
                break;
            case "GameOverScene":
                audioSourceBGM.clip = bgmSounds[1].audioClip;
                audioSourceBGM.volume = 1.0f;
                audioSourceBGM.Play();
                break;
            default:
                audioSourceBGM.clip = bgmSounds[0].audioClip;
                audioSourceBGM.volume = 0.5f;
                audioSourceBGM.Play();
                break;
        }
    
    }
    public void StopAllSE()
    {
        for(int i = 0; i < audioSourceSFX.Length; ++i)
        {
            audioSourceSFX[i].Stop();
        }
    }
    public void StopSE(string name)
    {
        for (int i = 0; i < audioSourceSFX.Length; ++i)
        {
            if(playSoundName[i] == name)
            {
                audioSourceSFX[i].Stop();
                break;
            }
        }
    }

   
}