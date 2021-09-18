using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
[CustomPropertyDrawer(typeof(ActorData))]
public class ActorDataDrawer : PropertyDrawer
{
    private List<SerializedProperty> _dataFields;
    private FieldInfo[] _fields;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var rect = position;
        rect.height = 18;

        EditorGUI.BeginProperty(position, label, property);
        EditorGUI.PropertyField(rect, property, new GUIContent(property.displayName), true);
        var data = property.objectReferenceValue as ActorData;
        if (data != null)
        {
            var pos = rect;
            var obj = new SerializedObject(data, property.objectReferenceValue);
            obj.Update();
            _fields = data.GetType().GetFields();
            if (_dataFields == null || _fields.Length != _dataFields.Count)
            {
                _dataFields = new List<SerializedProperty>();
                for (int i = 0; i < _fields.Length; i++)
                {
                    _dataFields.Add(obj.FindProperty(_fields[i].Name));
                    SerializedProperty sp = _dataFields.Last();
                    pos.y += pos.height * 2.5f;
                    EditorGUI.PropertyField(pos, sp, new GUIContent(_dataFields[i].displayName), true);
                }
            }
            else
            {
                for (int i = 0; i < _dataFields.Count; i++)
                {
                    EditorGUI.BeginChangeCheck();
                    SerializedProperty sp = _dataFields[i];
                    pos.y += pos.height * 2.5f;
                    EditorGUI.PropertyField(pos, sp, new GUIContent(_dataFields[i].displayName), true);
                    if (EditorGUI.EndChangeCheck())
                    {
                        UpdateFieldValue(_fields[i], data, sp);
                    }
                }
            }
        }

        EditorGUI.EndProperty();
    }
    private void UpdateFieldValue(FieldInfo field, object obj, SerializedProperty property)
    {
        switch (property.propertyType)
        {
            case SerializedPropertyType.Boolean:
                field.SetValue(obj, property.boolValue);
                break;
            case SerializedPropertyType.Integer:
                field.SetValue(obj, property.intValue);
                break;
            case SerializedPropertyType.ObjectReference:
                field.SetValue(obj, property.objectReferenceValue);
                break;
            default:
                field.SetValue(obj, property.floatValue);
                break;
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = base.GetPropertyHeight(property, label);
        return _fields != null ? height * 2.8f * _fields.Length : height;
    }
}
