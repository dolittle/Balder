#region License
//
// Author: Einar Ingebrigtsen <einar@dolittle.com>
// Copyright (c) 2007-2010, DoLittle Studios
//
// Licensed under the Microsoft Permissive License (Ms-PL), Version 1.1 (the "License")
// you may not use this file except in compliance with the License.
// You may obtain a copy of the license at 
//
//   http://balder.codeplex.com/license
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion
#if(XNA)
using System;
using System.IO;
using System.Windows;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Silverlight;

namespace Balder.Rendering.Xna
{
	public class ShaderManager
	{
		const string Path = @"Balder;component/Rendering/Xna/Shaders/";

		static readonly GraphicsDevice resourceDevice = GraphicsDeviceManager.Current.GraphicsDevice;

		static Stream GetStream(string path)
		{
			var stream = Application.GetResourceStream(new Uri(path, UriKind.Relative)).Stream;
			return stream;
		}

		static VertexShader GetVertexShader(string name)
		{
			var path = string.Format("{0}{1}.vs", Path, name);
			var stream = GetStream(path);
			var shader = VertexShader.FromStream(resourceDevice, stream);
			return shader;
		}

		static PixelShader GetPixelShader(string name)
		{
			var path = string.Format("{0}{1}.ps", Path, name);
			var stream = GetStream(path);
			var shader = PixelShader.FromStream(resourceDevice, stream);
			return shader;
		}


		public static readonly ShaderManager Instance = new ShaderManager();

		private ShaderManager()
		{
			var properties = GetType().GetProperties();
			foreach( var property in properties )
			{
				if (property.PropertyType != typeof(Shader))
					continue;

				var shaderName = property.Name;
				var shader = new Shader();
				shader.Vertex = GetVertexShader(shaderName);
				shader.Pixel = GetPixelShader(shaderName);

				property.SetValue(this, shader, null);
			}
		}

		public Shader Flat { get; private set; }
		public Shader Gouraud { get; private set; }

		public Shader FlatTexture { get; private set; }
		public Shader GouraudTexture { get; private set; }

		public Shader FlatDualTexture { get; private set; }
		public Shader GouraudDualTexture { get; private set; }

		public Shader Texture { get; private set; }
		public Shader DualTexture { get; private set; }
	}
}
#endif