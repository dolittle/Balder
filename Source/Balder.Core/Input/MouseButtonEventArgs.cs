#if(SILVERLIGHT)
using System.Windows;
using System.Windows.Input;
#else
using System.Drawing;
#endif
using Balder.Core.Execution;

namespace Balder.Core.Input
{
	public class MouseButtonEventArgs : BubbledEventArgs
	{
		private bool _positionSet;

		public MouseButtonEventArgs()
		{
			
		}

#if(SILVERLIGHT)
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