using System.ComponentModel;
using Balder.Extensions.Silverlight;

namespace Balder.Input.Silverlight
{
	public class MouseInfo : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged = (s, e) => { };

		private int _xPosition;
		public int XPosition
		{
			get { return _xPosition; }
			set
			{
				_xPosition = value;
				PropertyChanged.Notify(() => XPosition);
			}
		}

		private int _yPosition;
		public int YPosition
		{
			get { return _yPosition; }
			set
			{
				_yPosition = value;
				PropertyChanged.Notify(() => YPosition);
			}
		}
	}
}