using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FlashModifier : MonoBehaviour
{
    public Transform Element;
    public float Rate;
    private float _timer;
    private Vector3 _invisibleScale = Vector3.zero;
    private bool _visible = true;
    private Vector3 _originScale;

    // Start is called before the first frame update
    void Start()
    {
        _timer = 0;
        _originScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        _timer += 1.0f * Time.unscaledDeltaTime;
        if (_timer >= Rate)
        {
            _visible = !_visible;
            _timer = 0;
        }

        Element.localScale = _visible ? _originScale : _invisibleScale; 
    }
}
