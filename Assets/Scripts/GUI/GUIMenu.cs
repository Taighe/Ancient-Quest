using UnityEngine;
using UnityEngine.EventSystems;

namespace AQEngine.GUI
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Canvas))]
    public class GUIMenu : MonoBehaviour
    {
        public int MenuIndex;
        private Animator _animator;

        public void Start()
        {
            _animator = GetComponent<Animator>();
            _animator.SetInteger("menuIndex", MenuIndex);
        }

        public void SetMenuIndex(int index)
        {
            MenuIndex = index;
            _animator.SetInteger("menuIndex", MenuIndex);
        }

        public void UpdateEventSystem()
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(EventSystem.current.firstSelectedGameObject);
            var events = EventSystem.current.gameObject;
            events.SetActive(false);
            events.SetActive(true);
        }
    }
}