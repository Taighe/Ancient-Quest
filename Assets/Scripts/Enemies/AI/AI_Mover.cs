using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameAI
{
    public class AI_Mover : MonoBehaviour
    {
        public float Speed;
        public Transform Min;
        public Transform Max;
        [Range(0, 999)]
        [HideInInspector]
        public float MoveRange;
        [Range(0, 1)]
        [HideInInspector]
        public float StartPoint = 0.5f;

        private Object3D _object3D;
        private KinematicObject3D _kinematicObject3D;
        private Vector3 _move;
        private Vector3 _min;
        private Vector3 _max;

        private void Awake()
        {
            _object3D = GetComponent<Object3D>();
            if (_object3D is KinematicObject3D)
                _kinematicObject3D = _object3D as KinematicObject3D;

            InitEditorValues();
        }

        void InitEditorValues()
        {
            _min = Min.position;
            _max = Max.position;
            MoveRange = Vector3.Distance(_min, _max);
        }

        public void InitMoveRange()
        {
            if(Min == null)
            {
                var min = new GameObject("Min");
                min.transform.position = Vector3.zero;
                min.transform.SetParent(transform);
                Min = min.transform;
            }

            if (Max == null)
            {
                var max = new GameObject("Max");
                max.transform.position = Vector3.zero;
                max.transform.SetParent(transform);
                Max = max.transform;
            }
        }

        private void Start()
        {
            if (MoveRange > 0)
                transform.position = Vector3.Lerp(_min, _max, StartPoint);

            _move = new Vector3(Speed * (int)_object3D.Direction, 0, 0);
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

            if (MoveRange > 0)
            {
                if (pos.x <= _min.x)
                    _object3D.Direction = Direction.RIGHT;
                else if (pos.x >= _max.x)
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

        public Vector3 GetStartPoint()
        {
            return Vector3.Lerp(_min, _max, StartPoint);
        }

    }
}
