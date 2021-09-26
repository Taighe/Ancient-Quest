using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Player), true)]
public class PlayerEditor : Editor 
{
    [InitializeOnEnterPlayMode]
    static void InitializeOnPlay()
    {
        var obj = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        // Init Player variables
        obj.SetHP(obj.HP);
        obj.SetMaxHP(obj.MaxHP);
    }
}