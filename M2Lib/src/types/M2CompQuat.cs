using UnityEngine;
namespace M2Lib.types
{
    /// <summary>
    ///     A four shorts (compressed) quaternion.
    /// </summary>
    public struct M2CompQuat
    {
        public readonly short X, Y, Z, W;

        public M2CompQuat(short p1, short p2, short p3, short p4)
        {
            X = p1;
            Y = p2;
            Z = p3;
            W = p4;
        }

        public M2CompQuat(float p1, float p2, float p3, float p4) {
            X = FloatToShort(p1);
            Y = FloatToShort(p2);
            Z = FloatToShort(p3);
            W = FloatToShort(p4);
        }

        public M2CompQuat(Quaternion quaternion) {
            X = FloatToShort(quaternion.x);
            Y = FloatToShort(quaternion.y);
            Z = FloatToShort(quaternion.z);
            W = FloatToShort(quaternion.w);
        }

        public static explicit operator Quaternion(M2CompQuat comp)
        {
            return new Quaternion(ShortToFloat(comp.X), ShortToFloat(comp.Y), ShortToFloat(comp.Z), ShortToFloat(comp.W));
        }

        /// <summary>
        ///     Decompress a short in a float.
        /// </summary>
        /// <param name="value">The short to convert.</param>
        /// <returns>A converted float value.</returns>
        private static float ShortToFloat(short value)
        {
            if (value == -1) return 1;
            return (float) ((value > 0 ? value - 32767 : value + 32767)/32767.0);
        }

        /// <summary>
        ///     Compress a float in a short
        /// </summary>
        /// <param name="value">Float to compress.</param>
        /// <returns>A short, compressed version of value.</returns>
        private static short FloatToShort(float value) {
            return (short)(value > 0 ? value * 32767.0 - 32768 : value * 32767.0 + 32768);
        }

        public override string ToString()
        {
            return $"({X},{Y},{Z},{W})";
        }
    }
}