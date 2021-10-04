using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class AI_Mover : MonoBehaviour
{
    public float Speed;
    [Range(0, 999)]
    public float MoveRange;
    [Range(0, 1)]
    public float StartPoint;

    private Object3D _object3D;
    private KinematicObject3D _kinematicObject3D;
    private Vector3 _moveMin;
    private Vector3 _moveMax;
    private Vector3 _move;

    private void Awake()
    {
        _object3D = GetComponent<Object3D>();
        if (_object3D is KinematicObject3D)
            _kinematicObject3D = _object3D as KinematicObject3D;

        InitMoveRange();
    }

    private void Start()
    {
        if(MoveRange > 0)
            transform.position = Vector3.Lerp(_moveMin, _moveMax, StartPoint);
        
        _move = new Vector3(Speed * (int)_object3D.Direction, 0, 0);
    }

    private void InitMoveRange()
    {
        if (MoveRange > 0)
        {
            _moveMin = transform.position - new Vector3(MoveRange, 0, 0);
            _moveMax = transform.position + new Vector3(MoveRange, 0, 0);
        }
    }

    private void Update()
    {
        if (_kinematicObject3D != null)
            KinematicUpdate();
        else
            ObjectUpdate();
    }
        
    private void ObjectUpdate()
    {
        var move = GetMoveDirection();
        _object3D.Move(move);
    }

    private Vector3 GetMoveDirection()
    {
        var pos = transform.position;

        if(MoveRange > 0)
        {
            if (pos.x <= _moveMin.x)
                _object3D.Direction = Direction.RIGHT;
            else if (pos.x >= _moveMax.x)
                _object3D.Direction = Direction.LEFT;
        }
        else
        {
            if (CheckForWall())
                _object3D.FlipDirection();
        }

        return _object3D.Direction == Direction.RIGHT ? new Vector3(Speed, 0, 0) : new Vector3(-Speed, 0, 0);
    }

    private bool CheckForWall()
    {
        var check = Physics.BoxCast(transform.position, _object3D.CollisionBounds * 0.5f, _object3D.GetDirectionVector(), transform.rotation, 0.1f, 1 << 0);
        return check;
    }

    private void KinematicUpdate()
    {

    }

#if UNITY_EDITOR
    public void OnDrawGizmos()
    {
        if(!Application.isPlaying)
            InitMoveRange();

        if(MoveRange > 0)
        {
            Handles.color = Color.blue;
            if (Selection.activeGameObject == gameObject)
                Handles.color = Color.red;

            Handles.DrawLine(_moveMin, _moveMax);
            Vector3 start = Vector3.Lerp(_moveMin, _moveMax, StartPoint);
            Handles.DrawWireCube(start, new Vector3(1, 1, 1));
        }
    }
#endif
}
