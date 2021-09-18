using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Actor", menuName = "ScriptableObjects/Actor/PlayerData", order = 1)]
public class PlayerData : ActorData
{
    [Header("Sounds")]
    public AudioClip JumpSfx;

    public Vector3 WarpPoint { get{ return _warpPoint; } set { _warpPoint = value; } }
    private Vector3 _warpPoint;
    public Direction ExitDirection { get { return _exitDirection; } set { _exitDirection = value; } }
    private Direction _exitDirection;
    public bool IsWarping { get { return _isWarping; } set { _isWarping = value; } }
    private bool _isWarping;
}
