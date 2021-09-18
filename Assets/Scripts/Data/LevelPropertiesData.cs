using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelPropertiesData", menuName = "ScriptableObjects/Level/LevelPropertiesData", order = 1)]

public class LevelPropertiesData : ScriptableObject
{
    public string PreviousBackgroundMusic { get { return _previousBackgroundMusic; } set{ _previousBackgroundMusic = value; } }
    private string _previousBackgroundMusic;
    public float PreviousAudioTime { get { return _previousAudioTime; } set { _previousAudioTime = value; } }
    private float _previousAudioTime;
}
