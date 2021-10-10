using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Assets.Scripts.Events
{
    // Event Arguements
    public class DamagedEventArgs : EventArgs
    {
        public int Damage { get; set; }
        public GameObject Attacker { get; set; }
        public GameObject Defender { get; set; }

        public DamagedEventArgs(GameObject attacker, GameObject defender, int damage)
        {
            Damage = damage;
            Attacker = attacker;
            Defender = defender;
        }
    }

    public class ActiveEventArgs : EventArgs
    {
        public GameObject Instance { get; }

        public ActiveEventArgs(GameObject instance)
        {
            Instance = instance;
        }
    }

    public class CollectEventArgs : EventArgs
    {
        public Collectable Instance { get; }

        public CollectEventArgs(Collectable instance)
        {
            Instance = instance;
        }
    }

    public class DeathEventArgs : EventArgs
    {
        public IInstanceObject Instance { get; }

        public DeathEventArgs(IInstanceObject instance)
        {
            Instance = instance;
        }
    }
    //
    public class GameEvents : MonoBehaviour
    {
        private Dictionary<string, Delegate> _instanceEvents;
        private object objectLock = new object();
        // InstanceManager Events
        public event EventHandler<DeathEventArgs> InstanceManager_Death;

        // Player Events
        public event EventHandler<CollectEventArgs> Player_Collect;

        // Instance Events
        private event EventHandler<ActiveEventArgs> _active;
        private event EventHandler<ActiveEventArgs> _instActive;
        public event EventHandler<ActiveEventArgs> Active
        {
            add
            {
                lock (objectLock)
                {
                    AddInstanceEvent(value, nameof(Active));
                    _instActive += value;
                }
            }
            remove
            {
                lock (objectLock)
                {
                    RemoveInstanceEvent(value, nameof(Active));
                    _instActive -= value;
                }
            }
        }

        private event EventHandler<DamagedEventArgs> _damaged;
        private event EventHandler<DamagedEventArgs> _instDamaged;
        public event EventHandler<DamagedEventArgs> Damaged
        {
            add
            {
                lock(objectLock)
                {
                    AddInstanceEvent(value, nameof(Damaged));
                    _instDamaged += value;
                }
            }
            remove
            {
                lock (objectLock)
                {
                    RemoveInstanceEvent(value, nameof(Damaged));
                    _instDamaged -= value;
                }
            }
        }

        private event EventHandler<DamagedEventArgs> _hit;
        private event EventHandler<DamagedEventArgs> _instHit;
        public event EventHandler<DamagedEventArgs> Hit
        {
            add
            {
                lock (objectLock)
                {
                    AddInstanceEvent(value, nameof(Hit));
                    _instHit += value;
                }
            }
            remove
            {
                lock (objectLock)
                {
                    RemoveInstanceEvent(value, nameof(Hit));
                    _instHit -= value;
                }
            }
        }
        //
        private static GameEvents _gameEvents;
        public static GameEvents Instance
        {
            get
            {
                if (_gameEvents == null)
                {
                    _gameEvents = new GameObject("GameEvents").AddComponent<GameEvents>().Init();
                }

                return _gameEvents;
            }
        }

        private GameEvents Init()
        {
            _instanceEvents = new Dictionary<string, Delegate>();
            _damaged += GameEvents__damaged;
            _hit += GameEvents__hit;
            _active += GameEvents__active;
            return this;
        }

        private void GameEvents__active(object sender, ActiveEventArgs e)
        {
            if (_instActive != null && _instanceEvents != null)
            {
                InvokeInstanceEvent(GetInstanceEvent(e.Instance, nameof(Active)), sender, e);
            }
        }

        private void GameEvents__damaged(object sender, DamagedEventArgs e)
        {
            if (_instDamaged != null && _instanceEvents != null)
            {
                InvokeInstanceEvent(GetInstanceEvent(e.Defender, nameof(Damaged)), sender, e);
            }
        }

        private void GameEvents__hit(object sender, DamagedEventArgs e)
        {
            if (_instHit != null && _instanceEvents != null)
            {
                InvokeInstanceEvent(GetInstanceEvent(e.Attacker, nameof(Hit)), sender, e);
            }
        }

        public void HandleEvent<T>(EventHandler<T> eventHandler, T eventArgs)
        {
            EventHandler<T> handler = eventHandler;
            if (handler != null)
            {
                handler(this, eventArgs);
            }
        }
        /// <summary>
        /// Triggers the Damaged event for the defender listening to the event. 
        /// </summary>
        /// <param name="e"></param>
        public void OnDamaged(DamagedEventArgs e)
        {
            HandleEvent(_damaged, e);
        }
        /// <summary>
        /// Triggers the Hit event for the attacker listening to the event. 
        /// </summary>
        /// <param name="e"></param>
        public void OnHit(DamagedEventArgs e)
        {
            HandleEvent(_hit, e);
        }

        public void OnActive(ActiveEventArgs e)
        {
            HandleEvent(_active, e);
        }

        public void InstanceManager_OnDeath(DeathEventArgs e)
        {
            HandleEvent(InstanceManager_Death, e);
        }

        public void Player_OnCollect(CollectEventArgs e)
        {
            HandleEvent(Player_Collect, e);
        }

        private void AddInstanceEvent(Delegate eventHandler, string name)
        {
            var key = eventHandler.Target != null ? eventHandler.Target as MonoBehaviour : null;
            if (key == null)
                Debug.LogError("Cannot add event as instance event. Not a MonoBehaviour.");

            _instanceEvents.Add(GetInstanceEvent(key.gameObject, name), eventHandler);
        }

        private void RemoveInstanceEvent(Delegate eventHandler, string name)
        {
            var key = eventHandler.Target != null ? eventHandler.Target as MonoBehaviour : null;
            _instanceEvents.Remove(GetInstanceEvent(key.gameObject, name));
        }

        private void InvokeInstanceEvent(string instanceEventKey, object sender, EventArgs e)
        {
            if(_instanceEvents.ContainsKey(instanceEventKey))
            {
                var target = _instanceEvents[instanceEventKey].Target;
                _instanceEvents[instanceEventKey].GetMethodInfo().Invoke(target, new object[] { sender, e });
            }
        }

        private string GetInstanceEvent(GameObject gameObject, string eventName)
        {
            return $"{gameObject.GetInstanceID()}{eventName}";
        }
    }
}