using System.Collections;
using Balder.Execution;
using Balder.Silverlight.Helpers;

namespace Balder.Silverlight.SampleBrowser.Samples.Materials.Editor
{
	public partial class MapEditor
	{
		public MapEditor()
		{
			InitializeComponent();

			MapItemsComboBox.SelectionChanged += MapItemsComboBoxSelectionChanged;
			OpacityNumericUpDown.ValueChanged += OpacityNumericUpDownValueChanged;
		}

		private void OpacityNumericUpDownValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
		{
			Opacity = OpacityNumericUpDown.Value;
		}


		private void  MapItemsComboBoxSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			SelectedMapItem = MapItemsComboBox.SelectedItem;
		}



		public static readonly DependencyProperty<MapEditor, string> MapNameProperty =
			DependencyProperty<MapEditor, string>.Register(m => m.MapName);

		public string MapName
		{
			get { return MapNameTextBlock.Text; }
			set { MapNameTextBlock.Text = value; }
		}


		public static readonly DependencyProperty<MapEditor, double> OpacityProperty =
			DependencyProperty<MapEditor, double>.Register(m => m.Opacity);
		public double Opacity
		{
			get { return OpacityProperty.GetValue(this);  }
			set
			{
				OpacityProperty.SetValue(this, value);
				OpacityNumericUpDown.Value = value;
			}
		}


		public static readonly DependencyProperty<MapEditor, IEnumerable> MapItemsProperty =
			DependencyProperty<MapEditor, IEnumerable>.Register(m => m.MapItems);
		public IEnumerable MapItems
		{
			get { return MapItemsComboBox.ItemsSource;  }
			set { MapItemsComboBox.ItemsSource = value; }
		}

		public static readonly DependencyProperty<MapEditor, string> MapItemDisplayMemberPathProperty =
			DependencyProperty<MapEditor, string>.Register(m => m.MapItemDisplayMemberPath);

		public string MapItemDisplayMemberPath
		{
			get { return MapItemsComboBox.DisplayMemberPath; }
			set { MapItemsComboBox.DisplayMemberPath = value; }
		}

		public static readonly DependencyProperty<MapEditor, object> SelectedMapItemProperty =
			DependencyProperty<MapEditor, object>.Register(m => m.SelectedMapItem);

		public object SelectedMapItem
		{
			get { return SelectedMapItemProperty.GetValue(this); }
			set
			{
				SelectedMapItemProperty.SetValue(this, value);
				MapItemsComboBox.SelectedValue = value;
			}
		}
	}
}
