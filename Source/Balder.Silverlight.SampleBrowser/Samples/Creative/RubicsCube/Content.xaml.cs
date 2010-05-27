using Balder.Core.Display;

namespace Balder.Silverlight.SampleBrowser.Samples.Creative.RubicsCube
{
	public partial class Content
	{
		public static IDisplay Display;


		public Content()
		{
			InitializeComponent();

			Loaded += Content_Loaded;
		}

		void Content_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			Display = Game.Display;
		}
	}
}
