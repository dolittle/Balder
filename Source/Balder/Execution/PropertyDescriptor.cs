using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Balder.Extensions;

namespace Balder.Execution
{
	public class PropertyDescriptor
	{
		public Type OwnerType { get; private set; }
		public Type PropertyType { get; private set; }
		public PropertyInfo PropertyInfo { get; private set; }
		public List<IProperty> ChildProperties { get; private set; }
		public bool IsUnique { get; private set; }
		public bool CanNotify { get; private set; }
		public bool IsValueType { get; private set; }
		public bool IsCopyable { get; private set; }
#if(SILVERLIGHT)
		public bool IsValueNotifyPropertyChanged { get; private set; }
#endif

		public PropertyDescriptor(Type ownerType, Type propertyType, Expression expression)
		{
			OwnerType = ownerType;
			PropertyType = propertyType;
			PropertyInfo = expression.GetPropertyInfo();
			
			IsValueType = propertyType.IsValueType;
			ChildProperties = new List<IProperty>();
			IsCopyable = PropertyType.HasInterface<ICopyable>();
			CanNotify = OwnerType.HasInterface<ICanNotifyChanges>();
			IsUnique = OwnerType.HasInterface<IAmUnique>();
				
#if(SILVERLIGHT)
			IsValueNotifyPropertyChanged = PropertyType.HasInterface<INotifyPropertyChanged>();
#endif
			PopulateChildProperties();
		}


		private void PopulateChildProperties()
		{
			var fields = PropertyType.GetFields(BindingFlags.Static | BindingFlags.Public);
			var query = from p in fields
			            where p.Name.StartsWith("Property") || p.FieldType.IsGenericType
			            select p;
			foreach (var field in query)
			{
				var property = (IProperty)field.GetValue(null);
				ChildProperties.Add(property);
			}
		}
	}
}