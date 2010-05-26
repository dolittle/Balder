using System.Windows;
using System.Windows.Input;
using Balder.Core.Execution;

namespace Balder.Core.Input
{
	public class MouseButtonEventArgs : BubbledEventArgs
	{
		private readonly System.Windows.Input.MouseButtonEventArgs _originalMouseEventArgs;

		internal MouseButtonEventArgs(System.Windows.Input.MouseButtonEventArgs originalMouseEventArgs)
		{
			_originalMouseEventArgs = originalMouseEventArgs;
			StylusDevice = originalMouseEventArgs.StylusDevice;
		}

		public MouseButtonEventArgs()
		{
			
		}

		public Point	GetPosition(UIElement relativeTo)
		{
			if( null == _originalMouseEventArgs )
			{
				return new Point();
			}
			return _originalMouseEventArgs.GetPosition(relativeTo);
		}

		public StylusDevice StylusDevice { get; private set; }
	}
}