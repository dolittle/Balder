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
using System.Collections.Generic;
using System.Linq;
#if(SILVERLIGHT)
using System.Windows;
#endif
using Ninject;

namespace Balder.Core.Execution
{
	[Singleton]
	public class TypeDiscoverer : ITypeDiscoverer
	{
		private readonly List<Type> _types;

		public TypeDiscoverer()
		{
			_types = new List<Type>();
			CollectTypes();
		}

#if(SILVERLIGHT)
		private void CollectTypes()
		{

			if (null != Deployment.Current)
			{
				var parts = Deployment.Current.Parts;
				foreach (var part in parts)
				{
					AddTypesFromPart(part);
				}
			}
		}

		private void AddTypesFromPart(AssemblyPart part)
		{
			var info = Application.GetResourceStream(new Uri(part.Source, UriKind.Relative));
			var assembly = part.Load(info.Stream);
			var types = assembly.GetTypes();
			_types.AddRange(types);
		}
#else
		private void CollectTypes()
		{
			
		}
#endif

		private Type[] Find<T>()
		{
			var type = typeof(T);
			var query = from t in _types
						where type.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract
						select t;
			var typesFound = query.ToArray();
			return typesFound;
		}


		public Type FindSingle<T>()
		{
			var typesFound = Find<T>();

			if( typesFound.Length > 1 )
			{
				throw new ArgumentException(string.Format("More than one type found for '{0}'",typeof(T).FullName));
			}
			return typesFound.SingleOrDefault();
		}

		public Type[] FindMultiple<T>()
		{
			var typesFound = Find<T>();
			return typesFound;
		}
	}
}
