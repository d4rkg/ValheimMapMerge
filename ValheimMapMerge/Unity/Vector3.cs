using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValheimMapMerge.Unity
{
    public struct Vector3
    {
        public float x;
        public float y;
        public float z;

        private static readonly Vector3 zeroVector = new Vector3(0f, 0f, 0f);

        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static Vector3 zero
        {
            get
            {
                return Vector3.zeroVector;
            }
        }
    }
}
