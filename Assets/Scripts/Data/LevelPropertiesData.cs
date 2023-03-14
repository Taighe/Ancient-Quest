using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AQEngine.Data
{
    [CreateAssetMenu(fileName = "LevelPropertiesData", menuName = "ScriptableObjects/Level/LevelPropertiesData", order = 1)]
    public class LevelPropertiesData : ScriptableObject
    {
        public string PreviousBackgroundMusic { get { return _previousBackgroundMusic; } set { _previousBackgroundMusic = value; } }
        private string _previousBackgroundMusic;
        public float PreviousAudioTime { get { return _previousAudioTime; } set { _previousAudioTime = value; } }
        private float _previousAudioTime;

        public bool UpdateCheckpoint { get { return _updateCheckpoint; } set { _updateCheckpoint = value; } }
        private bool _updateCheckpoint;
        public Dictionary<string, bool> PersistantCollectables { get { return _persistantCollectables; } set { _persistantCollectables = value; } }
        private Dictionary<string, bool> _persistantCollectables;
    }
}
