using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Events
{
    public class DamagedEventArgs : EventArgs
    {
        public int Damage { get; set; }

        public DamagedEventArgs(int damage)
        {
            Damage = damage;
        }
    }

    public class GameEvents : MonoBehaviour
    {
        public event EventHandler<DamagedEventArgs> Damaged;
        public static GameEvents Instance
        {
            get
            {
                if (_gameEvents == null)
                {
                    _gameEvents = new GameObject("GameEvents").AddComponent<GameEvents>();
                }

                return _gameEvents;
            }
        }
        private static GameEvents _gameEvents;
        public void OnDamaged(DamagedEventArgs e)
        {
            EventHandler<DamagedEventArgs> handler = Damaged;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}