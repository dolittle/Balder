using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.Policies;

namespace Balder.Design
{
	[UsesItemPolicy(typeof(GameSelectionPolicy))]
	public class GameAdornerProvider : AdornerProvider
	{
		private Rectangle _rectangle;
		private ModelItem _modelItem;


		protected override void Activate(ModelItem item)
		{
			_modelItem = item;
			if (null == _rectangle)
			{
				_rectangle = new Rectangle();
				_rectangle.HorizontalAlignment = HorizontalAlignment.Stretch;
				_rectangle.VerticalAlignment = VerticalAlignment.Stretch;
				_rectangle.Fill = new SolidColorBrush(Colors.Blue);
				_rectangle.Opacity = 0.01;
				_rectangle.MouseLeftButtonDown += _rectangle_MouseLeftButtonDown;
				Adorners.Add(_rectangle);
			}

			base.Activate(item);
		}

		protected override void Deactivate()
		{
			if( null != _rectangle )
			{
				_rectangle.MouseLeftButtonDown -= _rectangle_MouseLeftButtonDown;
				Adorners.Remove(_rectangle);
				_rectangle = null;
			}
			base.Deactivate();
		}


		void _rectangle_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			var item = _modelItem.Content.Collection[0];
			SelectionOperations.SelectOnly(Context, item);
		}


		public override bool IsToolSupported(Tool tool)
		{
			if( tool is SelectionTool )
			{
				return true;
			}

			return false;
		}
	}
}