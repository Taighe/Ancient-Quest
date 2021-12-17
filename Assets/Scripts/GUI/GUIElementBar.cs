using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class GUIElementBar : GUIElement
{
    public List<string> Value;
    public List<string> MaxValue;
    public float IconSize;
    public Image BackImage;
    public Image FrontImage;

    private PropertyInfo _maxValueInfo;

    public override void Start()
    {
        base.Start();
        _propInfo = GetBinding(Value);
        _maxValueInfo = GetBinding(MaxValue);
        UpdateBinding();
    }

    public override void UpdateBinding()
    {
        if (_propInfo != null && _maxValueInfo != null && _instance != null)
        {
            object val = _propInfo.GetValue(_instance);
            object max = _maxValueInfo.GetValue(_instance);

            if (val is int v && max is int m)
            {
                v = Mathf.Clamp(v, 0, m);
                FrontImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, IconSize * v);
                BackImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, IconSize * m);
                FrontImage.enabled = true;
                BackImage.enabled = true;
            }
        }
        else
        {
            FrontImage.enabled = false;
            BackImage.enabled = false;
        }
    }
}
