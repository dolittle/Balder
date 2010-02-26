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
using Balder.Core.Execution;
using Balder.Core.Objects.Geometries;

namespace Balder.Core.Content
{
	/// <summary>
	/// Handles the creation of content programatically
	/// </summary>
	public class ContentCreator
	{
		private readonly IObjectFactory _objectFactory;

		/// <summary>
		/// Creates a new ContentCreator and provides functionality for creating content
		/// </summary>
		/// <param name="objectFactory">the IObjectFactory that the ContentCreator will use for creating content</param>
		public ContentCreator(IObjectFactory objectFactory)
		{
			_objectFactory = objectFactory;
		}

		/// <summary>
		/// Creates a geometry based on the geometry type
		/// </summary>
		/// <typeparam name="T">Type of geometry to create</typeparam>
		/// <returns>An instance of the geometry created</returns>
		public T CreateGeometry<T>() where T : Geometry
		{
			var geometry = _objectFactory.Get<T>();
			return geometry;
		}
	}
}
