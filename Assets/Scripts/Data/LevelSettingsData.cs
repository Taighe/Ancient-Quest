using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AQEngine.Data 
{
    public class LevelSettingsData : ScriptableObject
    {
        public int BoundaryWidth = 5;
        public int BoundaryHeight = 5;
        public float CameraDistance = 18;
        public float FieldOfView = 30;
        public Vector2 Offset = new Vector2(0, 0.5f);

        public Object PlayerPrefab;
        public Object CanvasPrefab;
        public Object EventSystemPrefab;
        public Object SkyPrefab;

        public LevelSettingsData() { }

        public LevelSettingsData(LevelSettingsData data)
        {
            BoundaryWidth = data.BoundaryWidth;
            BoundaryHeight = data.BoundaryHeight;
            CameraDistance = data.CameraDistance;
            PlayerPrefab = data.PlayerPrefab;
            CanvasPrefab = data.CanvasPrefab;
            EventSystemPrefab = data.EventSystemPrefab;
            SkyPrefab = data.SkyPrefab;
        }

        public void SetLevelSettingsData(int width, int height, float cameraDistance, float fieldOfView, Object playerPrefab, Object canvasPrefab, Object eventPrefab, Object skyPrefab, Vector2 offset)
        {
            BoundaryWidth = width;
            BoundaryHeight = height;
            CameraDistance = cameraDistance;
            PlayerPrefab = playerPrefab;
            FieldOfView = fieldOfView;
            CanvasPrefab = canvasPrefab;
            EventSystemPrefab = eventPrefab;
            SkyPrefab = skyPrefab;
            Offset = offset;
        }
    }
}