using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class LevelProperties : SingletonObject<LevelProperties>
{
    public LevelPropertiesData Data;
    public AudioClip BackgroundMusic;
    public float FadeOutTime = 0.3f;
    private AudioSource _audSource;
    
    // Start is called before the first frame update
    public void Start()
    {
        if (Data == null)
        {
            Data = new LevelPropertiesData();
        }

        _audSource = GetComponent<AudioSource>();
        _audSource.clip = BackgroundMusic;
         
        if (_audSource.clip != null)
        {
            if (string.IsNullOrEmpty(Data.PreviousBackgroundMusic) || _audSource.clip.name != Data.PreviousBackgroundMusic)
            {
                _audSource.time = 0;
            }
            else
            {
                _audSource.time = Data.PreviousAudioTime;
            }

            _audSource.Play();
            Data.PreviousBackgroundMusic = _audSource.clip.name;
            Data.PreviousAudioTime = _audSource.time;
        }
        else
        {
            Data.PreviousBackgroundMusic = null;       
        }
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {        
        StartCoroutine(GameUI.GetInstance().TransitionFade(FadeOutTime, true));
        while (!GameUI.GetInstance().IsTransitionDone)
        {
            yield return new WaitForEndOfFrame();
        }

        Time.timeScale = 1;
    }

    public void WarpLevelProperties()
    {
        if(_audSource.clip != null)
        {
            Data.PreviousBackgroundMusic = _audSource.clip.name;
            Data.PreviousAudioTime = _audSource.time;
        }
    }
}
