using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public interface IInstanceObject
{
    public int MaxInstancesAlive { get; set; }
    public float SpawnRate { get; set; }
    public GameObject GameObject { get; }
    public IInstanceObject Prefab { get; }
    public int OwnerID { get; }
    public IInstanceObject SetInstance( int ownerID);

    public void Spawn(Vector3 position, Vector3 dir);
    public void Death();
    public void DeSpawn();

}