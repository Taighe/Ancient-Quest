using AQEngine.Animators;
using AQEngine.Events;
using AQEngine.Objects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AQEngine 
{
    [RequireComponent(typeof(Object3D))]
    [RequireComponent(typeof(AnimatorController))]
    [RequireComponent(typeof(AudioSource))]
    public class ActiveObjectByCameraModifier : MonoBehaviour
    {
        private Object3D _object3D;
        private AnimatorController _animationController;
        private AudioSource _audioSource;
        private bool _awakend;

        public void Awake()
        {
            _object3D = GetComponent<Object3D>();
            _audioSource = GetComponent<AudioSource>();
            _animationController = GetComponent<AnimatorController>();
            _awakend = true;
            GameEvents.Instance.Active += Instance_Active;
        }

        private void Instance_Active(object sender, ActiveEventArgs e)
        {
            ActivateObject();
        }

        public virtual void ActivateObject()
        {
            if (_awakend)
            {
                _object3D.enabled = true;
                _audioSource.enabled = true;
                _animationController.enabled = true;
                _animationController.Animator.gameObject.SetActive(true);
            }
        }
    }

}