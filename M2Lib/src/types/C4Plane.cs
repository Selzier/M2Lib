using UnityEngine;
namespace M2Lib.types
{
    /// <summary>
    ///     A 3D plane defined by four floats
    /// </summary>
    public struct C4Plane
    {
        public readonly float Distance;
        public readonly Vector3 Normal;

        public C4Plane(Vector3 vec, float dist)
        {
            Normal = vec;
            Distance = dist;
        }

        public override string ToString()
        {
            return $"Normal:{Normal} Dist:{Distance}";
        }
    }
}