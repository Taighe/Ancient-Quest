using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class SaveData
{
    public int HP { get; set; }
    public int MaxHP { get; set; }
    public int PowerUpMask { get; set; }
    public int Lives { get; set; }
    public int Score { get; set; }
    public string CheckpointSceneDisplayName { get; set; }
    public string CheckpointSceneName { get; set; }
    public Direction CheckpointFacing { get; set; }
    public float CheckpointX { get; set; }
    public float CheckpointY { get; set; }
    public float CheckpointZ { get; set; }
    public int SaveSlot { get; set; }

    public SaveData Clone()
    {
        return MemberwiseClone() as SaveData;
    }

    public SaveData Init()
    {
        return this;
    }
}
