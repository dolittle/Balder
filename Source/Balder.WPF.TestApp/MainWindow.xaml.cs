using System.Windows.Media;

namespace Balder.WPF.TestApp
{
	public partial class MainWindow
	{
		public MainWindow()
		{
			InitializeComponent();

            CompositionTarget.Rendering += CompositionTarget_Rendering;
		}

        void CompositionTarget_Rendering(object sender, System.EventArgs e)
        {
            _object.Rotation.Y += 0.5;
        }
	}
}
