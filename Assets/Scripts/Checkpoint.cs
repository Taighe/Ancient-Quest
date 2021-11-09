using Assets.Scripts.Globals;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Checkpoint : MonoBehaviour
{
    public Vector3 TriggerArea;
    public Transform SpawnPoint;
    public Direction Facing;

    public void Start()
    {

    }

    public void FixedUpdate()
    {
        Vector3 center = new Vector3(transform.position.x + TriggerArea.x * 0.5f, transform.position.y + TriggerArea.y * 0.5f, transform.position.z);
        var colliders = Physics.OverlapBox(center, TriggerArea * 0.5f, transform.rotation, (int)Layers.Player, QueryTriggerInteraction.Collide);
        if(colliders.Length > 0)
        {
            LevelProperties.GetInstance().UpdateGameDataCheckpoint(SpawnPoint.position, SceneManager.GetActiveScene().name, Facing);
        }
    }

#if UNITY_EDITOR
    public void OnDrawGizmos()
    {
        Handles.color = Color.blue;
        Vector3 center = new Vector3(transform.position.x + TriggerArea.x * 0.5f, transform.position.y + TriggerArea.y * 0.5f, transform.position.z);

        Handles.DrawWireCube(center, TriggerArea);
    }
#endif
}
