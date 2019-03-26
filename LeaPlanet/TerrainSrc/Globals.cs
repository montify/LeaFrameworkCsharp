using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeaFramework.Effect;
using LeaFramework.PlayGround.Misc;

namespace LeaFramework.PlayGround.TerrainSrc
{
    public static class Globals
    {
        public static int earthRadius = 6000;
        public static SpaceCam cam;
        public static LeaEffect effect;
        public static readonly double FarClippingPlaneDistance = 10000000;
        public static readonly short nodeSize = 256; // n²+1
        public static readonly int maxNodeDepth = 10;  //NORMAL MAP 13 ok 14 bad
    }
}
