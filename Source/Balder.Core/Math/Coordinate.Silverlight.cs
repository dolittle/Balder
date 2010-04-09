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
using System.ComponentModel;
using System.Windows;
using Balder.Core.Execution;
using Balder.Core.Silverlight.Extensions;
using Balder.Core.Silverlight.TypeConverters;

namespace Balder.Core.Math
{
	[TypeConverter(typeof(CoordinateTypeConverter))]
	public partial class Coordinate : DependencyObject, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public static readonly Property<Coordinate, double> XProp = Property<Coordinate, double>.Register(c => c.X);
		public double X
		{
			get { return XProp.GetValue(this); }
			set
			{
				XProp.SetValue(this, value);
				PropertyChanged.Notify(() => X);
			}
		}

		public static readonly Property<Coordinate, double> YProp = Property<Coordinate, double>.Register(c => c.Y);
		public double Y
		{
			get { return YProp.GetValue(this); }
			set
			{
				YProp.SetValue(this, value);
				PropertyChanged.Notify(() => Y);
			}
		}

		public static readonly Property<Coordinate, double> ZProp = Property<Coordinate, double>.Register(c => c.Z);
		public double Z
		{
			get { return ZProp.GetValue(this); }
			set
			{
				ZProp.SetValue(this, value);
				PropertyChanged.Notify(() => Z);
			}
		}


	}
}
