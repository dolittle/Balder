using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework;
using XnaColor = Microsoft.Xna.Framework.Color;

namespace Balder.Rendering.Xna.Shaders
{
	public struct LightInfo
	{
		public const int MaxLights = 5;

		public Vector4 L0_PositionOrDirection;
		public XnaColor L0_Ambient;
		public XnaColor L0_Diffuse;
		public XnaColor L0_Specular;
		public float L0_Strength;
		public float L0_Range;
		public LightType L0_LightType;
		public int L0_Pad;

		public Vector4 L1_PositionOrDirection;
		public XnaColor L1_Ambient;
		public XnaColor L1_Diffuse;
		public XnaColor L1_Specular;
		public float L1_Strength;
		public float L1_Range;
		public LightType L1_LightType;
		public int L1_Pad;

		public Vector4 L2_PositionOrDirection;
		public XnaColor L2_Ambient;
		public XnaColor L2_Diffuse;
		public XnaColor L2_Specular;
		public float L2_Strength;
		public float L2_Range;
		public LightType L2_LightType;
		public int L2_Pad;
		/*
		public Vector4 L3_PositionOrDirection;
		public XnaColor L3_Ambient;
		public XnaColor L3_Diffuse;
		public XnaColor L3_Specular;
		public float L3_Strength;
		public float L3_Range;
		public LightType L3_LightType;
		public int L3_Pad;

		public Vector4 L4_PositionOrDirection;
		public XnaColor L4_Ambient;
		public XnaColor L4_Diffuse;
		public XnaColor L4_Specular;
		public float L4_Strength;
		public float L4_Range;
		public LightType L4_LightType;
		public int L4_Pad;

		
		public Vector4 L5_PositionOrDirection;
		public XnaColor L5_Ambient;
		public XnaColor L5_Diffuse;
		public XnaColor L5_Specular;
		public float L5_Strength;
		public float L5_Range;
		public LightType L5_LightType;
		public int L5_Pad;

		public Vector4 L6_PositionOrDirection;
		public XnaColor L6_Ambient;
		public XnaColor L6_Diffuse;
		public XnaColor L6_Specular;
		public float L6_Strength;
		public float L6_Range;
		public LightType L6_LightType;
		public int L6_Pad;
		*/
		private static Dictionary<string, FieldInfo> _lightInfoFields = new Dictionary<string, FieldInfo>();

		static LightInfo()
		{
			var fields = typeof(LightInfo).GetFields();
			foreach (var field in fields)
				_lightInfoFields[field.Name] = field;
		}

		public static void SetField(object lightInfo, int count, string name, object value)
		{
			if (count >= MaxLights)
				throw new ArgumentException("Only "+MaxLights+" Lirectional lights are supported");

			var fieldName = string.Format("L{0}_{1}", count, name);
			_lightInfoFields[fieldName].SetValue(lightInfo, value);
		}
	}
}