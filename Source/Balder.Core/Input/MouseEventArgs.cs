using System.Windows;
using System.Windows.Input;
using Balder.Core.Execution;

namespace Balder.Core.Input
{
	public class MouseEventArgs : BubbledEventArgs
	{
		private readonly System.Windows.Input.MouseEventArgs _originalMouseEventArgs;
		private bool _positionSet;

		public MouseEventArgs()
		{
		}

		internal MouseEventArgs(System.Windows.Input.MouseEventArgs originalMouseEventArgs, Point position)
			: this(originalMouseEventArgs)
		{
			Position = position;
		}

		internal MouseEventArgs(System.Windows.Input.MouseEventArgs originalMouseEventArgs)
		{
			_originalMouseEventArgs = originalMouseEventArgs;
			StylusDevice = originalMouseEventArgs.StylusDevice;
		}

		internal MouseEventArgs(Point position)
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