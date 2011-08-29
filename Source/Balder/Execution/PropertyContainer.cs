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
#if(XAML)
using System.ComponentModel;
using System.Collections.Generic;
using System;
#endif

namespace Balder.Execution
{
	public class PropertyContainer : IPropertyContainer
	{
#if(XAML)
		DependencyObject _owner;
#else
		object _owner;
#endif

		Dictionary<IProperty, object> _values = new Dictionary<IProperty, object>();
		bool _localOnlyEnabled = false;

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
			if (!_localOnlyEnabled)
			{
				Action a = () => _owner.SetValue(property.ActualDependencyProperty, value);
				if (Deployment.Current.Dispatcher.CheckAccess())
					a();
				else
					Deployment.Current.Dispatcher.BeginInvoke(a);
			}
#endif
			PropertyChanged(property, value, _values.ContainsKey(property)?_values[property]:null);
			_values[property] = value;
		}

		public T GetValue<T>(IProperty property)
		{
			if( _values.ContainsKey(property) ) 
				return (T)_values[property];

			return (T)property.DefaultValue;
		}


		public void PropertyChanged<T>(IProperty property, T oldValue, T newValue)
		{
#if(XAML)
			if( property.IsValueNotifyPropertyChanged )
			{
				if (property.IsValueNotifyPropertyChanged)
				{
					PropertyChangedEventHandler handler = (s, e) =>
					{
						property.OnPropertyValueChanged(_owner, oldValue, newValue);
						SignalRendering();
					};

					if (oldValue != null)
						((INotifyPropertyChanged)oldValue).PropertyChanged -= handler;

					if (newValue != null)
						((INotifyPropertyChanged)newValue).PropertyChanged += handler;
				}
			}
#endif
			SignalRendering();
		}

		public void BeginLocal()
		{
			_localOnlyEnabled = true;
		}

		public void EndLocal()
		{
			_localOnlyEnabled = false;
		}


		void ChildPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			SignalRendering();
		}

		void SignalRendering()
		{
			if (_owner is IHaveRuntimeContext)
				if (((IHaveRuntimeContext)_owner).RuntimeContext != null)
					((IHaveRuntimeContext)_owner).RuntimeContext.SignalRendering();
		}


	}
}
