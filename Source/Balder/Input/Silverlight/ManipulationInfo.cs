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
using Balder.Extensions.Silverlight;

namespace Balder.Input.Silverlight
{
	public class ManipulationInfo : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged = (s, e) => {};

		private INode _node;
		public INode Node
		{
			get { return _node; }
			set
			{
				_node = value;
				PropertyChanged.Notify(() => Node);
			}
		}

		private int _deltaX;
		public int DeltaX
		{
			get { return _deltaX; }
			set
			{
				_deltaX = value;
				PropertyChanged.Notify(() => DeltaX);
			}
		}

		private int _deltaY;
		public int DeltaY
		{
			get { return _deltaY; }
			set
			{
				_deltaY = value;
				PropertyChanged.Notify(() => DeltaY);
			}
		}

		private bool _isManipulating;
		public bool IsManipulating
		{
			get { return _isManipulating; }
			set
			{
				_isManipulating = value;
				PropertyChanged.Notify(() => IsManipulating);
			}
		}
	}
}
#endif