﻿using System.Text;

namespace jm2lib.blizzard.wow.lichking
{

	using LERandomAccessFile = com.mindprod.ledatastream.LERandomAccessFile;

	using jm2lib.blizzard.common.types;
	using QuatS = jm2lib.blizzard.common.types.QuatS;
	using Vec3F = jm2lib.blizzard.common.types.Vec3F;
	using AnimFilesHandler = jm2lib.blizzard.wow.common.AnimFilesHandler;
	using MarshalingStream = jm2lib.io.MarshalingStream;
	using UnmarshalingStream = jm2lib.io.UnmarshalingStream;

	public class UVAnimation : AnimFilesHandler
	{
		public AnimationBlock<Vec3F> translation;
		public AnimationBlock<QuatS> rotation;
		public AnimationBlock<Vec3F> scale;

		public UVAnimation()
		{
			translation = new AnimationBlock<Vec3F>(typeof(Vec3F));
			rotation = new AnimationBlock<QuatS>(typeof(QuatS), 1);
			scale = new AnimationBlock<Vec3F>(typeof(Vec3F), 1);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void unmarshal(jm2lib.io.UnmarshalingStream in) throws java.io.IOException, ClassNotFoundException
		public virtual void unmarshal(UnmarshalingStream @in)
		{
			translation.unmarshal(@in);
			rotation.unmarshal(@in);
			scale.unmarshal(@in);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void marshal(jm2lib.io.MarshalingStream out) throws java.io.IOException
		public virtual void marshal(MarshalingStream @out)
		{
			translation.marshal(@out);
			rotation.marshal(@out);
			scale.marshal(@out);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void writeContent(jm2lib.io.MarshalingStream out) throws InstantiationException, IllegalAccessException, java.io.IOException
		public virtual void writeContent(MarshalingStream @out)
		{
				translation.writeContent(@out);
				rotation.writeContent(@out);
				scale.writeContent(@out);
		}

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			builder.Append(this.GetType().FullName).Append(" {\n\ttranslation: ").Append(translation).Append("\n\trotation: ").Append(rotation).Append("\n\tscale: ").Append(scale).Append("\n}");
			return builder.ToString();
		}

		public virtual LERandomAccessFile[] AnimFiles
		{
			set
			{
				translation.AnimFiles = value;
				rotation.AnimFiles = value;
				scale.AnimFiles = value;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public jm2lib.blizzard.wow.burningcrusade.UVAnimation downConvert(jm2lib.blizzard.common.types.ArrayRef<jm2lib.blizzard.wow.classic.Animation> animations) throws Exception
		public virtual jm2lib.blizzard.wow.burningcrusade.UVAnimation downConvert(ArrayRef<jm2lib.blizzard.wow.classic.Animation> animations)
		{
			jm2lib.blizzard.wow.burningcrusade.UVAnimation output = new jm2lib.blizzard.wow.burningcrusade.UVAnimation();
			output.translation = translation.downConvert(animations);
			output.rotation = rotation.downConvert(animations);
			output.scale = scale.downConvert(animations);
			return output;
		}
	}

}