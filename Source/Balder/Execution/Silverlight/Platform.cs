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
#if(SILVERLIGHT)
using System;
using System.ComponentModel;
using System.Windows;
using Balder.Content.Silverlight;
using Balder.Display;
using Balder.Input;
using Balder.Display.Silverlight;
using Balder.Input.Silverlight;
using Balder.Materials.Silverlight;
#if(WINDOWS_PHONE)
using Balder.Rendering.Xna;
#else
using Balder.Rendering.Silverlight;
#endif

namespace Balder.Execution.Silverlight
{
	public class Platform : IPlatform
	{
		public event PlatformStateChange BeforeStateChange = (p, s) => { };
		public event PlatformStateChange StateChanged = (p, s) => { };

		public Platform()
		{
			CurrentState = PlatformState.Idle;
			InitializeObjects();
			Initialize();
		}

		private void InitializeObjects()
		{
#if(WINDOWS_PHONE)
            if( IsInDesignMode )
#endif
            {
                DisplayDevice = new DisplayDevice(this);    
            } 
#if(WINDOWS_PHONE)
			else
            {
                DisplayDevice = new Balder.Display.WP7.DisplayDevice();
            }
#endif
			
			MouseDevice = new MouseDevice();
		}

		private void Initialize()
		{
			ChangeState(PlatformState.Initialize);
			ChangeState(PlatformState.Load);
			ChangeState(PlatformState.Run);
		}


		public string PlatformName
		{
			get { return "Silverlight"; }
		}

		public string EntryAssemblyName
		{
			get
			{
				var fullAssemblyName = Application.Current.GetType().Assembly.FullName;
				return fullAssemblyName;
			}
		}

		public bool IsInDesignMode
		{
			get
			{
				var isInDesignMode = DesignerProperties.IsInDesignTool;
#if(!WINDOWS_PHONE)
				if (!isInDesignMode)
				{
					try
					{

						var host = Application.Current.Host.Source;
						isInDesignMode = false;
					}
					catch
					{
						isInDesignMode = true;
					}
				}
#endif
				return isInDesignMode;
			}
		}

		public IDisplayDevice DisplayDevice { get; private set; }
		public IMouseDevice MouseDevice { get; private set; }
		public Type FileLoaderType { get { return typeof(FileLoader); } }
		public Type GeometryContextType { get { return typeof(GeometryContext); } }
		public Type SpriteContextType { get { return typeof(SpriteContext); } }
		public Type ImageContextType { get { return typeof(ImageContext); } }
		public Type ShapeContextType { get { return typeof(ShapeContext); } }
		public Type MaterialCalculatorType { get { return typeof(MaterialCalculator); } }
		public Type SkyboxContextType { get { return typeof(SkyboxContext); } }

		public PlatformState CurrentState { get; private set; }

		private void ChangeState(PlatformState platformState)
		{
			BeforeStateChange(this, platformState);
			CurrentState = platformState;
			StateChanged(this, platformState);
		}
	}
}
#endif