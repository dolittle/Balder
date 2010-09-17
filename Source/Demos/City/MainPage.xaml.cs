using System.Windows.Input;
using Balder.Math;

namespace City
{
	public partial class MainPage
	{
		public MainPage()
		{
			InitializeComponent();

			Loaded += MainPage_Loaded;

			
		}

		void MainPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			var dot = Vector.Dot(Camera.Position, Camera.Target);


			_forward = Vector.Forward;

			_speed = new Vector(0,0,5f);

			_rotation = 120;
		}

		private void LayoutRoot_KeyUp(object sender, KeyEventArgs e)
		{
			

		}

		private const float RotationSpeed = 2f;
		private Vector _direction;

		private float _rotation;
		private Vector _forward;
		private Vector _speed;



		private void LayoutRoot_KeyDown(object sender, KeyEventArgs e)
		{

			
			if( e.Key == Key.Left )
			{
				_rotation -= RotationSpeed;
			} 
			if( e.Key == Key.Right )
			{
				_rotation += RotationSpeed;
			}

			var position = (Vector) Camera.Position;
			var rotationMatrix = Matrix.CreateRotationY(_rotation);

			var rotatedForward = _forward*rotationMatrix;

			var rotatedSpeed = _speed*rotationMatrix;

			if( e.Key == Key.Up )
			{
				position += rotatedSpeed;
			}

			if( e.Key == Key.Down )
			{
				position -= rotatedSpeed;
			}


			Camera.Position = position;
			Camera.Target = position + rotatedForward;
		}

		private void City_MouseMove(object sender, Balder.Input.MouseEventArgs args)
		{
			MouseOverNodeTextBlock.Text = args.OriginalSource.Name;

		}
	}
}
