using System;
using System.Collections.Generic;
#if(SILVERLIGHT)
using System.Windows;
#endif

using Balder.Execution;
using Balder.Rendering;

namespace Balder
{
	public class NodeCloner
	{
		public static readonly NodeCloner Instance = new NodeCloner();
		private readonly Dictionary<Type, NodeCloneInfo> _typeCloneInfo;

		private NodeCloner()
		{
			_typeCloneInfo = new Dictionary<Type, NodeCloneInfo>();
		}


		private NodeCloneInfo GetInfoForType(Type type)
		{
			NodeCloneInfo info;
			if (_typeCloneInfo.ContainsKey(type))
			{
				info = _typeCloneInfo[type];
			}
			else
			{
				info = new NodeCloneInfo(type);
				_typeCloneInfo[type] = info;
			}
			return info;
		}


		public INode Clone(INode source, bool unique)
		{
			PreClone(source);

			var type = source.GetType();
			var cloneInfo = GetInfoForType(type);
			var clone = Activator.CreateInstance(type) as INode;

			ClonePropertyValues(source, cloneInfo, clone);
#if(SILVERLIGHT)
			CloneBindingExpressions(source, cloneInfo, clone);
#endif

			if( unique ) 
			{
				MakeUnique(source);
			}

			CloneChildren(source, clone, unique);
			PostClone(source, clone);

			return clone;
		}

		private void PostClone(INode source, INode clone)
		{
			if( clone is ICanHandleCloning )
			{
				((ICanHandleCloning)clone).PostClone(source);
			}
		}

		private void PreClone(INode source)
		{
			if (source is ICanHandleCloning)
			{
				((ICanHandleCloning)source).PreClone();
			}
		}

		private void MakeUnique(INode source)
		{
			if (source is ICanBeUnique)
			{
				((ICanBeUnique) source).MakeUnique();
			}
		}

		private void CloneChildren(INode source, INode clone, bool unique)
		{
			if (source is IHaveChildren &&
				clone is IHaveChildren)
			{
				foreach (var child in ((IHaveChildren)source).Children)
				{
					if (child is ICanBeCloned)
					{
						var clonedChild = ((ICanBeCloned)child).Clone(unique) as INode;
						((IHaveChildren)clone).Children.Add(clonedChild);
					}
				}
			}
		}

		private static void ClonePropertyValues(INode source, NodeCloneInfo cloneInfo, INode clone)
		{
			foreach( var property in cloneInfo.GetProperties() )
			{
				var value = property.PropertyInfo.GetValue(source, null);
				var cloneValue = property.PropertyInfo.GetValue(clone, null);

				if (ShouldPropertyBeCloned(value, cloneValue))
				{
					if (property.IsCopyable &&
						null != value &&
						null != cloneValue)
					{
						((Execution.ICopyable)value).CopyTo(cloneValue);
					}
					else
					{
						if (property.IsCloneable &&
						    null != value)
						{
							value = ((ICanBeCloned) value).Clone();
						}

						if (null != value)
						{
							property.PropertyInfo.SetValue(clone, value, null);
						}
					}
				}
			}
		}

		private static bool ShouldPropertyBeCloned(object newValue, object existingValue)
		{
			if ((null == (object)existingValue && null != (object)newValue) ||
				null == (object)newValue || !newValue.Equals(existingValue))
			{
				return true;
			}
			return false;
		}

		

#if(SILVERLIGHT)
		private static void CloneBindingExpressions(INode source, NodeCloneInfo cloneInfo, INode clone)
		{
			if (source is FrameworkElement &&
				clone is FrameworkElement)
			{
				var sourceAsframeworkElement = source as FrameworkElement;
				var cloneAsFrameworkElement = clone as FrameworkElement;

				foreach (var dependencyProperty in cloneInfo.DependencyProperties)
				{
					var bindingExpression = sourceAsframeworkElement.GetBindingExpression(dependencyProperty);
					if (null != bindingExpression)
					{
						cloneAsFrameworkElement.SetBinding(dependencyProperty, bindingExpression.ParentBinding);
					}
				}
			}
		}
#endif
	}
}