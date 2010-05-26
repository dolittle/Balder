using System.Windows;
using System.Windows.Input;
using Balder.Core.Execution;

namespace Balder.Core.Input
{
	public class MouseEventArgs : BubbledEventArgs
	{
		private readonly System.Windows.Input.MouseEventArgs _originalMouseEventArgs;

		internal MouseEventArgs(System.Windows.Input.MouseEventArgs originalMouseEventArgs)
		{
			_originalMouseEventArgs = originalMouseEventArgs;
			StylusDevice = originalMouseEventArgs.StylusDevice;
		}

		public MouseEventArgs()
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