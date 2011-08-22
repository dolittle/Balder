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
using Balder.Rendering;
using System.Windows;

namespace Balder.Execution
{
	public class PropertyContainer : IPropertyContainer
	{
#if(XAML)
		DependencyObject _owner;
#else
		object _owner;
#endif

#if(XAML)
		public PropertyContainer(DependencyObject owner)
#else
		public PropertyContainer(object owner)
#endif
		{
			_owner = owner;
		}

		public void SetValue<T>(IProperty property, T value)
		{
#if(XAML)
			_owner.SetValue(property.ActualDependencyProperty, value);
#endif
		}

		public T GetValue<T>(IProperty property)
		{
#if(XAML)
			return (T)_owner.GetValue(property.ActualDependencyProperty);
#endif
		}
	}
}
