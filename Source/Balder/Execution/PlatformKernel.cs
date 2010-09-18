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
using Balder.Content;
using Balder.Display;
using Balder.Imaging;
using Balder.Input;
using Balder.Materials;
using Balder.Objects;
using Balder.Objects.Flat;
using Balder.Objects.Geometries;
using Balder.Rendering;
using Ninject;
using Ninject.Activation;

namespace Balder.Execution
{
	public class PlatformKernel : AutoKernel
	{
		public PlatformKernel(Type platformType)
		{
			var platform = this.Get(platformType) as IPlatform;

			Bind<IPlatform>().ToConstant(platform);
			Bind<IDisplayDevice>().ToConstant(platform.DisplayDevice);
			Bind<IMouseDevice>().ToConstant(platform.MouseDevice);
			Bind<IFileLoader>().To(platform.DefaultFileLoaderType).InSingletonScope();
			Bind<IGeometryContext>().To(platform.GeometryContextType);
			Bind<ISpriteContext>().To(platform.SpriteContextType);
			Bind<IImageContext>().To(platform.ImageContextType);
			Bind<IShapeContext>().To(platform.ShapeContextType);
			Bind<IMaterialCalculator>().To(platform.MaterialCalculatorType);
			Bind<ISkyboxContext>().To(platform.SkyboxContextType);
			Bind<IRuntimeContext>().ToMethod(RuntimeContextResolver);
		}

		private IRuntimeContext _currentRuntimeContext;

		private IRuntimeContext RuntimeContextResolver(IContext context)
		{
			if( context.Request.ParentContext == null )
			{
				_currentRuntimeContext = context.Kernel.Get<RuntimeContext>();
			} 

			return _currentRuntimeContext;
		}
	}
}
