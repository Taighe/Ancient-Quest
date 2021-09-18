using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelProperties), true)]
public class LevelPropertiesEditor : Editor 
{
    [InitializeOnEnterPlayMode]
    static void InitializeOnPlay()
    {
        var instance = LevelProperties.GetInstance("Level");
        instance.Data.PreviousAudioTime = 0;
        instance.Data.PreviousBackgroundMusic = null;
    }
}