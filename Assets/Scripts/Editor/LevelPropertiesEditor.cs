using AQEngine.Level;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UEditor = UnityEditor.Editor;

namespace AQEngine.Editor
{
    [CustomEditor(typeof(LevelProperties), true)]
    public class LevelPropertiesEditor : UEditor
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
}