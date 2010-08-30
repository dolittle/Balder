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
using System.ComponentModel;
using Balder.Extensions.Silverlight;

namespace Balder.Silverlight.SampleBrowser.Samples.Data.InstancingNodesControl
{
	public class ViewModel : INotifyPropertyChanged
	{
		public const int RowCount = 10;
		public const int ColumnCount = 10;
		public const int DepthCount = 10;

		public const double RowSpace = 12;
		public const double ColumnSpace = 12;
		public const double DepthSpace = 12;

		private ObservableCollection<Depth> _depths;
		public ObservableCollection<Depth> Depths
		{
			get { return _depths; }
			private set
			{
				_depths = value;
				PropertyChanged.Notify(() => Depths);
			}
		}

		public void GenerateData()
		{
			Depths = new ObservableCollection<Depth>();
			var position = -(DepthSpace * (DepthCount / 2d));
			for (var depthIndex = 0; depthIndex < DepthCount; depthIndex++)
			{
				var row = new Depth(position, depthIndex);
				Depths.Add(row);
				position += DepthSpace;
			}

		}


		public void Clear()
		{
			Depths = null;
		}


		public event PropertyChangedEventHandler PropertyChanged = (s, e) => { };
	}
}
