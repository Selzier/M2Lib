using System.Collections.Generic;
using System.IO;
using UnityEngine;
using M2Lib.interfaces;
using M2Lib.types;

namespace M2Lib.m2
{
    public class M2TextureTransform : IAnimated
    {
        public M2Track<Vector3> Translation { get; set; } = new M2Track<Vector3>();
        public M2Track<Quaternion> Rotation { get; set; } = new M2Track<Quaternion>(Quaternion.identity);
        public M2Track<Vector3> Scale { get; set; } = new M2Track<Vector3>(Vector3.one);

        public void Load(BinaryReader stream, M2.Format version)
        {
            Translation.Load(stream, version);
            Rotation.Load(stream, version);
            Scale.Load(stream, version);
        }

        public void Save(BinaryWriter stream, M2.Format version)
        {
            Translation.Save(stream, version);
            Rotation.Save(stream, version);
            Scale.Save(stream, version);
        }

        public void LoadContent(BinaryReader stream, M2.Format version)
        {
            Translation.LoadContent(stream, version);
            Rotation.LoadContent(stream, version);
            Scale.LoadContent(stream, version);
        }

        public void SaveContent(BinaryWriter stream, M2.Format version)
        {
            Translation.SaveContent(stream, version);
            Rotation.SaveContent(stream, version);
            Scale.SaveContent(stream, version);
        }

        /// <summary>
        ///     Pass the sequences reference to Tracks so they can : switch between 1 timeline & multiple timelines, open .anim
        ///     files...
        /// </summary>
        /// <param name="sequences"></param>
        public void SetSequences(IReadOnlyList<M2Sequence> sequences)
        {
            Translation.Sequences = sequences;
            Rotation.Sequences = sequences;
            Scale.Sequences = sequences;
        }
    }
}