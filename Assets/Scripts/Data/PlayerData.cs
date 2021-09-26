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

    public int HP
    {
        get
        {
            return _hp;
        }
        set
        {
            _hp = Mathf.Clamp(value, 0, _maxHP);
        }
    }

    private int _hp = 3;
    public int MaxHP
    {
        get
        {
            return _maxHP < _hp ? _hp: _maxHP;
        }
        set
        {
            _maxHP = value >= _hp ? value : _maxHP;
        }
    }
    private int _maxHP = 3;
}
