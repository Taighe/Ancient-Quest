using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Globals
{
    public enum Layers 
    {
        Default = 1 << 0,
        TransparentFX = 1 << 1,
        Kinematic = 1 << 6,
        Object = 1 << 7,
        Player = 1 << 8
    }

    public static class LayerHelper
    {
        public static int LayerMask(params Layers[] layers)
        {
            switch(layers.Length)
            {
                case 1:
                    return (int)layers[0];
                case 2:
                    return (int)(layers[0] | layers[1]);
                case 3:
                    return (int)(layers[0] | layers[1] | layers[2]);
                case 4:
                    return (int)(layers[0] | layers[1] | layers[2] | layers[3]);
                case 5:
                    return (int)(layers[0] | layers[1] | layers[2] | layers[3] | layers[4]);
                case 6:
                    return (int)(layers[0] | layers[1] | layers[2] | layers[3] | layers[4] | layers[5]);
                default:
                    return 0;
            }
        }

        public static int LayerMask(int layer)
        {
            return 1 << layer;
        }
    }
}
