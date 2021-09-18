using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEditor;
using UnityEngine;

[InitializeOnLoadAttribute]
public static class HierarchyMonitor
{
    private static int _previousCount;
    static HierarchyMonitor()
    {
        EditorApplication.hierarchyChanged += OnHierarchyChanged;
    }

    private static bool SingleInstancePlayer(GameObject[] players)
    {
        if (players != null && players.Length > 1)
        {
            var player = players[0];
            for (int i = players.Length - 1; i > 0; i--)
            {
                player.transform.position = players[i].transform.position;
                GameObject.DestroyImmediate(players[i]);
            }

            return true;
        }

        return false;
    }

    static void OnHierarchyChanged()
    {
        // Get only the player objects that have been placed in the scene.
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player").Where(p => (p.hideFlags & HideFlags.HideInHierarchy) != HideFlags.HideInHierarchy).ToArray();
        if (players.Length - _previousCount != 0)
        {
            if(SingleInstancePlayer(players))
            {
                Debug.LogFormat($"There are currently {players.Length} Player objects in this scene. Only 1 Player object can exist in a scene.");
            }
        }
        _previousCount = players.Length;
    }
}