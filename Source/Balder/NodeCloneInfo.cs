using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Balder.Core.Execution;
#if(SILVERLIGHT)
using System.Windows;
using Balder.Core.Silverlight.Helpers;
#endif

namespace Balder.Core
{
	public class NodeCloneInfo
	{
		public Type Type { get; private set; }
		public Dictionary<PropertyInfo,NodeClonePropertyInfo> Properties { get; private set; }

#if(SILVERLIGHT)
		public DependencyProperty[] DependencyProperties { get; private set; }
		private FieldInfo[] _dependencyPropertyFields;
#endif


		public NodeCloneInfo(Type type)
		{
			Type = type;
			Properties = new Dictionary<PropertyInfo, NodeClonePropertyInfo>();
#if(SILVERLIGHT)
			PrepareDependencyProperties();
#endif
			PrepareProperties();
		}

		private void PrepareProperties()
		{
			var propertiesInType = Type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
			foreach (var property in propertiesInType)
			{
				if (!ShouldPropertyBeIgnored(property))
				{
					var cloneableInterface = property.PropertyType.GetInterface(typeof (ICanBeCloned).Name, false);
					var isCloneable = null != cloneableInterface;
					var copyableInterface = property.PropertyType.GetInterface(typeof (ICopyable).Name, false);
					var isCopyable = null != copyableInterface;
					var nodeCloneProperty = new NodeClonePropertyInfo
					                        	{
					                        		PropertyInfo = property,
					                        		IsCloneable = isCloneable,
													IsCopyable = isCopyable
					                        	};
					Properties[property] = nodeCloneProperty;
				}
			}
		}


		private bool ShouldPropertyBeIgnored(PropertyInfo property)
		{
			if (property.Name.Equals("Children") ||
				!property.CanWrite ||
				property.DeclaringType.FullName.StartsWith("System") 				
				)
			{
				return true;
			}
			return false;
		}


		public bool IsPropertyCloneable(PropertyInfo property)
		{
			var nodeClonePropertyInfo = Properties[property];
			return nodeClonePropertyInfo.IsCloneable;
		}

		public NodeClonePropertyInfo[]	GetProperties()
		{
			var properties = Properties.Values.ToArray();
			return properties;
		}



#if(SILVERLIGHT)
		private bool IsPropertyDependencyProperty(PropertyInfo property)
		{
			var propName = string.Format("{0}Prop", property.Name);
			var propertyName = string.Format("{0}Property", property.Name);
			var query = from f in _dependencyPropertyFields
						where f.Name.Equals(propName) || f.Name.Equals(propertyName)
						select f;
			var field = query.SingleOrDefault();
			return null != field;
		}

		private void PrepareDependencyProperties()
		{
			var dependencyPropertyFields = new List<FieldInfo>();
			var dependencyProperties = new List<DependencyProperty>();
			var fields = Type.GetFields(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Static);
			foreach (var field in fields)
			{
				var value = field.GetValue(null);
				if (field.FieldType.IsGenericType)
				{
					var propertyType = typeof(Property<,>);
					var dependencyPropertyType = typeof(DependencyProperty<,>);
					if (field.FieldType.FullName.StartsWith(propertyType.FullName) ||
						field.FieldType.FullName.StartsWith(dependencyPropertyType.FullName))
					{
						var actualDependencyPropertyProperty = field.FieldType.GetProperty("ActualDependencyProperty");
						value = actualDependencyPropertyProperty.GetValue(value, null);
					}
				}
				if (value is DependencyProperty)
				{
					var dependencyProperty = value as DependencyProperty;
					dependencyProperties.Add(dependencyProperty);
					dependencyPropertyFields.Add(field);
				}
			}

			DependencyProperties = dependencyProperties.ToArray();
			_dependencyPropertyFields = dependencyPropertyFields.ToArray();
		}
#endif

	}
}