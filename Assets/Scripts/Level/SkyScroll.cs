using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class SkyScroll : MonoBehaviour
{
    public Vector2 ScrollSpeed;
    private Vector2 _offset;
    private MeshFilter _filter;
    private MeshRenderer _render;

    void Start()
    {
        _filter = GetComponent<MeshFilter>();
        _render = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        _offset = new Vector2(_offset.x + ScrollSpeed.x * Time.deltaTime, _offset.y + ScrollSpeed.y * Time.deltaTime);
        _render.material.SetTextureOffset("_MainTex", _offset);
    }
}
