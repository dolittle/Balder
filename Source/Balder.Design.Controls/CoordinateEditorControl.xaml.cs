using System.Windows;

namespace Balder.Design.Controls
{
	/// <summary>
	/// Interaction logic for VectorEditorControl.xaml
	/// </summary>
	public partial class CoordinateEditorControl
	{
		public CoordinateEditorControl()
		{
			InitializeComponent();

			DataContextChanged += new DependencyPropertyChangedEventHandler(VectorEditorControl_DataContextChanged);
		}

		void VectorEditorControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			int i = 0;
			i++;
		}
	}
}
