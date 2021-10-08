using Assets.Scripts.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Scripts.Managers
{
    public class InstanceManager : MonoBehaviour
    {
        private static InstanceManager _manager;
        private Dictionary<string, IInstanceObject> _aliveInstanceCollection;
        private Dictionary<string, Queue<IInstanceObject>> _instanceCollection;
        public static InstanceManager Instance
        {
            get
            {
                if (_manager == null)
                {
                    _manager = new GameObject("InstanceManager").AddComponent<InstanceManager>().Init();
                }

                return _manager;
            }
        }

        private InstanceManager Init()
        {
            transform.position = new Vector3(-1000, 0, 0);
            _instanceCollection = new Dictionary<string, Queue<IInstanceObject>>();
            _aliveInstanceCollection = new Dictionary<string, IInstanceObject>();
            GameEvents.Instance.InstanceManager_Death += Instance_InstanceManager_Death;
            return this;
        }

        private void Instance_InstanceManager_Death(object sender, DeathEventArgs e)
        {
            var inst = e.Instance;
            CycleInstance(inst.OwnerID, inst.Prefab.GameObject.GetInstanceID(), inst.GameObject.GetInstanceID());
        }

        public void CycleInstance(int ownerID, int prefabID, int instanceID)
        {
            var key = $"{ownerID}{instanceID}";
            var prefabKey = $"{ownerID}{prefabID}";

            if (_aliveInstanceCollection.Count() > 0)
            {
                var inst = _aliveInstanceCollection[key];
                _aliveInstanceCollection.Remove(key);
                _instanceCollection[prefabKey].Enqueue(inst);
            }
        }

        public void AddInstancePrefabs(int ownerInstanceId, IEnumerable<GameObject> prefabs)
        {
            foreach (var prefab in prefabs)
            {
                IInstanceObject instancePrefab= prefab.GetComponent<IInstanceObject>();
                if (instancePrefab == null)
                    throw new Exception($"{nameof(InstanceManager)} tried to add a prefab that does not inherit from {nameof(IInstanceObject)}.");

                var key = $"{ownerInstanceId}{instancePrefab.GameObject.GetInstanceID()}";
                _instanceCollection.Add(key, new Queue<IInstanceObject>());
                // Clone the number of required instances.
                for (int i = 0; i < instancePrefab.MaxInstancesAlive; i++)
                {
                    var inst = instancePrefab.SetInstance( ownerInstanceId);
                    inst.GameObject.SetActive(false);
                    _instanceCollection[key].Enqueue(inst);
                }
            }
        }

        public IInstanceObject SpawnInstance(int ownerInstanceId, GameObject prefab, Vector3 origin, Vector3 dir)
        {
            var key = $"{ownerInstanceId}{prefab.GetInstanceID()}";
            if (_instanceCollection[key].Any())
            {
                var inst = _instanceCollection[key].Dequeue();
                _aliveInstanceCollection.Add($"{ownerInstanceId}{inst.GameObject.GetInstanceID()}", inst);
                inst.Spawn(origin, dir);
                return inst;
            }

            return null;
        }
    }
}
