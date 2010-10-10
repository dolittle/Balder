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