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

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Balder.Rendering;
#if(XAML)
using System.ComponentModel;
using System.Windows;
#endif

namespace Balder.Execution
{
	public class Property<T,TP> : IProperty
#if(XAML)
		where T:DependencyObject, IHavePropertyContainer
#else
		where T:IHavePropertyContainer
#endif
	{
		private Property(Expression<Func<T, TP>> expression, TP defaultValue, PropertyValueChanged<TP> propertyValueChanged)
		{
			Descriptor = new PropertyDescriptor(typeof(T), typeof(TP), defaultValue, expression);
			Initialize();
		}

		
		public static Property<T, TP> Register(Expression<Func<T, TP>> expression)
		{
			var property = Register(expression, default(TP));
			return property;
		}

		public static Property<T, TP> Register(Expression<Func<T, TP>> expression, PropertyValueChanged<TP> propertyValueChanged)
		{
			var property = Register(expression, propertyValueChanged, default(TP));
			return property;
		}

		public static Property<T, TP> Register(Expression<Func<T, TP>> expression, TP defaultValue)
		{
			var property = Register(expression, null, defaultValue);
			return property;
		}

		public static Property<T, TP> Register(Expression<Func<T, TP>> expression, PropertyValueChanged<TP> propertyValueChanged, TP defaultValue)
		{
			var property = new Property<T, TP>(expression, defaultValue, propertyValueChanged);
			return property;
		}


		public PropertyDescriptor Descriptor { get; private set; }

		public TP GetValue(T obj)
		{
			return obj.PropertyContainer.GetValue<TP>(this);
		}

		public void SetValue(T obj, TP value)
		{
			obj.PropertyContainer.SetValue<TP>(this, value);
		}

#if(XAML)

		public DependencyProperty ActualDependencyProperty { get; private set; }

		private void PropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
		}

		private void Initialize()
		{
			ActualDependencyProperty =
				DependencyProperty.Register(
					Descriptor.PropertyInfo.Name,
					Descriptor.PropertyInfo.PropertyType,
					Descriptor.OwnerType,
					new PropertyMetadata(Descriptor.DefaultValue, PropertyChanged)
				);
		}
#endif
	}
}
