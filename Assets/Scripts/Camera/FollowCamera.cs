using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class FollowCamera : SingletonObject<FollowCamera>
{
    public Object3D Player;
    public LevelBounds CameraBounds;
    public Vector2 Offset;
    public Camera Camera
    {
        get
        {
            return _camera == null ? GetComponent<Camera>() : _camera;
        }
    }
    private Camera _camera;
    private Vector2 _previousPos;
    private float _cornerDistX;
    private float _cornerDistY;
    private Vector3 _leftEdge;
    private Vector3 _rightEdge;
    private Vector3 _topEdge;
    private Vector3 _bottomEdge;

    // Start is called before the first frame update
    public override void Awake()
    {
        base.Awake();
        _camera = GetComponent<Camera>();
        _previousPos = transform.position;
        Vector3[] frustumCorners = new Vector3[4];
        float farClip = Mathf.Abs(_camera.transform.position.z);
        _camera.CalculateFrustumCorners(new Rect(0, 0, 1, 1), farClip, Camera.MonoOrStereoscopicEye.Mono, frustumCorners);
        _cornerDistX = Mathf.Abs(frustumCorners[0].x);
        _cornerDistY = Mathf.Abs(frustumCorners[0].y);
    }

    // Update is called once per frame
    void Update()
    {
        if (!Player.IsAlive)
            return;

        Vector3 pos = transform.position;
        Vector3 newPos = pos;
        float zPos = transform.position.z;
        if (Player != null)
        {
            Vector3 pPos = Player.transform.position + new Vector3(Offset.x, Offset.y, 0);
            newPos = pPos;
        }

        if(CameraBounds != null)
        {
            _leftEdge = new Vector3(-_cornerDistX, 0) + newPos;
            
            if (_leftEdge.x <= CameraBounds.Min.x)
            {
                float d = Vector3.Distance(new Vector3(_leftEdge.x, newPos.y, newPos.z), new Vector3(CameraBounds.Min.x, newPos.y, newPos.z));
                newPos.x += d;
            }

            _rightEdge = new Vector3(_cornerDistX, 0) + newPos;

            if (_rightEdge.x >= CameraBounds.Width)
            {
                float d = Vector2.Distance(new Vector3(_rightEdge.x, newPos.y, newPos.z), new Vector3(CameraBounds.Width, newPos.y, newPos.z));
                newPos.x -= d;
            }
            
            _topEdge = new Vector3(0, _cornerDistY) + newPos;

            if (_topEdge.y >= CameraBounds.Height)
            {
                float d = Vector3.Distance(new Vector3(newPos.x, _topEdge.y, newPos.z), new Vector3(newPos.x, CameraBounds.Height, newPos.z));
                newPos.y -= d;
            }
            
            _bottomEdge = new Vector3(0, -_cornerDistY) + newPos;
            
            if (_bottomEdge.y <= CameraBounds.Min.y)
            {
                float d = Vector3.Distance(new Vector3(newPos.x, _bottomEdge.y, newPos.z), new Vector3(newPos.x, CameraBounds.Min.y, newPos.z));
                newPos.y += d;
            }
        }

        transform.position = new Vector3(newPos.x, newPos.y, zPos);
        _previousPos = pos;
    }

#if UNITY_EDITOR

    void OnDrawGizmos()
    {
        Vector3 pos = transform.position;
        Handles.color = Color.green;
        Vector3[] frustumCorners = new Vector3[4];
        Camera cam = GetComponent<Camera>();
        float farClip = cam.farClipPlane;
        cam.CalculateFrustumCorners(new Rect(0, 0, 1, 1), farClip, Camera.MonoOrStereoscopicEye.Mono, frustumCorners);
        float cornerDistX = Mathf.Abs(frustumCorners[0].x);
        float cornerDistY = Mathf.Abs(frustumCorners[0].y);
        Vector3 leftDir = new Vector3(-cornerDistX, 0, farClip) + transform.position;
        Vector3 rightDir = new Vector3(cornerDistX, 0, farClip) + transform.position;
        Vector3 topDir = new Vector3(0, cornerDistY, farClip) + transform.position;
        Vector3 bottomDir = new Vector3(0, -cornerDistY, farClip) + transform.position;

        Handles.DrawLine(pos, leftDir);
        Handles.DrawLine(pos, rightDir);
        Handles.DrawLine(pos, topDir);
        Handles.DrawLine(pos, bottomDir);
    }
#endif
    void Foo()
    {
        Vector3 pos = transform.position;
        Vector3 newPos = pos;
        float zDistance = transform.position.z;
        if (Player != null)
        {
            Vector3 pPos = Player.transform.position;
            newPos = new Vector3(pPos.x, pPos.y, pPos.z);
        }

        if (CameraBounds != null)
        {

            float z = 0;
            Vector3 diff = pos - _camera.ViewportToWorldPoint(new Vector3(0, 0.5f, z));
            Vector3 leftEdge = newPos - new Vector3(diff.x, 0);
            if (leftEdge.x <= CameraBounds.Min.x)
            {
                float d = Vector3.Distance(new Vector3(leftEdge.x, newPos.y, newPos.z), new Vector3(CameraBounds.Min.x, newPos.y, newPos.z));
                newPos.x += d;
            }
            diff = pos - _camera.ViewportToWorldPoint(new Vector3(1, 0.5f, z));
            Vector3 rightEdge = newPos - new Vector3(diff.x, 0);
            if (rightEdge.x >= CameraBounds.Width)
            {
                float d = Vector2.Distance(new Vector3(rightEdge.x, newPos.y, newPos.z), new Vector3(CameraBounds.Width, newPos.y, newPos.z));
                newPos.x -= d;
            }

            diff = pos - _camera.ViewportToWorldPoint(new Vector3(0.5f, 1, z));
            Vector3 topEdge = newPos - new Vector3(0, diff.y);
            if (topEdge.y >= CameraBounds.Height)
            {
                float d = Vector3.Distance(new Vector3(newPos.x, topEdge.y, newPos.z), new Vector3(newPos.x, CameraBounds.Height, newPos.z));
                newPos.y -= d;
            }

            diff = pos - _camera.ViewportToWorldPoint(new Vector3(0.5f, 0, z));
            Vector2 bottomEdge = new Vector3(newPos.x, newPos.y, newPos.z) - new Vector3(0, diff.y);
            if (bottomEdge.y <= CameraBounds.Min.y)
            {
                float d = Vector3.Distance(new Vector3(newPos.x, bottomEdge.y, newPos.z), new Vector3(newPos.x, CameraBounds.Min.y, newPos.z));
                newPos.y += d;
            }
        }

        transform.position = new Vector3(newPos.x, newPos.y, zDistance);
        _previousPos = pos;
    }
}
