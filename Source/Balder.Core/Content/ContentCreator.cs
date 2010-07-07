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
using Balder.Core.Materials;
using Balder.Core.Objects.Geometries;

namespace Balder.Core.Content
{
	/// <summary>
	/// Handles the creation of content programatically
	/// </summary>
	public class ContentCreator
	{
		private readonly IObjectFactory _objectFactory;
		private readonly IIdentityManager _identityManager;

		/// <summary>
		/// Creates a new ContentCreator and provides functionality for creating content
		/// </summary>
		/// <param name="objectFactory">The IObjectFactory that the ContentCreator will use for creating content</param>
		/// <param name="identityManager">IdentityManager used during creation of certain content</param>
		public ContentCreator(IObjectFactory objectFactory, IIdentityManager identityManager)
		{
			_objectFactory = objectFactory;
			_identityManager = identityManager;
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

		/// <summary>
		/// Creates a material
		/// </summary>
		/// <returns>An instance of a Material</returns>
		public Material CreateMaterial()
		{
			var material = new Material(_identityManager);
			return material;
		}
	}
}
