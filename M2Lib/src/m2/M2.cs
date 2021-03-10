using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using M2Lib.interfaces;
using M2Lib.io;
using M2Lib.types;

namespace M2Lib.m2
{
    /// <summary>
    ///     World of Warcraft model format.
    /// </summary>
    public class M2 : IMarshalable
    {
        /// <summary>
        ///     Versions of M2 encountered so far.
        /// </summary>
        public enum Format
        {
            Useless = 0xCAFE,
            Classic = 256,
            BurningCrusade = 260,
            LateBurningCrusade = 263,
            LichKing = 264,
            Cataclysm = 272,
            Pandaria = 272,
            Draenor = 272,
            Legion = 274
        }

        [Flags]
        public enum GlobalFlags
        {
            TiltX = 0x0001,
            TiltY = 0x0002,
            Add2Fields = 0x0008,
            LoadPhys = 0x0020,
            HasLod = 0x0080,
            CameraRelated = 0x0100
        }

        /// <summary>
        /// Possible Chunks in an M2 File
        /// </summary>
        public enum ChunkID { 
            MD21 = 825377869,
            PFID = 1145652816,
            SFID = 1145652819,
            AFID = 1145652801,
            BFID = 1145652802,
            TXAC = 1128355924,
            EXPT = 1414551621,
            EXP2 = 844126277,
            PABC = 1128415568,
            PADC = 1128546640,
            PSBC = 1128420176,
            PEDC = 1128547664,
            SKID = 1145654099,
            TXID = 1145657428,
            LDV1 = 827737164,
            RPID = 1145655378,
            GPID = 1145655367,
            WFV1 = 827737687,
            WFV2 = 844514903,
            PGD1 = 826558288,
            WFV3 = 861292119,
            PFDC = 1128547920,
            EDGF = 1179075653,
            NERF = 1179796814,
            DETL = 1280591172        
        }

        public int[] skinFileDataIDs;
        public int nViews;
        //end my temp vars

        private readonly M2Array<byte> _name = new M2Array<byte>();

        public Format Version { get; set; } = Format.Draenor;

        public string Name
        {
            get { return _name.ToNameString(); }
            set { _name.SetString(value); }
        }

        public GlobalFlags GlobalModelFlags { get; set; } = 0;
        public M2Array<int> GlobalSequences { get; } = new M2Array<int>();
        public M2Array<M2Sequence> Sequences { get; } = new M2Array<M2Sequence>();
        public M2Array<M2Bone> Bones { get; } = new M2Array<M2Bone>();
        public M2Array<M2Vertex> GlobalVertexList { get; } = new M2Array<M2Vertex>();
        public M2Array<M2SkinProfile> Views { get; } = new M2Array<M2SkinProfile>();
        public M2Array<M2Color> Colors { get; } = new M2Array<M2Color>();
        public M2Array<M2Texture> Textures { get; } = new M2Array<M2Texture>();
        public M2Array<M2TextureWeight> Transparencies { get; } = new M2Array<M2TextureWeight>();
        public M2Array<M2TextureTransform> TextureTransforms { get; } = new M2Array<M2TextureTransform>();
        public M2Array<M2Material> Materials { get; } = new M2Array<M2Material>();
        public M2Array<M2Attachment> Attachments { get; } = new M2Array<M2Attachment>();
        public M2Array<M2Event> Events { get; } = new M2Array<M2Event>();
        public M2Array<M2Light> Lights { get; } = new M2Array<M2Light>();
        public M2Array<M2Camera> Cameras { get; } = new M2Array<M2Camera>();

        // Data referenced by Views. TODO See if can be generated on the fly.
        public M2Array<short> BoneLookup { get; } = new M2Array<short>();
        public M2Array<short> TexLookup { get; } = new M2Array<short>();
        public M2Array<short> TexUnitLookup { get; } = new M2Array<short>();
        public M2Array<short> TransLookup { get; } = new M2Array<short>();
        public M2Array<short> UvAnimLookup { get; } = new M2Array<short>();

        public CAaBox BoundingBox { get; set; }
        public float BoundingSphereRadius { get; set; }
        public CAaBox CollisionBox { get; set; }
        public float CollisionSphereRadius { get; set; }
        public M2Array<ushort> CollisionTriangles { get; } = new M2Array<ushort>();
        public M2Array<Vector3> CollisionVertices { get; } = new M2Array<Vector3>();
        public M2Array<Vector3> CollisionNormals { get; } = new M2Array<Vector3>();
        public M2Array<M2Ribbon> Ribbons { get; } = new M2Array<M2Ribbon>();
        public M2Array<M2Particle> Particles { get; } = new M2Array<M2Particle>();
        public M2Array<ushort> BlendingMaps { get; } = new M2Array<ushort>();

        public void Load(BinaryReader stream, Format version = Format.Useless)
        {
            BinaryReader fullStream = stream;
            // LOAD MAGIC
            var magic = Encoding.UTF8.GetString(stream.ReadBytes(4)); // 0x000            
            if (magic == "MD21") // Has Chunked Data
            {
                stream.ReadBytes(4); // Ignore chunked structure of Legion (until after reading header)
                stream = new BinaryReader(new Substream(stream.BaseStream));
                magic = Encoding.UTF8.GetString(stream.ReadBytes(4));
            }

            if (magic != "MD20") { UnityEngine.Debug.LogError("Invalid MD20 Magic: " + magic); }
            // LOAD HEADER
            if (version == Format.Useless) version = (Format) stream.ReadUInt32();
            else stream.ReadUInt32();
            Version = version; // 0x004
            if (version == Format.Useless) { UnityEngine.Debug.LogError("Invalid Version" + version); }
            _name.Load(stream, version); // 0x008
            GlobalModelFlags = (GlobalFlags) stream.ReadUInt32();
            GlobalSequences.Load(stream, version);
            Sequences.Load(stream, version); // 0x010
            SkipArrayParsing(stream, version);
            if (version < Format.LichKing) SkipArrayParsing(stream, version);
            Bones.Load(stream, version);
            SkipArrayParsing(stream, version);
            GlobalVertexList.Load(stream, version);
            //uint nViews = 0; //For Lich King external views system.
            nViews = 0;
            if (version < Format.LichKing) Views.Load(stream, version);
            else nViews = (int)stream.ReadUInt32();
            Colors.Load(stream, version);
            Textures.Load(stream, version);
            Transparencies.Load(stream, version);
            if (version < Format.LichKing) SkipArrayParsing(stream, version); //Unknown Ref
            TextureTransforms.Load(stream, version);
            SkipArrayParsing(stream, version);
            Materials.Load(stream, version);
            BoneLookup.Load(stream, version);
            TexLookup.Load(stream, version);
            TexUnitLookup.Load(stream, version);
            TransLookup.Load(stream, version);
            UvAnimLookup.Load(stream, version);
            BoundingBox = stream.ReadCAaBox();
            BoundingSphereRadius = stream.ReadSingle();
            CollisionBox = stream.ReadCAaBox();
            CollisionSphereRadius = stream.ReadSingle();
            CollisionTriangles.Load(stream, version);
            CollisionVertices.Load(stream, version);
            CollisionNormals.Load(stream, version);
            Attachments.Load(stream, version);
            SkipArrayParsing(stream, version);
            Events.Load(stream, version);
            Lights.Load(stream, version);
            Cameras.Load(stream, version);
            SkipArrayParsing(stream, version);
            Ribbons.Load(stream, version);
            Particles.Load(stream, version);
            if (version >= Format.LichKing && GlobalModelFlags.HasFlag(GlobalFlags.Add2Fields)) BlendingMaps.Load(stream, version);

            // LOAD REFERENCED CONTENT
            _name.LoadContent(stream);
            GlobalSequences.LoadContent(stream);
            Sequences.LoadContent(stream, version);
            if (version >= Format.LichKing)
            {
                foreach (var seq in Sequences.Where(seq => !seq.IsAlias && seq.IsExtern))
                {
                    //var substream = stream.BaseStream as Substream;
                    //var path = substream != null ? ((FileStream) substream.GetInnerStream()).Name : ((FileStream) stream.BaseStream).Name;
                    //seq.ReadingAnimFile = new BinaryReader(new FileStream(seq.GetAnimFilePath(path), FileMode.Open));
                }
            }
            SetSequences();
            Bones.LoadContent(stream, version);
            GlobalVertexList.LoadContent(stream, version);
            //VIEWS
            if (version < Format.LichKing) Views.LoadContent(stream, version);
            else
            {
                for (var i = 0; i < nViews; i++)
                {
                    
                    var view = new M2SkinProfile();
                    var substream = stream.BaseStream as Substream;
                    //var path = substream != null ? ((FileStream)substream.GetInnerStream()).Name : ((FileStream)stream.BaseStream).Name;
                    //using (var skinFile = new BinaryReader(new FileStream(M2SkinProfile.SkinFileName(path, i), FileMode.Open)))
                    //{
                    //    view.Load(skinFile, version);
                    //    view.LoadContent(skinFile, version);
                    //}
                    //Views.Add(view);
                }
            }
            //VIEWS END
            Colors.LoadContent(stream, version);
            Textures.LoadContent(stream, version);
            Transparencies.LoadContent(stream, version);
            TextureTransforms.LoadContent(stream, version);

            // @author PhilipTNG
            if(version < Format.Cataclysm) { 
                foreach(var mat in Materials)
                {
                    // Flags fix
                    mat.Flags = mat.Flags & (M2Material.RenderFlags) 0x1F;
                    // Blending mode fix
                    if(mat.BlendMode > M2Material.BlendingMode.DeeprunTram) mat.BlendMode = M2Material.BlendingMode.Mod2X;
                }
            }

            Materials.LoadContent(stream, version);
            BoneLookup.LoadContent(stream, version);
            TexLookup.LoadContent(stream, version);
            TexUnitLookup.LoadContent(stream, version);
            TransLookup.LoadContent(stream, version);
            UvAnimLookup.LoadContent(stream, version);
            CollisionTriangles.LoadContent(stream, version);
            CollisionVertices.LoadContent(stream, version);
            CollisionNormals.LoadContent(stream, version);
            Attachments.LoadContent(stream, version);
            Events.LoadContent(stream, version);
            Lights.LoadContent(stream, version);
            Cameras.LoadContent(stream, version);
            Ribbons.LoadContent(stream, version);
            Particles.LoadContent(stream, version);
            if (version >= Format.LichKing && GlobalModelFlags.HasFlag(GlobalFlags.Add2Fields)) BlendingMaps.LoadContent(stream, version);
            foreach (var seq in Sequences) { seq.ReadingAnimFile?.Close(); }

            // Read M2 Chunks
            stream = fullStream; // Revert back to full file
            stream.BaseStream.Seek(0, SeekOrigin.Begin);

            magic = Encoding.UTF8.GetString(stream.ReadBytes(4)); // 0x000            
            if (magic == "MD21") // Has Chunked Data
            {
                long chunkSize = (long)stream.ReadUInt32();
                long nextChunkPos = stream.BaseStream.Position + chunkSize;
                stream.BaseStream.Seek(nextChunkPos, SeekOrigin.Begin);

                while (stream.BaseStream.Position < stream.BaseStream.Length) {                 // Advance the reader position until the end of the file
                    //string chunkID = Encoding.UTF8.GetString(stream.ReadBytes(4));            // Reading first 4 bytes of a chunk will give us the ID
                    ChunkID chunkID = (ChunkID)stream.ReadUInt32();                             // Maybe better to use INT for performance
                    chunkSize = (long)stream.ReadUInt32();                                      // Get the size in bytes of the current chunk
                    nextChunkPos = stream.BaseStream.Position + chunkSize;                      // The next chunk will be from the current reader's position + size of current chunk

                    // Process current chunk
                    switch (chunkID) {
                        case ChunkID.PFID: break;
                        case ChunkID.SFID: // Skin File IDs
                            skinFileDataIDs = new int[nViews];
                            for (int i = 0; i < nViews; i++) {
                                skinFileDataIDs[i] = (int)stream.ReadUInt32();

                                // At this point external code (CascLib) uses the FileDataIDs to load
                                // skinFiles from CASC into a MemoryStream, and a new M2SkinProfile is 
                                // created from the stream and added to list M2Array<M2SkinProfile>Views.
                            }
                            // lod_skinFileDataIDs come next in this chunk
                            break;
                        case ChunkID.AFID: break;
                        case ChunkID.BFID: break;
                        case ChunkID.TXAC: break;
                        case ChunkID.EXPT: break;
                        case ChunkID.EXP2: break;
                        case ChunkID.PABC: break;
                        case ChunkID.PADC: break;
                        case ChunkID.PSBC: break;
                        case ChunkID.PEDC: break;
                        case ChunkID.SKID: break;
                        case ChunkID.TXID: break;
                        case ChunkID.LDV1: break;
                        case ChunkID.RPID: break;
                        case ChunkID.GPID: break;
                        case ChunkID.WFV1: break;
                        case ChunkID.WFV2: break;
                        case ChunkID.PGD1: break;
                        case ChunkID.WFV3: break;
                        case ChunkID.PFDC: break;
                        case ChunkID.EDGF: break;
                        case ChunkID.NERF: break;
                        case ChunkID.DETL: break;
                    }
                    stream.BaseStream.Seek(nextChunkPos, SeekOrigin.Begin); // Advance the reader position to the the next chunk and proceed with loop
                }
            }
        }

        public void Save(BinaryWriter stream, Format version = Format.Useless)
        {
            SetSequences();

            // SAVE MAGIC
            stream.Write(Encoding.UTF8.GetBytes("MD20"));
            if (version == Format.Useless) version = Version;

            // SAVE HEADER
            stream.Write((uint) version);
            _name.Save(stream, version);
            stream.Write((uint) GlobalModelFlags);
            GlobalSequences.Save(stream, version);
            Sequences.Save(stream, version);
            var sequenceLookup = M2Sequence.GenerateLookup(Sequences);
            sequenceLookup.Save(stream, version);
            M2Array<PlayableRecord> playableLookup = null;
            if (version < Format.LichKing)
            {
                playableLookup = M2Sequence.GeneratePlayableLookup(sequenceLookup);
                playableLookup.Save(stream, version);
            }
            Bones.Save(stream, version);
            var keyBoneLookup = M2Bone.GenerateKeyBoneLookup(Bones);
            keyBoneLookup.Save(stream, version);
            GlobalVertexList.Save(stream, version);
            if (version < Format.LichKing) Views.Save(stream, version);
            else stream.Write(Views.Count);
            Colors.Save(stream, version);
            Textures.Save(stream, version);
            Transparencies.Save(stream, version);
            if (version < Format.LichKing) stream.Write((long) 0); //Unknown Ref
            TextureTransforms.Save(stream, version);
            var texReplaceLookup = M2Texture.GenerateTexReplaceLookup(Textures);
            texReplaceLookup.Save(stream, version);
            Materials.Save(stream, version);
            BoneLookup.Save(stream, version);
            TexLookup.Save(stream, version);
            if(version <= Format.LichKing && TexUnitLookup.Count == 0) TexUnitLookup.Add(0);// @author Zim4ik
            TexUnitLookup.Save(stream, version);
            TransLookup.Save(stream, version);
            UvAnimLookup.Save(stream, version);
            stream.Write(BoundingBox);
            stream.Write(BoundingSphereRadius);
            stream.Write(CollisionBox);
            stream.Write(CollisionSphereRadius);
            CollisionTriangles.Save(stream, version);
            CollisionVertices.Save(stream, version);
            CollisionNormals.Save(stream, version);
            Attachments.Save(stream, version);
            var attachmentLookup = M2Attachment.GenerateLookup(Attachments);
            attachmentLookup.Save(stream, version);
            Events.Save(stream, version);
            Lights.Save(stream, version);
            Cameras.Save(stream, version);
            var cameraLookup = M2Camera.GenerateLookup(Cameras);
            cameraLookup.Save(stream, version);
            Ribbons.Save(stream, version);
            Particles.Save(stream, version);
            if (version >= Format.LichKing && GlobalModelFlags.HasFlag(GlobalFlags.Add2Fields))
                BlendingMaps.Save(stream, version);

            // SAVE REFERENCED CONTENT
            _name.SaveContent(stream);
            GlobalSequences.SaveContent(stream);
            if (version < Format.LichKing)
            {
                uint time = 0;
                //Alias system. TODO Alias should be skipped in the timing ?
                foreach (var seq in Sequences/*.Where(seq => !seq.IsAlias)*/)
                {
                    time += 3333;
                    seq.TimeStart = time;
                    time += seq.Length;
                }
                //set the timeStart of Alias to their real counterpart
                /*
                foreach (var seq in Sequences.Where(seq => seq.IsAlias))
                    seq.TimeStart = seq.GetRealSequence(Sequences).TimeStart;
                    */
            }
            Sequences.SaveContent(stream, version);
            if (version >= Format.LichKing)
            {
                foreach (var seq in Sequences.Where(seq => !seq.IsAlias &&
                                                           seq.IsExtern))
                {
                    var substream = stream.BaseStream as Substream;
                    var path = substream != null ? ((FileStream)substream.GetInnerStream()).Name : ((FileStream)stream.BaseStream).Name;
                    seq.WritingAnimFile =
                        new BinaryWriter(
                            new FileStream(seq.GetAnimFilePath(path), FileMode.Create));
                }
            }
            sequenceLookup.SaveContent(stream);
            playableLookup?.SaveContent(stream);
            Bones.SaveContent(stream, version);
            keyBoneLookup.SaveContent(stream);
            GlobalVertexList.SaveContent(stream, version);
            //VIEWS
            if (version < Format.LichKing) Views.SaveContent(stream, version);
            else
            {
                for (var i = 0; i < Views.Count; i++)
                {
                    var substream = stream.BaseStream as Substream;
                    var path = substream != null ? ((FileStream)substream.GetInnerStream()).Name : ((FileStream)stream.BaseStream).Name;
                    using (var skinFile = new BinaryWriter(
                        new FileStream(M2SkinProfile.SkinFileName(path, i),
                            FileMode.Create)))
                    {
                        Views[i].Save(skinFile, version);
                        Views[i].SaveContent(skinFile, version);
                    }
                }
            }
            //VIEWS END
            Colors.SaveContent(stream, version);
            Textures.SaveContent(stream, version);
            Transparencies.SaveContent(stream, version);
            TextureTransforms.SaveContent(stream, version);
            texReplaceLookup.SaveContent(stream, version);
            Materials.SaveContent(stream, version);
            BoneLookup.SaveContent(stream, version);
            TexLookup.SaveContent(stream, version);
            TexUnitLookup.SaveContent(stream, version);
            TransLookup.SaveContent(stream, version);
            UvAnimLookup.SaveContent(stream, version);
            CollisionTriangles.SaveContent(stream, version);
            CollisionVertices.SaveContent(stream, version);
            CollisionNormals.SaveContent(stream, version);
            Attachments.SaveContent(stream, version);
            attachmentLookup.SaveContent(stream, version);
            Events.SaveContent(stream, version);
            Lights.SaveContent(stream, version);
            Cameras.SaveContent(stream, version);
            cameraLookup.SaveContent(stream, version);
            Ribbons.SaveContent(stream, version);
            Particles.SaveContent(stream, version);
            if (version >= Format.LichKing && GlobalModelFlags.HasFlag(GlobalFlags.Add2Fields)) BlendingMaps.SaveContent(stream, version);
            foreach (var seq in Sequences)
                seq.WritingAnimFile?.Close();
        }

        private void SetSequences()
        {
            Bones.PassSequences(Sequences);
            Colors.PassSequences(Sequences);
            Transparencies.PassSequences(Sequences);
            TextureTransforms.PassSequences(Sequences);
            Attachments.PassSequences(Sequences);
            Events.PassSequences(Sequences);
            Lights.PassSequences(Sequences);
            Cameras.PassSequences(Sequences);
            Ribbons.PassSequences(Sequences);
            Particles.PassSequences(Sequences);
        }

        /// <summary>
        ///     Skip the parsing of useless M2Array (like lookups, since lookups are generated at writing).
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="version"></param>
        private void SkipArrayParsing(BinaryReader stream, Format version)
        {
            new M2Array<short>().Load(stream, version);
        }
    }
}