using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Balder.Core.Math;
using Balder.Silverlight.MVVM;
using Color = Balder.Core.Color;

namespace Balder.Silverlight.SampleBrowser.Samples.Data.NodesControl
{
	public class ViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged = (s, e) => { };

		public ViewModel()
		{
			ManyObjects = new ObservableCollection<BusinessObject>();
			FewObjects = new ObservableCollection<BusinessObject>();

			Populate(ManyObjects,10,10);
			Populate(FewObjects,3,3);

			Objects = ManyObjects;

			SwitchCommand = DelegateCommand.Create(Switch);
		}

		private void Populate(ObservableCollection<BusinessObject> destination, int xCount, int zCount)
		{
			var zDistance = 12d;
			var xDistance = 12d;

			var currentZ = -((zCount / 2d) * zDistance);
			for (var z = 0; z < zCount; z++)
			{
				var currentX = -((xCount / 2d) * xDistance);
				for (var x = 0; x < xCount; x++)
				{
					var obj = new BusinessObject
					{
						Position = new Coordinate(currentX, 0, currentZ),
						Color = Color.Random()
					};
					destination.Add(obj);
					currentX += xDistance;
				}

				currentZ += zDistance;
			}
		}


		private ObservableCollection<BusinessObject> _objects;
		public ObservableCollection<BusinessObject> Objects
		{
			get { return _objects; }
			set
			{
				_objects = value;
				OnPropertyChanged("Objects");
			}
		}

		public ObservableCollection<BusinessObject> ManyObjects { get; set; }
		public ObservableCollection<BusinessObject> FewObjects { get; set; }
		

		private void OnPropertyChanged(string propertyName)
		{
			PropertyChanged(this,new PropertyChangedEventArgs(propertyName));
		}


		public ICommand SwitchCommand { get; private set; }
		public void Switch()
		{
			if( Objects.Equals(ManyObjects))
			{
				Objects = FewObjects;
			} else
			{
				Objects = ManyObjects;
			}
			
		}
	}
}