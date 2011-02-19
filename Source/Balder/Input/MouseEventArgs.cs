#if(XAML)
using System.Windows;
using System.Windows.Input;
#else
using System.Drawing;
#endif
using Balder.Execution;

namespace Balder.Input
{
	public class MouseEventArgs : BubbledEventArgs
	{
		private bool _positionSet;

		public MouseEventArgs()
		{
		}

#if(XAML)
		private readonly System.Windows.Input.MouseEventArgs _originalMouseEventArgs;

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

		public StylusDevice StylusDevice { get; private set; }

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
#endif

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



		
	}
}