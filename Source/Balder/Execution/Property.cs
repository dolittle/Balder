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
using System.Linq;
using System.Linq.Expressions;
using Balder.Rendering;
using Balder.Extensions;
#if(XAML)
using System.ComponentModel;
using System.Windows;
using System.Reflection;
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
		public Type OwnerType { get; private set; }
		public Type PropertyType { get; private set; }
		public PropertyInfo PropertyInfo { get; private set; }
		public IEnumerable<IProperty> ChildProperties { get; private set; }
		public bool IsUnique { get; private set; }
		public bool CanNotify { get; private set; }
		public bool IsValueType { get; private set; }
		public bool IsCopyable { get; private set; }
		public object DefaultValue { get; private set; }

#if(XAML)
		public bool IsValueNotifyPropertyChanged { get; private set; }
		public DependencyProperty ActualDependencyProperty { get; private set; }
#endif


		Property(Expression<Func<T, TP>> expression, TP defaultValue, PropertyValueChanged<TP> propertyValueChanged)
		{
			DefaultValue = defaultValue;
			OwnerType = typeof(T);
			PropertyType = typeof(TP);
			PropertyInfo = expression.GetPropertyInfo();

			IsValueType = PropertyType.IsValueType;
			ChildProperties = new List<IProperty>();
			IsCopyable = PropertyType.HasInterface<ICopyable>();
			CanNotify = OwnerType.HasInterface<ICanNotifyChanges>();
			IsUnique = OwnerType.HasInterface<IAmUnique>();
			PopulateChildProperties();

#if(XAML)
			IsValueNotifyPropertyChanged = PropertyType.HasInterface<INotifyPropertyChanged>();
			InitializeDependencyProperty();
#endif
		}

		void PopulateChildProperties()
		{
			var childProperties = new List<IProperty>();
			var fields = PropertyType.GetFields(BindingFlags.Static | BindingFlags.Public);
			var query = from p in fields
						where p.Name.StartsWith("Property") || p.FieldType.IsGenericType
						select p;
			foreach (var field in query)
			{
				var property = (IProperty)field.GetValue(null);
				childProperties.Add(property);
			}

			ChildProperties = childProperties;
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


		private void PropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
		}

		private void InitializeDependencyProperty()
		{
			ActualDependencyProperty =
				DependencyProperty.Register(
					PropertyInfo.Name,
					PropertyInfo.PropertyType,
					OwnerType,
					new PropertyMetadata(DefaultValue, PropertyChanged)
				);
		}
#endif
	}
}
