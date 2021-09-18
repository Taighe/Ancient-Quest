using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Player), true)]
public class PlayerEditor : Editor 
{
    //private SerializedProperty _instantiated;
    //public void Awake()
    //{
    //    _instantiated = serializedObject.FindProperty("Instantiated");
    //    if (_instantiated.boolValue == false && PrefabUtility.IsPartOfPrefabInstance(serializedObject.targetObject))
    //    {
    //        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

    //        if (players != null || players.Length > 1)
    //        {
    //            var player = players[0];
    //            for (int i = players.Length - 1; i > 0; i--)
    //            {
    //                player.transform.position = players[i].transform.position;
    //                Destroy(players[i]);
    //            }
    //        }

    //        _instantiated.boolValue = true;
    //    }
    //}
}