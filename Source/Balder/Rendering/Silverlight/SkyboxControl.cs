#region License

//
// Author: Einar Ingebrigtsen <einar@dolittle.com>
// Copyright (c) 2007-2010, DoLittle Studios
//
// Licensed under the Microsoft Permissive License (Ms-PL), Version 1.1 (the "License")
// you may not use this file except in compliance with the License.
// You may obtain a copy of the license at 
//
//   http://balder.codeplex.com/license
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

#endregion

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Balder.Display;
using Balder.Execution;
using Balder.Math;
using Balder.Objects;
using Ninject;
using Matrix = Balder.Math.Matrix;
using Matrix3DProjection = System.Windows.Media.Matrix3DProjection;
using Stretch = System.Windows.Media.Stretch;

namespace Balder.Rendering.Silverlight
{
	public class SkyboxControl : Grid, ISkyboxContext
	{
		private ITextureManager _textureManager;

		public SkyboxControl()
			: this(Runtime.Instance.Kernel.Get<ITextureManager>())
		{

		}

		public SkyboxControl(ITextureManager textureManager)
		{
			_textureManager = textureManager;
			HorizontalAlignment = HorizontalAlignment.Stretch;
			VerticalAlignment = VerticalAlignment.Stretch;
			PrepareImages();

			Clip = new RectangleGeometry();
		}

		private void PrepareImages()
		{
			_frontImage = CreateImage();
			_backImage = CreateImage();
			_leftImage = CreateImage();
			_rightImage = CreateImage();
			_topImage = CreateImage();
			_bottomImage = CreateImage();
		}

		private Image CreateImage()
		{
			var image = new Image { Stretch = Stretch.None };
			image.Projection = new Matrix3DProjection();
			Children.Add(image);
			return image;
		}

		private Image _frontImage;
		private Image _backImage;
		private Image _leftImage;
		private Image _rightImage;
		private Image _topImage;
		private Image _bottomImage;



		private void UpdateImage(Image image, TextureMipMapLevel texture, Viewport viewport, Matrix viewMatrix, float rotationX, float rotationY, float xPos, float yPos, float zPos)
		{
			image.Source = texture.WriteableBitmap;
			var widthScale = 1f / (texture.Width * 0.5f);
			var heightScale = 1f / (texture.Height * 0.5f);

			var invertY = Matrix.CreateScale(new Vector(1f, -1f, 1f));
			var origin = Matrix.CreateTranslation(-(texture.Width >> 1), -(texture.Height >> 1), 0);
			var scale = Matrix.CreateScale(new Vector(widthScale, heightScale, 1f));
			var translate = Matrix.CreateTranslation(xPos, yPos, zPos);
			var rotation = Matrix.CreateRotation(rotationX, rotationY, 0f);

			var world = origin * invertY * scale * rotation * translate;
			//scale * rotation * translate;
			var screen = Matrix.CreateScreenTranslation(viewport.Width, viewport.Height);

			var matrix = world * viewMatrix * viewport.View.ProjectionMatrix * screen;
			var m3d = matrix.ToMatrix3D();

			var projection = image.Projection as Matrix3DProjection;
			projection.ProjectionMatrix = m3d;
		}


		public void Render(Viewport viewport, Skybox skybox)
		{
			Visibility = Visibility.Visible;

			((RectangleGeometry)Clip).Rect = new Rect(0, 0, viewport.Width, viewport.Height);

			var front = _textureManager.GetTextureForMap(skybox.Front).FullDetailLevel;
			var back = _textureManager.GetTextureForMap(skybox.Back).FullDetailLevel;
			var left = _textureManager.GetTextureForMap(skybox.Left).FullDetailLevel;
			var right = _textureManager.GetTextureForMap(skybox.Right).FullDetailLevel;
			var top = _textureManager.GetTextureForMap(skybox.Top).FullDetailLevel;
			var bottom = _textureManager.GetTextureForMap(skybox.Bottom).FullDetailLevel;

			var viewMatrix = viewport.View.ViewMatrix.Clone();
			viewMatrix.SetTranslation(0, 0, 0);


			UpdateImage(_frontImage, front, viewport, viewMatrix, 0, 0, 0, 0, 1f);
			UpdateImage(_backImage, back, viewport, viewMatrix, 0, 180, 0, 0, -1f);

			UpdateImage(_leftImage, left, viewport, viewMatrix, 0, -90, -1f, 0, 0);

			UpdateImage(_rightImage, right, viewport, viewMatrix, 0, 90, 1f, 0, 0);

			UpdateImage(_topImage, top, viewport, viewMatrix, -90, 0, 0, 1f, 0);
			UpdateImage(_bottomImage, bottom, viewport, viewMatrix, 90, 0, 0, -1f, 0);






		}
	}
}