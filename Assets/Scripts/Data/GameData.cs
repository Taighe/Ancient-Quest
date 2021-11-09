using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CheckpointData
{
    public string SceneName;
    public Vector3 Position;
    public Direction Facing;
}

[CreateAssetMenu(fileName = "GameData", menuName = "ScriptableObjects/GameData", order = 1)]
public class GameData : ScriptableObject
{   
    public int Lives = 3;
    public int Score = 0;
    public CheckpointData Checkpoint;
    public float RetryTransistionTime;
    public float RetryTransistionDelayTime;
}
