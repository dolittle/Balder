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
using System.Windows;
using System.Windows.Input;
#else
using System.Drawing;
#endif
using Balder.Execution;

namespace Balder.Input
{
	public class MouseButtonEventArgs : BubbledEventArgs
	{
		private bool _positionSet;

		public MouseButtonEventArgs()
		{
			
		}

#if(XAML)
		private readonly System.Windows.Input.MouseButtonEventArgs _originalMouseEventArgs;
		internal MouseButtonEventArgs(System.Windows.Input.MouseButtonEventArgs originalMouseEventArgs, Point position)
		{
			Position = position;
		}

		internal MouseButtonEventArgs(System.Windows.Input.MouseButtonEventArgs originalMouseEventArgs)
		{
			_originalMouseEventArgs = originalMouseEventArgs;
			StylusDevice = originalMouseEventArgs.StylusDevice;
		}

		public Point GetPosition(UIElement relativeTo)
		{
			if (_positionSet)
			{
				return Position;
			}
			if (null == _originalMouseEventArgs)
			{
				return new Point();
			}
			return _originalMouseEventArgs.GetPosition(relativeTo);
		}

		public StylusDevice StylusDevice { get; private set; }
#endif

		internal MouseButtonEventArgs(Point position)
		{
			Position = position;
		}

		private Point _position;
		public Point Position
		{
			get { return _position; }
			private set
			{
				_position = value;
				_positionSet = true;
			}
		}


	}
}