#region License

//
// Author: Einar Ingebrigtsen <einar@dolittle.com>
// Copyright (c) 2007-2009, DoLittle Studios
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

using System.Text;
using Balder.Core.Assets.AssetLoaders;
using Balder.Core.Content;
using Balder.Core.Objects.Geometries;
using Balder.Core.Tests.Assets.AssetLoaders;
using Balder.Core.Tests.Fakes;
using CThru.Silverlight;
using Moq;
using NUnit.Framework;

namespace Balder.Core.Tests.AssetLoaders
{


	[TestFixture]
	public class AseLoaderTests
	{
		private string GetResourceString(byte[] bytes)
		{
			var str = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
			return str;
		}

		private Geometry[] LoadGeometries(string resourceName)
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
			var fileBytes = AseFiles.ResourceManager.GetObject(resourceName) as byte[];

			fileLoader.StringToRead = GetResourceString(fileBytes);
			var aseLoader = new AseLoader(null, fileLoader, mockContentManager.Object);
			var geometries = aseLoader.Load(string.Empty);
			return geometries;
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

			var vertices = geometries[0].GeometryContext.GetVertices();
			Assert.That(vertices.Length, Is.EqualTo(8));

			Assert.That(vertices[0].Vector.X, Is.EqualTo(-10f));
			Assert.That(vertices[0].Vector.Y, Is.EqualTo(-10f));
			Assert.That(vertices[0].Vector.Z, Is.EqualTo(-10f));

			Assert.That(vertices[2].Vector.X, Is.EqualTo(-10f));
			Assert.That(vertices[2].Vector.Y, Is.EqualTo(-10f));
			Assert.That(vertices[2].Vector.Z, Is.EqualTo(10f));
		}

		[Test, SilverlightUnitTest]
		public void SingleObjectFileShouldHaveFacesLoadedCorrectly()
		{
			var geometries = LoadGeometries("SingleBox");

			var faces = geometries[0].GeometryContext.GetFaces();
			Assert.That(faces.Length, Is.EqualTo(12));

			Assert.That(faces[0].A, Is.EqualTo(0));
			Assert.That(faces[0].B, Is.EqualTo(2));
			Assert.That(faces[0].C, Is.EqualTo(3));
		}

		[Test, SilverlightUnitTest]
		public void SingleObjectFileWithTextureInfoShouldHaveTextureCoordinatesLoadedCorrectly()
		{
			var geometries = LoadGeometries("SingleBoxWithDiffuseMaterial");

			var textureCoordinates = geometries[0].GeometryContext.GetTextureCoordinates();
			Assert.That(textureCoordinates.Length, Is.EqualTo(36));

			Assert.That(textureCoordinates[0].U, Is.EqualTo(0.9995f));
			Assert.That(textureCoordinates[0].V, Is.EqualTo(0.9995f));

			var faces = geometries[0].GeometryContext.GetFaces();
			Assert.That(faces[0].DiffuseA, Is.EqualTo(8));
			Assert.That(faces[0].DiffuseB, Is.EqualTo(9));
			Assert.That(faces[0].DiffuseC, Is.EqualTo(10));
		}

		[Test, SilverlightUnitTest]
		public void TwoObjectFileShouldReturnTwoGeometries()
		{
			var geometries = LoadGeometries("TwoBoxes");
			Assert.That(geometries.Length, Is.EqualTo(2));
		}


	}
}

