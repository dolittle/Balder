using System.Collections.Generic;
using Balder;
using Balder.Input;
using Balder.Math;
using Color = Balder.Color;

namespace Balder.Silverlight.SampleBrowser.Samples.View.Orthographic
{
	public partial class Content
	{
		private readonly Dictionary<object, Color> _originalNodeColors;

		public Content()
		{
			InitializeComponent();



			_originalNodeColors = new Dictionary<object, Color>();

			/*
			Box1.MouseMove += Mesh_MouseMove;
			Box1.MouseEnter += Mesh_MouseEnter;
			Box1.MouseLeave += Mesh_MouseLeave;
			Box1.MouseLeftButtonUp += Mesh_MouseLeftButtonUp;
			Box1.MouseLeftButtonDown += Mesh_MouseLeftButtonDown;
			 * */

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

			_originalNodeColors[Box1] = Box1.Color;
			_originalNodeColors[Box2] = Box2.Color;
			_originalNodeColors[Box3] = Box3.Color;
		}


		private int _mouseEnterCounter;

		private void Mesh_MouseEnter(object sender, MouseEventArgs e)
		{
			_mouseEnter.Text = "true";
			_mouseEnterCounter = 0;
			var node = sender as RenderableNode;
			if (null != node)
			{
				node.Color = Colors.White;
				if (!string.IsNullOrEmpty(node.Name))
				{
					_object.Text = node.Name;
				}
			}
		}

		private void Mesh_MouseLeave(object sender, MouseEventArgs e)
		{
			var node = sender as RenderableNode;
			if (null != node)
			{
				if (_originalNodeColors.ContainsKey(node))
				{
					node.Color = _originalNodeColors[node];
				}
			}

			_mouseMove.Text = "false";
			_mouseEnter.Text = "false";
			_object.Text = "None";
		}

		private void Mesh_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			_mouseLButtonUp.Text = "true";
			_mouseLButtonDown.Text = "false";
		}

		private void Mesh_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			_mouseLButtonUp.Text = "false";
			_mouseLButtonDown.Text = "true";
		}


		private string FormatCoordinate(Coordinate coordinate)
		{
			if (null == coordinate)
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
		}

		private void Mesh_MouseMove(object sender, MouseEventArgs e)
		{
			if (++_mouseEnterCounter > 2)
			{
				_mouseEnter.Text = "false";
			}
			_mouseMove.Text = "true";
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
