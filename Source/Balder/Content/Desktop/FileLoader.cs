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
#if(DESKTOP)
using System;
using System.IO;

namespace Balder.Content.Desktop
{
	public class FileLoader : IFileLoader
	{
		public Stream GetStream(string fileName)
		{
			var stream = File.OpenRead(fileName);
			return stream;
		}

		public bool Exists(string fileName)
		{
			throw new NotImplementedException();
		}

		public string[] SupportedSchemes
		{
			get { throw new NotImplementedException(); }
		}
	}
}
#endif