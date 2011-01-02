using System;
using System.Windows;
using System.Windows.Input;

namespace Balder.Silverlight.SampleBrowser.Samples.Creative.CoverFlow
{
	public partial class Content
	{
		public Content()
		{
			InitializeComponent();

			/*
			KeyDown += PageKeyUp;

			for (var i = 0; i < 10; i++  )
			{
				_flow.AddCover();
			}*/
				
		}
		/*
		void PageKeyUp(object sender, KeyEventArgs e)
		{
			
			if (e.Key == Key.Left)
			{
				_flow.MovePrevious();
			}
			if (e.Key == Key.Right)
			{
				_flow.MoveNext();
			}
			

			if (e.Key == Key.D1)
			{
				_flow.MoveTo(0);
			}
			if (e.Key == Key.D5)
			{
				_flow.MoveTo(4);
			}
			if (e.Key == Key.D0)
			{
				_flow.MoveTo(9);
			}
		}*/


		void NormalClick(object sender, RoutedEventArgs e)
		{
			VisualStateManager.GoToState(this, Normal.Name, true);
		}
		void LeftClick(object sender, RoutedEventArgs e)
		{
			VisualStateManager.GoToState(this, Left.Name, true);
		}
		void RightClick(object sender, RoutedEventArgs e)
		{
			VisualStateManager.GoToState(this, Right.Name, true);
		}
	}
}
