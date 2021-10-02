using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Events
{
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

    public class GameEvents : MonoBehaviour
    {
        private event EventHandler<DamagedEventArgs> _damaged;
        public event EventHandler<DamagedEventArgs> Damaged;
        private static GameEvents _gameEvents;

        public static GameEvents Instance
        {
            get
            {
                if (_gameEvents == null)
                {
                    _gameEvents = new GameObject("GameEvents").AddComponent<GameEvents>();
                    _gameEvents._damaged += _gameEvents._gameEvents__damaged;
                }

                return _gameEvents;
            }
        }

        private void _gameEvents__damaged(object sender, DamagedEventArgs e)
        {
            var target = _gameEvents.Damaged.Target;
        }

        public void HandleEvent<T>(EventHandler<T> eventHandler, T eventArgs)
        {
            EventHandler<T> handler = eventHandler;
            if (handler != null)
            {
                handler(this, eventArgs);
            }
        }

        public void OnDamaged(DamagedEventArgs e)
        {
            //HandleEvent(Damaged, e);
        }
    }
}