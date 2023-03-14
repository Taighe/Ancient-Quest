using Assets.Scripts.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using UnityEngine;

namespace AQEngine.GUI
{
    public abstract class GUIElement : MonoBehaviour
    {
        public string DataType;

        protected object _instance;
        protected PropertyInfo _propInfo;

        // Start is called before the first frame update
        public virtual void Start()
        {

        }

        public PropertyInfo GetBinding(List<string> propertyBind)
        {
            Type t = Type.GetType(DataType);
            if (t.BaseType.Name.Contains("SingletonObject"))
            {
                object data = t.BaseType.GetMethod("GetInstanceInvoked", BindingFlags.Public | BindingFlags.Static).Invoke(null, null);
            }
            else
            {
                PropertyInfo dataInfo = t.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);
                _instance = dataInfo.GetValue(null);

                PropertyInfo property = null;
                Type dataType = _instance.GetType();
                object inst = _instance;
                foreach (var p in propertyBind)
                {
                    var args = p.Split(',');
                    int index = -1;
                    foreach (string arg in args)
                    {
                        index = int.TryParse(arg.Trim('[', ']'), out index) ? index : -1;
                    }

                    property = dataType.GetProperty(args[0]);

                    if (index >= 0)
                    {
                        var array = property.GetValue(inst) as Array;
                        inst = array.GetValue(index);
                    }
                    else
                    {
                        if (property != null && property.PropertyType.IsByRef)
                            inst = property.GetValue(inst);
                    }

                    if (inst != null)
                        dataType = inst.GetType();
                }

                _instance = inst;
                return property;
            }

            return null;
        }

        public abstract void UpdateBinding();
    }
}
