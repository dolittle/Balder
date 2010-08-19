using System.Windows.Media;
using Balder.Execution;

namespace Balder.Silverlight.SampleBrowser.Samples.Materials.Editor
{
	public partial class ColorEditor
	{
		public ColorEditor()
		{
			InitializeComponent();

			ColorPicker.SelectedColorChanged += ColorPickerSelectedColorChanged;
		}

		void ColorPickerSelectedColorChanged(object sender, Controls.ColorPicker.SelectedColorEventArgs e)
		{
			Color = ColorPicker.SelectedColor;
		}

		public static readonly Property<ColorEditor, string> NameOfColorProperty =
			Property<ColorEditor, string>.Register(c => c.NameOfColor);
		public string NameOfColor
		{
			get { return ColorName.Text; }
			set { ColorName.Text = value; }
		}

		public static readonly Property<ColorEditor, System.Windows.Media.Color> ColorProperty =
			Property<ColorEditor, System.Windows.Media.Color>.Register(c => c.Color);

		public System.Windows.Media.Color Color
		{
			get { return ColorProperty.GetValue(this); }
			set
			{
				ColorProperty.SetValue(this, value);
				//ColorPicker.SelectedColor = value;
				LayoutRoot.DataContext = value;

				var brush = ColorBorder.Background as SolidColorBrush;
				if (null != brush)
				{
					brush.Color = value;
				}
			}
		}

		private void Border_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			ColorPickerPopup.IsOpen = true;
		}

		private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			ColorPickerPopup.IsOpen = false;
		}

		private void ColorValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
		{
			var color = new System.Windows.Media.Color();
			color.R = (byte)RedNumeric.Value;
			color.G = (byte)GreenNumeric.Value;
			color.B = (byte)BlueNumeric.Value;
			color.A = 0xff;
			Color = color;
		}

	}
}
