using System.Windows;
using System.Windows.Input;
using Balder.Core.Execution;

namespace Balder.Core.Input
{
	public class MouseButtonEventArgs : BubbledEventArgs
	{
		private readonly System.Windows.Input.MouseButtonEventArgs _originalMouseEventArgs;
		private bool _positionSet;

		public MouseButtonEventArgs()
		{
			
		}

		internal MouseButtonEventArgs(System.Windows.Input.MouseButtonEventArgs originalMouseEventArgs, Point position)
		{
			Position = position;
		}

		internal MouseButtonEventArgs(System.Windows.Input.MouseButtonEventArgs originalMouseEventArgs)
		{
			_originalMouseEventArgs = originalMouseEventArgs;
			StylusDevice = originalMouseEventArgs.StylusDevice;
		}


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


		public Point	GetPosition(UIElement relativeTo)
		{
			if( _positionSet )
			{
				return Position;
			}
			if( null == _originalMouseEventArgs )
			{
				return new Point();
			}
			return _originalMouseEventArgs.GetPosition(relativeTo);
		}

		public StylusDevice StylusDevice { get; private set; }
	}
}