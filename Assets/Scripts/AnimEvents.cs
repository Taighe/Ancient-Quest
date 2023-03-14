using UnityEngine;

namespace AQEngine
{
    public class AnimEvents : MonoBehaviour
    {
        public bool NoFaceDirection { get; set; }
        public void StartNoFaceAnimEvent()
        {
            NoFaceDirection = true;
        }

        public void EndNoFaceAnimEvent()
        {
            NoFaceDirection = false;
        }
    }
}