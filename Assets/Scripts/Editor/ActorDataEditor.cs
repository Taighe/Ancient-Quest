using AQEngine.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UEditor = UnityEditor.Editor;

namespace AQEngine.Editor
{
    [CustomEditor(typeof(ActorData), true)]
    public class ActorDataEditor : UEditor { }
}
