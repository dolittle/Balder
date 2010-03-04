using Balder.Core;

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

		private void LayoutRoot_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
		{
			var position = e.GetPosition(LayoutRoot);
			_xpos.Text = position.X.ToString();
			_ypos.Text = position.Y.ToString();
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
