using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "ScriptableObjects/GameData", order = 1)]

public class GameData : ScriptableObject
{   
    public int Lives = 3;
    public int Score = 0;
    public Vector3 Checkpoint;

}
