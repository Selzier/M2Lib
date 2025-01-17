﻿using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using M2Lib.interfaces;
using M2Lib.m2;
using M2Lib.types;

namespace M2Lib.io
{
    /// <summary>
    ///     Extensions to BinaryReader and BinaryWriter to hide the generic type identification done during IO.
    /// </summary>
    public static class StreamExtensions
    {
        public static readonly Dictionary<Type, Func<BinaryReader, object>> ReadFunctions =
            new Dictionary<Type, Func<BinaryReader, object>>();

        public static readonly Dictionary<Type, Action<BinaryWriter, object>> WriteFunctions =
            new Dictionary<Type, Action<BinaryWriter, object>>();

        static StreamExtensions()
        {
            ReadFunctions[typeof (bool)] = s => s.ReadBoolean();
            ReadFunctions[typeof (byte)] = s => s.ReadByte();
            ReadFunctions[typeof (short)] = s => s.ReadInt16();
            ReadFunctions[typeof (ushort)] = s => s.ReadUInt16();
            ReadFunctions[typeof (int)] = s => s.ReadInt32();
            ReadFunctions[typeof (uint)] = s => s.ReadUInt32();
            ReadFunctions[typeof (float)] = s => s.ReadSingle();
            ReadFunctions[typeof (Vector2)] = s => s.ReadVector2();
            ReadFunctions[typeof (C33Matrix)] = s => s.ReadC33Matrix();
            ReadFunctions[typeof (Vector3)] = s => s.ReadVector3();
            ReadFunctions[typeof (C4Plane)] = s => s.ReadC4Plane();
            ReadFunctions[typeof (Quaternion)] = s => s.ReadQuaternion();
            ReadFunctions[typeof (Vector4)] = s => s.ReadVector4();
            ReadFunctions[typeof (CAaBox)] = s => s.ReadCAaBox();
            ReadFunctions[typeof (CAaSphere)] = s => s.ReadCAaSphere();
            ReadFunctions[typeof (CArgb)] = s => s.ReadCArgb();
            ReadFunctions[typeof (M2CompQuat)] = s => s.ReadCompQuat();
            ReadFunctions[typeof (CRange)] = s => s.ReadCRange();
            ReadFunctions[typeof (FixedPoint_0_15)] = s => s.ReadFixedPoint_0_15();
            ReadFunctions[typeof (FixedPoint_6_9)] = s => s.ReadFixedPoint_6_9();
            ReadFunctions[typeof (FixedPoint_2_5)] = s => s.ReadFixedPoint_2_5();
            ReadFunctions[typeof (VertexProperty)] = s => s.ReadVertexProperty();

            WriteFunctions[typeof (bool)] = (s, t) => s.Write((bool) t);
            WriteFunctions[typeof (byte)] = (s, t) => s.Write((byte) t);
            WriteFunctions[typeof (short)] = (s, t) => s.Write((short) t);
            WriteFunctions[typeof (ushort)] = (s, t) => s.Write((ushort) t);
            WriteFunctions[typeof (int)] = (s, t) => s.Write((int) t);
            WriteFunctions[typeof (uint)] = (s, t) => s.Write((uint) t);
            WriteFunctions[typeof (float)] = (s, t) => s.Write((float) t);
            WriteFunctions[typeof (Vector2)] = (s, t) => s.Write((Vector2) t);
            WriteFunctions[typeof (C33Matrix)] = (s, t) => s.Write((C33Matrix) t);
            WriteFunctions[typeof (Vector3)] = (s, t) => s.Write((Vector3) t);
            WriteFunctions[typeof (C4Plane)] = (s, t) => s.Write((C4Plane) t);
            WriteFunctions[typeof (Quaternion)] = (s, t) => s.Write((Quaternion) t);
            WriteFunctions[typeof (Vector4)] = (s, t) => s.Write((Vector4) t);
            WriteFunctions[typeof (CAaBox)] = (s, t) => s.Write((CAaBox) t);
            WriteFunctions[typeof (CAaSphere)] = (s, t) => s.Write((CAaSphere) t);
            WriteFunctions[typeof (CArgb)] = (s, t) => s.Write((CArgb) t);
            WriteFunctions[typeof (M2CompQuat)] = (s, t) => s.Write((M2CompQuat) t);
            WriteFunctions[typeof (CRange)] = (s, t) => s.Write((CRange) t);
            WriteFunctions[typeof (FixedPoint_0_15)] = (s, t) => s.Write((FixedPoint_0_15) t);
            WriteFunctions[typeof (FixedPoint_6_9)] = (s, t) => s.Write((FixedPoint_6_9) t);
            WriteFunctions[typeof (FixedPoint_2_5)] = (s, t) => s.Write((FixedPoint_2_5) t);
            WriteFunctions[typeof (VertexProperty)] = (s, t) => s.Write((VertexProperty) t);
        }

        public static T ReadGeneric<T>(this BinaryReader stream, M2.Format version)
            where T : new()
        {
            if (!typeof (IMarshalable).IsAssignableFrom(typeof (T))) return (T) ReadFunctions[typeof (T)](stream);
            var item = new T();
            ((IMarshalable) item).Load(stream, version);
            return item;
        }


        public static void WriteGeneric<T>(this BinaryWriter stream, M2.Format version, T item) where T : new()
        {
            if (typeof (IMarshalable).IsAssignableFrom(typeof (T)))
                ((IMarshalable) item).Save(stream, version);
            else WriteFunctions[typeof (T)](stream, item);
        }

        //READING OF STRUCTS
        public static Vector2 ReadVector2(this BinaryReader stream)
            => new Vector2(stream.ReadSingle(), stream.ReadSingle());

        public static C33Matrix ReadC33Matrix(this BinaryReader stream)
            => new C33Matrix(stream.ReadVector3(), stream.ReadVector3(), stream.ReadVector3());

        public static Vector3 ReadVector3(this BinaryReader stream)
            => new Vector3(stream.ReadSingle(), stream.ReadSingle(), stream.ReadSingle());

        public static C4Plane ReadC4Plane(this BinaryReader stream)
            => new C4Plane(stream.ReadVector3(), stream.ReadSingle());

        public static Quaternion ReadQuaternion(this BinaryReader stream)
            => new Quaternion(stream.ReadSingle(), stream.ReadSingle(), stream.ReadSingle(), stream.ReadSingle());

        public static Vector4 ReadVector4(this BinaryReader stream)
            => new Vector4(stream.ReadSingle(), stream.ReadSingle(), stream.ReadSingle(), stream.ReadSingle());

        public static CAaBox ReadCAaBox(this BinaryReader stream)
            => new CAaBox(stream.ReadVector3(), stream.ReadVector3());

        public static CAaSphere ReadCAaSphere(this BinaryReader stream)
            => new CAaSphere(stream.ReadVector3(), stream.ReadSingle());

        public static CArgb ReadCArgb(this BinaryReader stream)
            => new CArgb(stream.ReadByte(), stream.ReadByte(), stream.ReadByte(), stream.ReadByte());

        public static M2CompQuat ReadCompQuat(this BinaryReader stream)
            => new M2CompQuat(stream.ReadInt16(), stream.ReadInt16(), stream.ReadInt16(), stream.ReadInt16());

        public static CRange ReadCRange(this BinaryReader stream)
            => new CRange(stream.ReadSingle(), stream.ReadSingle());

        public static FixedPoint_0_15 ReadFixedPoint_0_15(this BinaryReader stream)
            => new FixedPoint_0_15(stream.ReadInt16());

        public static FixedPoint_6_9 ReadFixedPoint_6_9(this BinaryReader stream)
            => new FixedPoint_6_9(stream.ReadInt16());

        public static FixedPoint_2_5 ReadFixedPoint_2_5(this BinaryReader stream)
            => new FixedPoint_2_5(stream.ReadByte());

        public static VertexProperty ReadVertexProperty(this BinaryReader stream)
            => new VertexProperty(stream.ReadByte(), stream.ReadByte(), stream.ReadByte(), stream.ReadByte());

        //WRITING OF STRUCTS
        public static void Write(this BinaryWriter stream, Vector2 item)
        {
            stream.Write(item.x);
            stream.Write(item.y);
        }

        public static void Write(this BinaryWriter stream, C33Matrix item)
        {
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < item.Columns.Length; i++)
            {
                stream.Write(item.Columns[i]);
            }
        }

        public static void Write(this BinaryWriter stream, Vector3 item)
        {
            stream.Write(item.x);
            stream.Write(item.y);
            stream.Write(item.z);
        }

        public static void Write(this BinaryWriter stream, C4Plane item)
        {
            stream.Write(item.Normal);
            stream.Write(item.Distance);
        }

        public static void Write(this BinaryWriter stream, Quaternion item)
        {
            stream.Write(item.x);
            stream.Write(item.y);
            stream.Write(item.z);
            stream.Write(item.w);
        }

        public static void Write(this BinaryWriter stream, Vector4 item)
        {
            stream.Write(item.w);
            stream.Write(item.x);
            stream.Write(item.y);
            stream.Write(item.z);
        }

        public static void Write(this BinaryWriter stream, CAaBox item)
        {
            stream.Write(item.Min);
            stream.Write(item.Max);
        }

        public static void Write(this BinaryWriter stream, CAaSphere item)
        {
            stream.Write(item.Position);
            stream.Write(item.Radius);
        }

        public static void Write(this BinaryWriter stream, CArgb item)
        {
            stream.Write(item.R);
            stream.Write(item.G);
            stream.Write(item.B);
            stream.Write(item.A);
        }

        public static void Write(this BinaryWriter stream, M2CompQuat item)
        {
            stream.Write(item.X);
            stream.Write(item.Y);
            stream.Write(item.Z);
            stream.Write(item.W);
        }

        public static void Write(this BinaryWriter stream, CRange item)
        {
            stream.Write(item.Min);
            stream.Write(item.Max);
        }

        public static void Write(this BinaryWriter stream, FixedPoint_0_15 item)
            => stream.Write(item.ToShort());
        public static void Write(this BinaryWriter stream, FixedPoint_6_9 item)
            => stream.Write(item.ToShort());
        public static void Write(this BinaryWriter stream, FixedPoint_2_5 item)
            => stream.Write(item.ToByte());

        public static void Write(this BinaryWriter stream, VertexProperty item)
        {
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < item.Properties.Length; i++)
            {
                stream.Write(item.Properties[i]);
            }
        }
    }
}