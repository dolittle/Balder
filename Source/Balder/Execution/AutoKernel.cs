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
using Ninject;
using Ninject.Activation;
using Ninject.Modules;


namespace Balder.Execution
{
	/// <summary>
	/// Represents the method that will handle resolving a binding
	/// </summary>
	/// <param name="context">Binding context to resolve in</param>
	/// <returns>The resolved binding - null if it didn't solve it</returns>
	public delegate bool BindingResolver(IRequest context);

	/// <summary>
	/// Represents the method that will handle resolving a binding with type information
	/// </summary>
	/// <param name="type">Type to resolve</param>
	/// <param name="context">Binding context to resolve in</param>
	/// <returns>The resolved binding - null if it didn't solve it</returns>
	public delegate bool GenericBindingResolver(Type type, IRequest context);


	/// <summary>
	/// Represents a kernel that can automatically resolve bindings based on conventions
	/// </summary>
	public class AutoKernel : StandardKernel
	{
		private readonly Dictionary<Type, BindingResolver> _bindingResolvers;
		private readonly List<GenericBindingResolver> _genericBindingResolvers;

		/// <summary>
		/// Creates an instance of the <see cref="AutoKernel"/>
		/// </summary>
		/// <param name="modules"></param>
		public AutoKernel(params INinjectModule[] modules)
			: base(modules)
		{
			_bindingResolvers = new Dictionary<Type, BindingResolver>();
			_genericBindingResolvers = new List<GenericBindingResolver>();
		}

		/// <summary>
		/// Add a binding resolver
		/// </summary>
		/// <typeparam name="T">Type to add resolver for</typeparam>
		/// <param name="resolver"><see cref="BindingResolver"/> to use for resolving specific type</param>
		public void AddBindingResolver<T>(BindingResolver resolver)
		{
			_bindingResolvers[typeof(T)] = resolver;
		}

		/// <summary>
		/// Add a binding resolver for any type
		/// </summary>
		/// <param name="resolver"><see cref="GenericBindingResolver"/> to use for resolving any type</param>
		public void AddGenericBindingResolver(GenericBindingResolver resolver)
		{
			_genericBindingResolvers.Add(resolver);
		}


		public override IEnumerable<object> Resolve(IRequest request)
		{
			var service = request.Service;

			var resolved = false;
			foreach (var resolver in _genericBindingResolvers)
			{
				if (resolver(service, request))
				{
					resolved = true;
					break;
				}
			}

			if (!resolved)
			{
				if (!base.CanResolve(request))
				{
					if (_bindingResolvers.ContainsKey(service))
					{
						resolved = _bindingResolvers[service](request);
					}


					if (!resolved)
					{
						var serviceName = service.Name;
						if (serviceName.StartsWith("I"))
						{
							var instanceName = string.Format("{0}.{1}", service.Namespace, serviceName.Substring(1));
							var serviceInstanceType = service.Assembly.GetType(instanceName);
							if (null != serviceInstanceType)
							{
								if (serviceInstanceType.IsAbstract)
								{
									return null;
								}

								var attributes = serviceInstanceType.GetCustomAttributes(typeof(SingletonAttribute), false);
								if (attributes.Length == 1)
								{
									Bind(service).To(serviceInstanceType).InSingletonScope();
								}
								else
								{
									Bind(service).To(serviceInstanceType);
								}
							}
						}
					}
				}
			}

			return base.Resolve(request);
		}
	}
}