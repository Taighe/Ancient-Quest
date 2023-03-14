using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AQEngine.Objects.SpawnableObjects
{
    /// <summary>
    /// Interface defining contract of what a spawnable object requires.
    /// </summary>
    public interface ISpawnableObject
    {
        public int MaxInstancesAlive { get; set; }
        public int Strength { get; set; }
        public float SpawnRate { get; set; }
        public GameObject GameObject { get; }
        public ISpawnableObject Prefab { get; }
        public int OwnerID { get; }
        public ISpawnableObject SetInstance(int ownerID);

        public void Spawn(Vector3 position, Vector3 dir);
        public void Death();
        public void DeSpawn();
    }
}