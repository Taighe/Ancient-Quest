using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Globals
{
    public static class SharedFunctions
    {
        public static bool PointWithinBox(Vector2 point, Vector2 min, Vector2 max)
        {
            if (point.x >= min.x && point.x <= max.x)
            {
                if (point.y >= min.y && point.y <= max.y )
                {
                    return true;
                }
            }

            return false;
        }
    }
}
