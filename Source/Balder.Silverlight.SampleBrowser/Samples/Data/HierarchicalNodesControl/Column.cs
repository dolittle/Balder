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
using Balder.Math;
using Balder.Objects.Geometries;

namespace Balder.Silverlight.SampleBrowser.Samples.Data.HierarchicalNodesControl
{
	public class Column : INotifyPropertyChanged
	{
		public Vector Position { get; set; }
		public Color Color { get; set; }

		public string Name { get; set; }
		private Box _box;
		public Box Box
		{
			get { return _box; }
			set
			{
				_box = value;
				OnPropertyChanged("Box");
			}
		}

		public Column(int depth, int row, int column)
		{
			Name = string.Format("Node : {0}, {1}, {2}", depth, row, column);

			var x = Row.ColumnSpace*column;
			var y = Depth.RowSpace*row;
			var z = ViewModel.DepthSpace * depth;
			Position = new Vector((float)x,(float)y,(float)z);
		}

		public override string ToString()
		{
			return string.Format("Column : {0}",Position.ToString());
		}

		public event PropertyChangedEventHandler PropertyChanged = (s, e) => { };
 		private void OnPropertyChanged(string propertyName)
 		{
 			PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
 		}
	}
}
