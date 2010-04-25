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

using System;
using System.Text;
using Balder.Core.Assets;
using Balder.Core.Assets.AssetLoaders;
using Balder.Core.Content;
using Balder.Core.Imaging;
using Balder.Core.Objects.Geometries;
using Balder.Core.Rendering;
using Balder.Core.Tests.Fakes;
using CThru.Silverlight;
using Moq;
using NUnit.Framework;

namespace Balder.Core.Tests.Assets.AssetLoaders
{
	public class FakeImageLoader : AssetLoader
	{
		public FakeImageLoader(IFileLoader fileLoader, IContentManager contentManager)
			: base(fileLoader, contentManager)
		{
		}

		public override string[] FileExtensions
		{
			get { return new[] { "jpg", "png" }; }
		}

		public override Type SupportedAssetType
		{
			get { return typeof (Image); }
		}

		public override IAssetPart[] Load(string assetName)
		{
			var images = new[] { new Image() };
			return images;
		}
	}


	[TestFixture]
	public class AseLoaderTests
	{
		private static string GetResourceString(byte[] bytes)
		{
			var str = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
			return str;
		}

		private static Geometry[] LoadGeometries(string resourceName)
		{
			var geometryContext = new FakeGeometryContext();
			var mockContentManager = new Mock<IContentManager>();
			mockContentManager.Expect(c => c.CreateAssetPart<Geometry>()).Returns(() =>
																					{
																						var geometry = new Geometry();
																						geometry.GeometryContext = geometryContext;
																						return geometry;
																					});

			var fileLoader = new StringFileLoader();

			var fakeImageLoader = new FakeImageLoader(fileLoader, mockContentManager.Object);

			var mockAssetLoaderService = new Mock<IAssetLoaderService>();
			mockAssetLoaderService.Expect(a => a.GetLoader<Image>(It.IsAny<string>())).Returns(fakeImageLoader);

			var fileBytes = AseFiles.ResourceManager.GetObject(resourceName) as byte[];

			fileLoader.StringToRead = GetResourceString(fileBytes);
			var aseLoader = new AseLoader(mockAssetLoaderService.Object, fileLoader, mockContentManager.Object);
			var geometries = aseLoader.Load(resourceName + ".ASE");
			return geometries as Geometry[];
		}


		[Test, SilverlightUnitTest]
		[TestCase("SingleBox")]
		public void SingleObjectFileShouldReturnOneGeometry(string aseFile)
		{
			var geometries = LoadGeometries(aseFile);
			Assert.That(geometries.Length, Is.EqualTo(1));
		}


		[Test, SilverlightUnitTest]
		public void SingleObjectFileShouldHaveVerticesLoadedCorrectly()
		{
			var geometries = LoadGeometries("SingleBox");
			var geometryDetailLevel = geometries[0].GeometryContext.GetDetailLevel(DetailLevel.Full);

			var vertices = geometryDetailLevel.GetVertices();
			Assert.That(vertices.Length, Is.EqualTo(8));

			Assert.That(vertices[0].X, Is.EqualTo(-10f));
			Assert.That(vertices[0].Y, Is.EqualTo(-10f));
			Assert.That(vertices[0].Z, Is.EqualTo(-10f));

			Assert.That(vertices[2].X, Is.EqualTo(-10f));
			Assert.That(vertices[2].Y, Is.EqualTo(-10f));
			Assert.That(vertices[2].Z, Is.EqualTo(10f));
		}

		[Test, SilverlightUnitTest]
		public void SingleObjectFileShouldHaveFacesLoadedCorrectly()
		{
			var geometries = LoadGeometries("SingleBox");
			var geometryDetailLevel = geometries[0].GeometryContext.GetDetailLevel(DetailLevel.Full);
			var faces = geometryDetailLevel.GetFaces();
			Assert.That(faces.Length, Is.EqualTo(12));

			Assert.That(faces[0].A, Is.EqualTo(0));
			Assert.That(faces[0].B, Is.EqualTo(2));
			Assert.That(faces[0].C, Is.EqualTo(3));
		}

		[Test, SilverlightUnitTest]
		public void SingleObjectFileWithTextureInfoShouldHaveTextureCoordinatesLoadedCorrectly()
		{
			var geometries = LoadGeometries("SingleBoxWithDiffuseMaterial");
			var geometryDetailLevel = geometries[0].GeometryContext.GetDetailLevel(DetailLevel.Full);
			var textureCoordinates = geometryDetailLevel.GetTextureCoordinates();
			Assert.That(textureCoordinates.Length, Is.EqualTo(36));

			Assert.That(textureCoordinates[0].U, Is.EqualTo(0.9995f));
			Assert.That(textureCoordinates[0].V, Is.EqualTo(0.9995f));

			var faces = geometryDetailLevel.GetFaces();
			Assert.That(faces[0].DiffuseA, Is.EqualTo(8));
			Assert.That(faces[0].DiffuseB, Is.EqualTo(9));
			Assert.That(faces[0].DiffuseC, Is.EqualTo(10));
		}

		[Test, SilverlightUnitTest]
		public void SingleObjectFileWithDiffuseTextureShouldLoadMaterialAndImage()
		{
			var geometries = LoadGeometries("SingleBoxWithDiffuseMaterial");
			Assert.That(geometries[0].Material, Is.Not.Null);
			Assert.That(geometries[0].Material.DiffuseMap, Is.Not.Null);
		}

		[Test, SilverlightUnitTest]
		public void TwoObjectFileShouldReturnTwoGeometries()
		{
			var geometries = LoadGeometries("TwoBoxes");
			Assert.That(geometries.Length, Is.EqualTo(2));
		}

		[Test, SilverlightUnitTest]
		public void TwoObjectFileShouldHavePositionForObjectsReadCorrectly()
		{
			var geometries = LoadGeometries("TwoBoxes");
			Assert.That(geometries[0].Position.X, Is.EqualTo(0d));
			Assert.That(geometries[0].Position.Y, Is.EqualTo(-10d));
			Assert.That(geometries[0].Position.Z, Is.EqualTo(0d));

			Assert.That(geometries[1].Position.X, Is.EqualTo(0.4756d));
			Assert.That(geometries[1].Position.Y, Is.EqualTo(-10d));
			Assert.That(geometries[1].Position.Z, Is.EqualTo(40d));
		}

		[Test, SilverlightUnitTest]
		public void TwoObjectFileShouldHaveScaleForObjectsReadCorrectly()
		{
			var geometries = LoadGeometries("TwoBoxes");
			Assert.That(geometries[0].Scale.X, Is.EqualTo(3d));
			Assert.That(geometries[0].Scale.Y, Is.EqualTo(1d));
			Assert.That(geometries[0].Scale.Z, Is.EqualTo(2d));

			Assert.That(geometries[1].Scale.X, Is.EqualTo(1d));
			Assert.That(geometries[1].Scale.Y, Is.EqualTo(3d));
			Assert.That(geometries[1].Scale.Z, Is.EqualTo(2d));
		}

		[Test, SilverlightUnitTest]
		public void FileWithNormalsShouldLoadFaceNormals()
		{
			var geometries = LoadGeometries("SplitSphere");
			var detailLevel = geometries[0].GeometryContext.GetDetailLevel(DetailLevel.Full);
			Assert.That(detailLevel.NormalCount, Is.EqualTo(482));
			var normals = detailLevel.GetNormals();

			
		}
	}
}