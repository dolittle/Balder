using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Balder.Core.Display;
using Balder.Core.Execution;
using Balder.Core.Math;
using Balder.Core.View;
using Matrix=Balder.Core.Math.Matrix;

namespace Balder.Silverlight.SampleBrowser
{
	public partial class MainPage
	{
		public MainPage()
		{
			InitializeComponent();

			/*
			var camera = new Camera();
			var viewport = new Viewport {Width = 640, Height = 480, View = camera};
			camera.Position = new Coordinate(0,0,-480);

			camera.Update(viewport);

			var world = Matrix.Identity;
			var matrix = (world*camera.ViewMatrix); // *camera.ProjectionMatrix;

			var vector = new Vector(-10, 0, 0);
			var result = Vector.Transform(vector, matrix);

			var translated = Vector.Translate(result, camera.ProjectionMatrix, viewport.Width, viewport.Height);
			*/

			/*
			var translated = result;
			translated.X = (translated.X * viewport.Width) + (viewport.Width / 2f);
			translated.Y = ((-translated.Y) * viewport.Height) + (viewport.Height / 2f);
			 * */
			//translated.Z = 0;

			/*
			var source = translated;
			source.X = (translated.X - (viewport.Width/2))/viewport.Width;
			source.Y = -((translated.Y - (viewport.Height/2))/viewport.Height);
			source.W = 1f;

			var unprojected = viewport.Unproject(translated, camera.ProjectionMatrix, camera.ViewMatrix, world);

			var ray = viewport.GetPickRay((int)translated.X, (int)translated.Y);
			*/

		}

		private void HandleGameInVisualTree(UIElement element, bool reload)
		{
			if( null == element )
			{
				return;
			}
			if( element is ItemsControl )
			{
				foreach( var item in ((ItemsControl)element).Items )
				{
					if( item is Game )
					{
						HandleGame((Game)item, reload);
						break;
					} else if( item is UIElement)
					{
						HandleGameInVisualTree((UIElement)item, reload);
					}
				}
			} else if( element is Panel )
			{
				foreach( var child in ((Panel)element).Children )
				{
					if( child is Game )
					{
						HandleGame((Game)child, reload);
						break;
					} else
					{
						HandleGameInVisualTree(child, reload);
					}
				}
			} else if( element is ContentControl )
			{
				var contentControl = element as ContentControl;
				if( contentControl.Content is Game )
				{
					HandleGame((Game)contentControl.Content, reload);
				} else if( contentControl.Content is UIElement )
				{
					HandleGameInVisualTree((UIElement)contentControl.Content, reload);
				}
			} else
			{
				var count = VisualTreeHelper.GetChildrenCount(element);
				if (count > 0)
				{
					var child = VisualTreeHelper.GetChild(element, 0);
					if (null != child && child is UIElement)
					{
						HandleGameInVisualTree(child as UIElement, reload);
					}
				}
			}
		}


		private void HandleGame(Game game, bool reload)
		{
			if( reload )
			{
			} else
			{
				game.Unload();	
			}
		}


		
		private bool _tabItemChanged = false;
		private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (null == TabControl)
			{
				return;
			}

			
			_tabItemChanged = true;
		}

		private void ContentFrame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
		{			
			_resourceView.Source = ContentFrame.Source;
		}

		private void ContentFrame_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
		{
			if (!_tabItemChanged)
			{
				HandleGameInVisualTree(ContentFrame, false);
			}
			
			_tabItemChanged = false;
		}
	}
}
