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
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;
using Balder.Materials;

namespace Balder.Silverlight.SampleBrowser.Samples.Materials.Editor
{
	public class ShadeValueConverter : IValueConverter
	{

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			switch ((MaterialShade)value)
			{
				case MaterialShade.None:
					{
						return 0;
					}
				case MaterialShade.Flat:
					{
						return 1;
					}
				case MaterialShade.Gouraud:
					{
						return 2;
					}
				case MaterialShade.Phong:
					{
						return 3;
					}
			}

			return 0;

		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			switch( (int) value)
			{
				case 0:
					{
						return MaterialShade.None;
					}
				case 1:
					{
						return MaterialShade.Flat;
					}
				case 2:
					{
						return MaterialShade.Gouraud;
					}
				case 3:
					{
						return MaterialShade.Phong;
					}
			}

			return MaterialShade.None;
		}
	}
}