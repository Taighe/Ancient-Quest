using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(LevelProperties), true)]
public class LevelPropertiesEditor : Editor 
{
    [InitializeOnEnterPlayMode]
    static void InitializeOnPlay()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
            return;

        var instance = LevelProperties.GetInstance("Level");
        instance.Data.PreviousAudioTime = 0;
        instance.Data.PreviousBackgroundMusic = null;
    }
}