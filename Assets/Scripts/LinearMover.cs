using AQEngine.Globals;
using AQEngine.Objects.KinematicObjects;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AQEngine
{
    public class LinearMover : MonoBehaviour
    {
        [Range(0, 100)]
        public float Speed;
        public float Width;
        public float Height;
        public float Distance;
        public Transform Point1;
        public Transform Point2;
        private float _time;
        Vector3 _min;
        Vector3 _max;
        Collider _collider;
        private Dictionary<GameObject, KinematicObject3D> _passengers;
        private void Awake()
        {
            _collider = GetComponent<Collider>();
        }

        private void Start()
        {
            _min = Point1.position;
            _max = Point2.position;
            _passengers = new Dictionary<GameObject, KinematicObject3D>();
        }

        public bool PointsInit()
        {
            if (Point1 == null)
            {
                Point1 = new GameObject("P1").GetComponent<Transform>();
                Point1.transform.parent = transform;
                Point1.transform.localPosition = Vector3.zero;
            }

            if (Point2 == null)
            {
                Point2 = new GameObject("P2").GetComponent<Transform>();
                Point2.transform.parent = transform;
                Point2.transform.localPosition = Vector3.zero;
            }

            return true;
        }

        public bool PointsReset()
        {
            Point1.transform.localPosition = Vector3.zero;
            Point2.transform.localPosition = Vector3.zero;
            return true;
        }

        private void PassengerUpdate()
        {
            RaycastHit[] hits = Physics.BoxCastAll(transform.position, new Vector3(Width, Height, 0), transform.up, transform.rotation,
                Distance, LayerHelper.LayerMask(Layers.Kinematic, Layers.Player));

            foreach (var p in _passengers.Values)
            {
                p.RemovePassenger();
            }

            foreach (var hit in hits)
            {
                if (!_passengers.ContainsKey(hit.collider.gameObject))
                {
                    var kinObj = hit.collider.gameObject.GetComponent<KinematicObject3D>();
                    if (kinObj != null)
                    {
                        _passengers.Add(hit.collider.gameObject, kinObj);
                    }
                }
                var passenger = _passengers[hit.collider.gameObject];
                passenger.AddAsPassenger(transform);
                float move = 0.001f;
                var m = new Vector3(move, 0);
                // Call this function to force collision detection for passenger
                passenger.SimpleMove(m);
                var dir = (_max - _min).normalized;
                if (dir.y != 0 && passenger.Velocity.y == 0)
                {
                    passenger.IsGrounded = true;
                }
            }
        }

        void FixedUpdate()
        {
            if (Speed == 0) return;
            // Passenger update logic
            PassengerUpdate();

            if (_time >= Speed)
            {
                var tMax = _max;
                _max = _min;
                _min = tMax;
                _time = 0;
            }

            transform.position = Vector3.Lerp(_min, _max, _time / Speed);
            var pos = Vector3.Lerp(_min, _max, _time / Speed);
            _time += 1.0f * Time.fixedDeltaTime;
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            Vector3 min = Point1.position;
            Vector3 max = Point2.position;

            if (Application.isPlaying)
            {
                min = _min;
                max = _max;
            }

            Handles.color = Color.red;
            Handles.DrawLine(min, max);
            Handles.color = Color.green;
            Handles.DrawWireCube(transform.position, new Vector3(Width + Distance, Height + Distance));
        }
#endif
    }
}
