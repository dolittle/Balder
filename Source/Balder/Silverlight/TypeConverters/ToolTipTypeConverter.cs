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
#if(XAML)
using System.ComponentModel;
using System.Windows.Controls;

namespace Balder.Silverlight.TypeConverters
{
	public class ToolTipTypeConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, System.Type sourceType)
		{
			return sourceType.Equals(typeof (string));
		}

		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
			var toolTip = new ToolTip {Content = value};
			return toolTip;
		}
		
	}
}
#endif