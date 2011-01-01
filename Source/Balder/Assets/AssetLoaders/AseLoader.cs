#region License
//
// Author: Einar Ingebrigtsen <einar@dolittle.com>
// Copyright (c) 2007-2011, DoLittle Studios
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

using System;
using System.Collections.Generic;
using System.IO;
using Balder.Content;
using Balder.Exceptions;
using Balder.Objects.Geometries;
using Balder.Rendering;


namespace Balder.Assets.AssetLoaders
{


	public class AseLoader : AssetLoader
	{
		private readonly IAssetLoaderService _assetLoaderService;

		public AseLoader(IAssetLoaderService assetLoaderService, IFileLoaderManager fileLoaderManager, IContentManager contentManager)
			: base(fileLoaderManager, contentManager)
		{
			_assetLoaderService = assetLoaderService;
		}


		public override Type SupportedAssetType { get { return typeof (Mesh); } }

		public override IAssetPart[] Load(string assetName)
		{
			var fileLoader = FileLoaderManager.GetFileLoader(assetName);
			var stream = fileLoader.GetStream(assetName);
			if (null == stream)
			{
				throw new AssetNotFoundException(assetName);
			}
			var streamReader = new StreamReader(stream);

			var lines = new List<string>();
			while (!streamReader.EndOfStream)
			{
				var line = streamReader.ReadLine();
				lines.Add(line);
			}

			var geometries = AseParser.Parse(assetName, lines, _assetLoaderService, ContentManager);
			HandleNormals(geometries);

			return geometries;

		}

		private void HandleNormals(Geometry[] geometries)
		{
			foreach (var geometry in geometries)
			{
				var geometryDetailLevel = geometry.GeometryContext.GetDetailLevel(DetailLevel.Full);
				GeometryHelper.CalculateNormals(geometryDetailLevel);
			}
		}

		public override string[] FileExtensions
		{
			get { return new[] { "ase" }; }
		}
	}
}