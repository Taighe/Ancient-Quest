using Assets.Scripts.Globals;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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

    //public void OnCollisionEnter(Collision collision)
    //{
    //    if (!_passengers.Contains(collision.gameObject) && collision.gameObject.tag == "Player")
    //    {
    //        _passengers.Add(collision.gameObject);
    //        collision.transform.parent = transform;
    //        Debug.Log(_passengers.Count);
    //    }
    //}

    //private void OnCollisionExit(Collision collision)
    //{
    //    if (_passengers.Count > 0 && _passengers.Contains(collision.gameObject))
    //    {
    //        collision.transform.parent = null;
    //        _passengers.Remove(collision.gameObject);
    //        Debug.Log(_passengers.Count);
    //    }
    //}

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
        RaycastHit[] hits = Physics.BoxCastAll(transform.position, new Vector3(Width, Height, 0), transform.up, transform.rotation, Distance, 1 << (int)Layers.Kinematic);
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
            //var matrix1 = Matrix4x4.TRS(transform.localPosition, transform.localRotation, transform.localScale);
            //var matrix2 = Matrix4x4.TRS(passenger.transform.localPosition, passenger.transform.localRotation, passenger.transform.localScale);
            //var matrix3 = matrix1 * matrix2;
            //var pos = matrix1.MultiplyPoint(passenger.transform.localPosition);
            //var m2 = passenger.transform.worldToLocalMatrix;
            //var point = passenger.transform.position - new Vector3(matrix.m03, matrix.m13, matrix.m23);
            //passenger.transform.position = new Vector3(matrix.m03, matrix.m13, matrix.m23);
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

    void Update()
    {
        if (Speed == 0 ) return;
        // Passenger update logic
        PassengerUpdate();

        if(_time >= Speed)
        {
            var tMax = _max;
            _max = _min;
            _min = tMax;
            _time = 0;
        }

        transform.position = Vector3.Lerp(_min, _max, _time / Speed);
        var pos = Vector3.Lerp(_min, _max, _time / Speed);
        _time += 1.0f * Time.deltaTime;
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
