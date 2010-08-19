using System.Collections.ObjectModel;
using Balder.Execution;
using Balder.Imaging;
using Balder.Materials;

namespace Balder.Silverlight.SampleBrowser.Samples.Materials.Editor
{
	public class ViewModel
	{

		public ViewModel()
		{
			Maps = new ObservableCollection<MapDescription>();
			PopulateMaps();
		}

		public virtual ObservableCollection<MapDescription> Maps { get; private set; }


		private void PopulateMaps()
		{
			Maps.Add(new MapDescription {Name = "None", Map = null});
			Maps.Add(new MapDescription { Name = "Balder Logo", Map = LoadMap("/Balder.Silverlight.SampleBrowser;component/Assets/BalderLogo.png") });
			Maps.Add(new MapDescription { Name = "Reflection map", Map = LoadMap("/Balder.Silverlight.SampleBrowser;component/Assets/ReflectionMap.jpg") });
			Maps.Add(new MapDescription { Name = "Visual Studio", Map = LoadMap("/Balder.Silverlight.SampleBrowser;component/Assets/VisualStudio.png") });
			Maps.Add(new MapDescription { Name = "Sun", Map = LoadMap("/Balder.Silverlight.SampleBrowser;component/Assets/sun.png") });
		}

		private IMap LoadMap(string file)
		{
			var image = Runtime.Instance.ContentManager.Load<Image>(file);
			var imageMap = new ImageMap(image);
			return imageMap;
		}

	}
}
