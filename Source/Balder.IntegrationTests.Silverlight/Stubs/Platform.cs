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
using Balder.Assets;
using Balder.Display;
using Balder.Execution;
using Balder.Input;

namespace Balder.IntegrationTests.Silverlight.Stubs
{
	public class Platform : IPlatform
	{
		public event PlatformStateChange BeforeStateChange;
		public event PlatformStateChange StateChanged;
		public string PlatformName { get; private set; }
		public string EntryAssemblyName { get; private set; }
		public bool IsInDesignMode { get; private set; }
		public IDisplayDevice DisplayDevice { get; private set; }
		public IMouseDevice MouseDevice { get; private set; }
		public Type FileLoaderType { get; private set; }
		public Type GeometryContextType { get; private set; }
		public Type SpriteContextType { get; private set; }
		public Type ImageContextType { get; private set; }
		public Type ShapeContextType { get; private set; }
		public PlatformState CurrentState { get; private set; }
		public void RegisterAssetLoaders(IAssetLoaderService assetLoaderService)
		{
			
		}
	}
}
