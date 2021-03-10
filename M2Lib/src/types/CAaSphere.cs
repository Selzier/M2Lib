using UnityEngine;
namespace M2Lib.types
{
    /// <summary>
    ///     An axis aligned sphere described by position and radius.
    /// </summary>
    public struct CAaSphere
    {
        public readonly Vector3 Position;
        public readonly float Radius;

        public CAaSphere(Vector3 pos, float rad)
        {
            Position = pos;
            Radius = rad;
        }

        public override string ToString()
        {
            return $"Pos:{Position} Radius:{Radius}";
        }
    }
}