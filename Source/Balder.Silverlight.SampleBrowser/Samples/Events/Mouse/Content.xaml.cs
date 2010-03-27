using Balder.Core;
using Balder.Core.Math;

namespace Balder.Silverlight.SampleBrowser.Samples.Events.Mouse
{
	public partial class Content
	{
		public Content()
		{
			InitializeComponent();

			Box1.MouseMove += Mesh_MouseMove;
			Box1.MouseEnter += Mesh_MouseEnter;
			Box1.MouseLeave += Mesh_MouseLeave;
			Box1.MouseLeftButtonUp += Mesh_MouseLeftButtonUp;
			Box1.MouseLeftButtonDown += Mesh_MouseLeftButtonDown;

			Box2.MouseMove += Mesh_MouseMove;
			Box2.MouseEnter += Mesh_MouseEnter;
			Box2.MouseLeave += Mesh_MouseLeave;
			Box2.MouseLeftButtonUp += Mesh_MouseLeftButtonUp;
			Box2.MouseLeftButtonDown += Mesh_MouseLeftButtonDown;

			Box3.MouseMove += Mesh_MouseMove;
			Box3.MouseEnter += Mesh_MouseEnter;
			Box3.MouseLeave += Mesh_MouseLeave;
			Box3.MouseLeftButtonUp += Mesh_MouseLeftButtonUp;
			Box3.MouseLeftButtonDown += Mesh_MouseLeftButtonDown;
		}

		private void Mesh_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
		{
			_mouseEnter.Text = "true";
			var node = sender as Node;
			if( null != node )
			{
				if( !string.IsNullOrEmpty(node.Name) )
				{
					_object.Text = node.Name;
				}
			}
		}

		private void Mesh_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
		{
			_mouseEnter.Text = "false";
			_object.Text = "None";
		}

		private void Mesh_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			_mouseLButtonUp.Text = "true";
			_mouseLButtonDown.Text = "false";
		}

		private void Mesh_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			_mouseLButtonUp.Text = "false";
			_mouseLButtonDown.Text = "true";
		}


		private string FormatCoordinate(Coordinate coordinate)
		{
			if( null == coordinate )
			{
				return "Not Set";
			}
			var xRounded = System.Math.Round(coordinate.X, 2);
			var yRounded = System.Math.Round(coordinate.Y, 2);
			var zRounded = System.Math.Round(coordinate.Z, 2);
			var text = string.Format("({0:0.##}, {1:0.##}, {2:0.##})", xRounded, yRounded, zRounded);
			return text;
		}


		private void LayoutRoot_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
		{
			var position = e.GetPosition(LayoutRoot);
			_xpos.Text = position.X.ToString();
			_ypos.Text = position.Y.ToString();

			_mousePickRayPosition.Text = FormatCoordinate(_game.Viewport.MousePickRayStart);
			_mousePickRayDirection.Text = FormatCoordinate(_game.Viewport.MousePickRayDirection);
		}

		private void Mesh_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
		{
			_mouseEnter.Text = "true";
			var node = sender as Node;
			if (null != node)
			{
				if (!string.IsNullOrEmpty(node.Name))
				{
					_object.Text = node.Name;
				}
			}
		}
	}
}
