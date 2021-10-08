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

        // Instance Events
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
            _damaged += _gameEvents__damaged;
            _hit += _gameEvents__hit;
            return this;
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

        public void InstanceManager_OnDeath(object sender, DeathEventArgs e)
        {
            HandleEvent(InstanceManager_Death, e);
        }

        private void _gameEvents__hit(object sender, DamagedEventArgs e)
        {
            if (_instHit != null && _instanceEvents != null)
            {
                InvokeInstanceEvent(GetInstanceEvent(e.Attacker, nameof(Hit)), sender, e );
            }
        }

        private void _gameEvents__damaged(object sender, DamagedEventArgs e)
        {
            if(_instDamaged != null && _instanceEvents != null)
            {
                InvokeInstanceEvent(GetInstanceEvent(e.Defender, nameof(Damaged)), sender, e);
            }
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
            var target = _instanceEvents[instanceEventKey].Target;
            _instanceEvents[instanceEventKey].GetMethodInfo().Invoke(target, new object[] { sender, e });
        }

        private string GetInstanceEvent(GameObject gameObject, string eventName)
        {
            return $"{gameObject.GetInstanceID()}{eventName}";
        }
    }
}