using System.Collections.ObjectModel;
using Balder.Math;
using Color=Balder.Color;

namespace Balder.Silverlight.SampleBrowser.Samples.Data.NodesStack
{
	public class ViewModel
	{
		public ViewModel()
		{
			Objects = new ObservableCollection<BusinessObject>
			          	{
			          		new BusinessObject {Color = Colors.Red, Position = new Coordinate(-120, 0, 0)},
			          		new BusinessObject {Color = Colors.Green, Position = new Coordinate(0, 0, 0)},
			          		new BusinessObject {Color = Colors.Blue, Position = new Coordinate(120, 0, 0)}
			          	};
		}

		public ObservableCollection<BusinessObject> Objects { get; set; }
	}
}