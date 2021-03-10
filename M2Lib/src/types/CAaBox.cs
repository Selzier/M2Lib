using UnityEngine;
namespace M2Lib.types
{
    /// <summary>
    ///     An axis aligned box described by the minimum and maximum point.
    /// </summary>
    public struct CAaBox
    {
        public readonly Vector3 Min, Max;

        public CAaBox(Vector3 min, Vector3 max)
        {
            Min = min;
            Max = max;
        }

        public override string ToString()
        {
            return $"{Min}->{Max}";
        }
    }
}