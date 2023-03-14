using AQEngine.Globals;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using AQEngine.Level;

namespace AQEngine
{
    public class Checkpoint : MonoBehaviour
    {
        public Vector3 TriggerArea;
        public Transform SpawnPoint;
        public Direction Facing;

        private bool _activated;

        public void Start()
        {

        }

        public void FixedUpdate()
        {
            Vector3 center = new Vector3(transform.position.x + TriggerArea.x * 0.5f, transform.position.y + TriggerArea.y * 0.5f, transform.position.z);
            var colliders = Physics.OverlapBox(center, TriggerArea * 0.5f, transform.rotation, (int)Layers.Player, QueryTriggerInteraction.Collide);
            if (colliders.Length > 0 && !_activated)
            {
                LevelProperties.GetInstance().UpdateGameDataCheckpoint(SpawnPoint.position, SceneManager.GetActiveScene().name, Facing);
                _activated = true;
            }
            else if (colliders.Length <= 0)
                _activated = false;
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

}