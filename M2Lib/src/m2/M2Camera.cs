﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using M2Lib.interfaces;
using M2Lib.io;
using M2Lib.types;

namespace M2Lib.m2
{
    public class M2Camera : IAnimated
    {
        public enum CameraType
        {
            Portrait = 0,
            CharacterInfo = 1,
            UserInterface = -1
        }

        public CameraType Type { get; set; } = CameraType.UserInterface;
        public float FarClip { get; set; }
        public float NearClip { get; set; }
        public M2Track<C33Matrix> Positions { get; set; } = new M2Track<C33Matrix>();
        public Vector3 PositionBase { get; set; }
        public M2Track<C33Matrix> TargetPositions { get; set; } = new M2Track<C33Matrix>();
        public Vector3 TargetPositionBase { get; set; }
        public M2Track<Vector3> Roll { get; set; } = new M2Track<Vector3>();
        public M2Track<Vector3> FieldOfView { get; set; } = new M2Track<Vector3>();

        public void Load(BinaryReader stream, M2.Format version)
        {
            Type = (CameraType) stream.ReadInt32();
            if (version < (M2.Format) 271)
            {
                FieldOfView.Timestamps.Add(new M2Array<uint> {0});
                FieldOfView.Values.Add(new M2Array<Vector3> {new Vector3(stream.ReadSingle(), 0, 0)});
            }
            FarClip = stream.ReadSingle();
            NearClip = stream.ReadSingle();
            Positions.Load(stream, version);
            PositionBase = stream.ReadVector3();
            TargetPositions.Load(stream, version);
            TargetPositionBase = stream.ReadVector3();
            Roll.Load(stream, version);
            if (version >= (M2.Format) 271) FieldOfView.Load(stream, version);
        }

        public void Save(BinaryWriter stream, M2.Format version)
        {
            stream.Write((int) Type);
            if (version < (M2.Format)271)
            {
                if (FieldOfView.Values.Count == 1) stream.Write(FieldOfView.Values[0][0].x);
                else stream.Write(Type == CameraType.Portrait ? 0.7F : 0.97F);
            }
            stream.Write(FarClip);
            stream.Write(NearClip);
            Positions.Save(stream, version);
            stream.Write(PositionBase);
            TargetPositions.Save(stream, version);
            stream.Write(TargetPositionBase);
            Roll.Save(stream, version);
            if (version >= (M2.Format)271) FieldOfView.Save(stream, version);
        }

        public override string ToString()
        {
            return $"Type: {Type}, FarClip: {FarClip}, NearClip: {NearClip}, Positions: {Positions}, PositionBase: {PositionBase}, TargetPositions: {TargetPositions}, TargetPositionBase: {TargetPositionBase}, Roll: {Roll}, FieldOfView: {FieldOfView}";
        }

        public void LoadContent(BinaryReader stream, M2.Format version)
        {
            Positions.LoadContent(stream, version);
            TargetPositions.LoadContent(stream, version);
            Roll.LoadContent(stream, version);
            if (version >= (M2.Format)271) FieldOfView.LoadContent(stream, version);
        }

        public void SaveContent(BinaryWriter stream, M2.Format version)
        {
            Positions.SaveContent(stream, version);
            TargetPositions.SaveContent(stream, version);
            Roll.SaveContent(stream, version);
            if (version >= (M2.Format)271) FieldOfView.SaveContent(stream, version);
        }

        public void SetSequences(IReadOnlyList<M2Sequence> sequences)
        {
            Positions.Sequences = sequences;
            TargetPositions.Sequences = sequences;
            Roll.Sequences = sequences;
            FieldOfView.Sequences = sequences;
        }

        public static M2Array<short> GenerateLookup(M2Array<M2Camera> cameras)
        {
            var lookup = new M2Array<short>();
            if (cameras.Count == 0) return lookup;
            var maxId = (short) cameras.Max(x => x.Type);
            if (maxId == -1)
            {
                lookup.Add(-1);
                return lookup;
            }
            for (short i = 0; i <= maxId; i++) lookup.Add(-1);
            for (short i = 0; i < cameras.Count; i++)
            {
                var id = (short) cameras[i].Type;
                if (id >= 0 && lookup[id] == -1) lookup[id] = i;
            }
            return lookup;
        }
    }
}