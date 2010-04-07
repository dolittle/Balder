using System;
using System.Collections.Generic;
using Balder.Core.Execution;

namespace Balder.Core
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


		public Node Clone(Node source, bool unique)
		{
			PreClone(source);

			var type = source.GetType();
			var cloneInfo = GetInfoForType(type);
			var clone = Activator.CreateInstance(type) as Node;

			ClonePropertyValues(source, cloneInfo, clone);
			CloneBindingExpressions(source, cloneInfo, clone);

			if( unique ) 
			{
				MakeUnique(source);
			}

			CloneChildren(source, clone, unique);
			PostClone(source, clone);

			return clone;
		}

		private void PostClone(Node source, Node clone)
		{
			if( clone is ICanHandleCloning )
			{
				((ICanHandleCloning)clone).PostClone(source);
			}
		}

		private void PreClone(Node source)
		{
			if (source is ICanHandleCloning)
			{
				((ICanHandleCloning)source).PreClone();
			}
		}

		private void MakeUnique(Node source)
		{
			if (source is ICanBeUnique)
			{
				((ICanBeUnique) source).MakeUnique();
			}
		}

		private void CloneChildren(Node source, Node clone, bool unique)
		{
			foreach (var child in source.Children)
			{
				var clonedChild = child.Clone(unique);
				clone.Children.Add(clonedChild);
			}
		}

		private static void ClonePropertyValues(Node source, NodeCloneInfo cloneInfo, Node clone)
		{
			foreach( var property in cloneInfo.GetProperties() )
			{

				var value = property.PropertyInfo.GetValue(source, null);
				if( property.IsCloneable &&
					null != value )
				{
					value = ((Execution.ICloneable) value).Clone();
				}

				if (null != value)
				{
					property.PropertyInfo.SetValue(clone, value, null);
				}
			}
		}

		private static void CloneBindingExpressions(Node source, NodeCloneInfo cloneInfo, Node clone)
		{
			foreach( var dependencyProperty in cloneInfo.DependencyProperties )
			{
				var bindingExpression = source.GetBindingExpression(dependencyProperty);
				if( null != bindingExpression )
				{
					clone.SetBinding(dependencyProperty, bindingExpression.ParentBinding);
				}
			}
		}
	}
}