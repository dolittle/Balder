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
using System.IO;
using Balder.Content;
using Balder.Execution;
using Moq;
using Ninject;
using NUnit.Framework;

namespace Balder.Tests.Content
{
	[TestFixture]
	public class FileLoaderManagerTests
	{
		public class DefaultFileLoader : IFileLoader
		{
			public Stream GetStream(string fileName)
			{
				throw new NotImplementedException();
			}

			public bool Exists(string fileName)
			{
				throw new NotImplementedException();
			}

			public string[] SupportedSchemes
			{
				get { return null; }
			}
		}

		public class SomeOtherFileLoader : IFileLoader
		{
			public Stream GetStream(string fileName)
			{
				throw new NotImplementedException();
			}

			public bool Exists(string fileName)
			{
				throw new NotImplementedException();
			}

			public string[] SupportedSchemes
			{
				get { return new[] {"Something"}; }
			}
		}


		[Test]
		public void GettingWithoutASchemeShouldGetDefaultFileLoader()
		{
			var typeDiscovererMock = new Mock<ITypeDiscoverer>();
			typeDiscovererMock.Expect(t => t.FindMultiple<IFileLoader>()).
				Returns(new[]
				        	{
				        		typeof(DefaultFileLoader),
								typeof(SomeOtherFileLoader)
				        	});
			var kernel = new StandardKernel();
			kernel.Bind<IFileLoader>().To(typeof(DefaultFileLoader));

			var defaultFileLoader = new DefaultFileLoader();
			var manager = new FileLoaderManager(defaultFileLoader, typeDiscovererMock.Object, kernel);
			var fileLoader = manager.GetFileLoader("Something");

			Assert.That(fileLoader.GetType(), Is.EqualTo(typeof(DefaultFileLoader)));
			Assert.That(fileLoader, Is.EqualTo(defaultFileLoader));
		}

		[Test]
		public void GettingWithSpecificSchemeShouldReturnCorrectFileLoader()
		{
			var typeDiscovererMock = new Mock<ITypeDiscoverer>();
			typeDiscovererMock.Expect(t => t.FindMultiple<IFileLoader>()).
				Returns(new[]
				        	{
				        		typeof(DefaultFileLoader),
								typeof(SomeOtherFileLoader)
				        	});
			var kernel = new StandardKernel();
			kernel.Bind<IFileLoader>().To(typeof(DefaultFileLoader));

			var defaultFileLoader = new DefaultFileLoader();
			var manager = new FileLoaderManager(defaultFileLoader, typeDiscovererMock.Object, kernel);
			var fileLoader = manager.GetFileLoader("Something://somefile");

			Assert.That(fileLoader.GetType(), Is.EqualTo(typeof(SomeOtherFileLoader)));
			Assert.That(fileLoader, Is.Not.EqualTo(defaultFileLoader));
			
		}
	}
}
