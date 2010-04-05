#region License

//
// Author: Einar Ingebrigtsen <einar@dolittle.com>
// Copyright (c) 2007-2009, DoLittle Studios
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

namespace Balder.Silverlight.SampleBrowser.Samples.Data.HierarchicalNodesControl
{
	public class ViewModel
	{
		public const int DepthCount = 4;
		public const double DepthSpace = 12;

		public ViewModel()
		{
			Depths = new ObservableCollection<Depth>();

			var position = -(DepthSpace*(DepthCount/2d));
			for( var depthIndex=0; depthIndex<DepthCount; depthIndex++)
			{
				var row = new Depth(position);
				Depths.Add(row);
				position += DepthSpace;
			}
		}

		public ObservableCollection<Depth> Depths { get; private set; }
	}
}
