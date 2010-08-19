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
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Balder.Silverlight.SampleBrowser.Samples.Materials.Editor
{
	public class MapValueConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var viewModels = Application.Current.Resources["ViewModels"] as ViewModels;
			if( null != viewModels )
			{
				foreach( var mapDescriptor in viewModels.MaterialEditor.Maps )
				{
					if( mapDescriptor.Map == null && value == null )
					{
						return mapDescriptor;
					}
					if( mapDescriptor.Map != null && mapDescriptor.Map.Equals(value))
					{
						return mapDescriptor;
					}
				}
				return viewModels.MaterialEditor.Maps[0];
			}
			return null;
		}	

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var mapDescriptor = (MapDescription) value;
			return mapDescriptor.Map;
		}
	}
}