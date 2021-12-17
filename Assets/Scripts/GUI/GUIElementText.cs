using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GUIElementText : GUIElement
{
    public TextMeshProUGUI Text;
    public List<string> PropertyBind;

    private string _text;

    public override void Start()
    {
        base.Start();
        _propInfo = GetBinding(PropertyBind);
        _text = Text.text;
        UpdateBinding();
    }

    public override void UpdateBinding()
    {
        if (_propInfo != null && _instance != null)
        {
            object val = _propInfo.GetValue(_instance);
            if (val is int)
                Text.text = _text + string.Format("{0,5:D8}", val);
            else
                Text.text = _text + val.ToString();
        }
        else
        {
            Text.text = "";
        }
    }
}
