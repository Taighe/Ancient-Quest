using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class SkyScroll : MonoBehaviour
{
    public bool AutoScroll;
    public Vector2 ScrollSpeed;
    private Vector2 _offset;
    private MeshFilter _filter;
    private MeshRenderer _render;
    private Vector3 _lastPosition;

    void Start()
    {
        _filter = GetComponent<MeshFilter>();
        _render = GetComponent<MeshRenderer>();
        _lastPosition = FollowCamera.GetInstance().transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        var pos = FollowCamera.GetInstance().transform.position;

        if (AutoScroll)
            _offset += ScrollSpeed * Time.deltaTime;
        else
        {
            var diff = _lastPosition - pos;
            _offset += new Vector2(diff.x * ScrollSpeed.x, diff.y * ScrollSpeed.y) * Time.deltaTime;
        }

        _render.material.SetTextureOffset("_MainTex", _offset);
        _lastPosition = pos;
    }
}
