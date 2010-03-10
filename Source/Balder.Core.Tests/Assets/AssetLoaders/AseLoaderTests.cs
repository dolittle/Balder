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

		[Test,SilverlightUnitTest]
		public void SingleObjectWithoutMaterialsShouldReturnOneGeometry()
		{
			var geometryContext = new FakeGeometryContext();
			var mockContentManager = new Mock<IContentManager>();
			mockContentManager.Expect(c => c.CreateAssetPart<Geometry>()).Returns(()=>
			                                                                      	{
			                                                                      		var geometry = new Geometry();
			                                                                      		geometry.GeometryContext = geometryContext;
			                                                                      		return geometry;
			                                                                      	});
			var fileLoader = new StringFileLoader();
			
			fileLoader.StringToRead = GetResourceString(AseFiles.SingleBox);
			var aseLoader = new AseLoader(null, fileLoader, mockContentManager.Object);
			var geometries = aseLoader.Load(string.Empty);
			Assert.That(geometries.Length,Is.EqualTo(1));
		}

	}
}

