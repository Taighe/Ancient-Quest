using Assets.Scripts.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct CheckpointData
{
    public string SceneName { get; set; }
    [NonSerialized]
    public Vector3 Position;
    public Direction Facing { get; set; }
}

[CreateAssetMenu(fileName = "GameData", menuName = "ScriptableObjects/GameData", order = 1)]
public class GameData : ScriptableObject
{   
    public int Lives = 3;
    public int Score = 0;
    public CheckpointData Checkpoint;
    public float RetryTransistionTime;
    public float RetryTransistionDelayTime;

    public int SaveSlot => _saveSlot;
    private int _saveSlot;
    [NonSerialized]
    private SaveData _loadedSaveData = null;

    public void LoadSaveData(int slot, SaveData saveData)
    {
        SetData(slot, saveData);
        _loadedSaveData = saveData;
    }

    public bool HasSaveDataLoaded(out SaveData saveData)
    {
        saveData = null;

        if (_loadedSaveData != null)
        {
            saveData = _loadedSaveData.Clone();
            _loadedSaveData = null;
        }

        return saveData != null;
    }

    public void SetData(int slot, SaveData saveData)
    {
        _saveSlot = slot;
        Lives = saveData.Lives;
        Score = saveData.Score;
        Checkpoint = new CheckpointData
        {
            SceneName = saveData.CheckpointSceneName,
            Facing = saveData.CheckpointFacing,
            Position = new Vector3(saveData.CheckpointX, saveData.CheckpointY, saveData.CheckpointZ),
        };
    }

    public void ResetToDefaults()
    {
        Lives = 2;
        Score = 0;
        Checkpoint = new CheckpointData
        {
            Facing = Direction.RIGHT,
            Position = new Vector3(3, 3),
            SceneName = "Level0x0"
        };
    }

    public SaveData GetData()
    {
        SaveData saveData = new SaveData()
        {
            Lives = Lives,
            Score = Score,
            CheckpointSceneName = Checkpoint.SceneName,
            CheckpointX = Checkpoint.Position.x,
            CheckpointY = Checkpoint.Position.y,
            CheckpointZ = Checkpoint.Position.z,
            CheckpointFacing = Checkpoint.Facing
        };

        return saveData;
    }
}
