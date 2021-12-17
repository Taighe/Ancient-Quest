using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonObject<T> : MonoBehaviour
{
    protected static T _ref;
    public virtual void Awake()
    {
        _ref = GetComponent<T>();
    }

    public static T GetInstance() 
    {
        return _ref;
    }

    public static T GetInstanceInvoked()
    {
        return _ref;
    }

    public static T GetInstance(string gameObjectName)
    {
#if UNITY_EDITOR
        if (_ref == null)
        {
            var obj = GameObject.Find(gameObjectName);
            _ref = obj != null ? obj.GetComponent<T>() : default;

            if (_ref == null)
                Debug.LogError($"Could not find {nameof(T)}.");
        }
#endif
        return GetInstance();
    }
}
