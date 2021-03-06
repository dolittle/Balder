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

using System.Collections.ObjectModel;

namespace Balder.Silverlight.SampleBrowser.Samples.Data.InstancingNodesControl
{
	public class Depth
	{
		private readonly double _zPosition;


		public Depth(double zPosition, int depth)
		{
			_zPosition = zPosition;
			Rows = new ObservableCollection<Row>();

			var position = -(ViewModel.RowSpace * (ViewModel.RowCount / 2d));
			for (var rowIndex = 0; rowIndex < ViewModel.RowCount; rowIndex++)
			{
				var row = new Row(position, zPosition, depth, rowIndex);
				Rows.Add(row);
				position += ViewModel.RowSpace;
			}
		}

		public ObservableCollection<Row> Rows { get; private set; }

		public override string ToString()
		{
			return string.Format("Depth : {0}", _zPosition);
		}
	}
}
