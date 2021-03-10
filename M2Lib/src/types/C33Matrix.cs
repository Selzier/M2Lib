using UnityEngine;
namespace M2Lib.types
{    
    /// <summary>
    ///     A 3x3 matrix;
    /// </summary>
    public class C33Matrix
    {
        public readonly Vector3[] Columns = new Vector3[3];

        public C33Matrix() : this(new Vector3(), new Vector3(), new Vector3()) { }

        public C33Matrix(Vector3 col0, Vector3 col1, Vector3 col2)
        {
            //Columns = new C3Vector[3];
            Columns[0] = col0;
            Columns[1] = col1;
            Columns[2] = col2;
        }

        public override string ToString()
        {
            return $"({Columns[0]},{Columns[1]},{Columns[2]})";
        }
    }
}