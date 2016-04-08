﻿using System;
using System.IO;
using m2lib_csharp.interfaces;

namespace m2lib_csharp.m2
{
    public class Material : IMarshalable
    {
        public RenderFlags Flags { get; set; }
        public BlendingMode Mode { get; set; }

        [Flags]
        public enum RenderFlags
        {
            Unlit = 0x01,
            Unfogged = 0x02,
            TwoSided = 0x04,
            Billboarded = 0x08,
            DisableZBuffer = 0x10,
            ShadowBatchRelated1 = 0x40,
            ShadowBatchRelated2 = 0x80,
            Unknown = 0x400,
            DisableAlpha = 0x800
        }

        public enum BlendingMode : ushort
        {
            Opaque = 0,
            Mod = 1,
            Decal = 2,
            Add = 3,
            Mod2X = 4,
            Fade = 5,
            DeeprunTram = 6,
            Unknown = 7
        }

        public void Load(BinaryReader stream, M2.Format version)
        {
            Flags = (RenderFlags) stream.ReadUInt16();
            Mode = (BlendingMode) stream.ReadUInt16();
        }

        public void Save(BinaryWriter stream, M2.Format version)
        {
            stream.Write((ushort) Flags);
            stream.Write((ushort) Mode);
        }
    }
}