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

using Balder.Display;
using Balder.Execution;
using Balder.Materials;
#if(XAML)
using System.ComponentModel;
using System.Windows;
using Balder.Silverlight.TypeConverters;
#endif
#if(DEFAULT_CONSTRUCTOR)
using Ninject;
using Balder.Rendering;
#endif

namespace Balder.Objects
{
#if(XAML)
	public class Skybox : FrameworkElement,
#else
	public class Skybox
#endif
		IHavePropertyContainer
	{
#if(DEFAULT_CONSTRUCTOR)
		public Skybox()
			: this(Runtime.Instance.Kernel.Get<ISkyboxContext>())
		{
			
		}
#endif

		IPropertyContainer _propertyContainer;
		public IPropertyContainer PropertyContainer
		{
			get
			{
				if (_propertyContainer == null)
					_propertyContainer = new PropertyContainer(this);

				return _propertyContainer;

			}
		}

		public Skybox(ISkyboxContext skyboxContext)
		{
			SkyboxContext = skyboxContext;
		}

		public static readonly Property<Skybox, bool> IsEnabledProperty = Property<Skybox, bool>.Register(s => s.IsEnabled, false);
		public bool IsEnabled
		{
			get { return IsEnabledProperty.GetValue(this); }
			set { IsEnabledProperty.SetValue(this, value); }
		}

#if(XAML)
		[TypeConverter(typeof(UriToImageMapTypeConverter))]
#endif
		public IMap Front { get; set; }

#if(XAML)
		[TypeConverter(typeof(UriToImageMapTypeConverter))]
#endif
		public IMap Back { get; set; }

#if(XAML)
		[TypeConverter(typeof(UriToImageMapTypeConverter))]
#endif
		public IMap Top { get; set; }

#if(XAML)
		[TypeConverter(typeof(UriToImageMapTypeConverter))]
#endif
		public IMap Bottom { get; set; }

#if(XAML)
		[TypeConverter(typeof(UriToImageMapTypeConverter))]
#endif
		public IMap Left { get; set; }

#if(XAML)
		[TypeConverter(typeof(UriToImageMapTypeConverter))]
#endif
		public IMap Right { get; set; }


		public ISkyboxContext SkyboxContext { get; private set; }
	}
}