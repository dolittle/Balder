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
		private TextureMipMapLevel _frontTexture;
		private TextureMipMapLevel _backTexture;
		private TextureMipMapLevel _leftTexture;
		private TextureMipMapLevel _rightTexture;
		private TextureMipMapLevel _topTexture;
		private TextureMipMapLevel _bottomTexture;


		private Image _frontImage;
		private Image _backImage;
		private Image _leftImage;
		private Image _rightImage;
		private Image _topImage;
		private Image _bottomImage;

		private Matrix _frontWorld;
		private Matrix _backWorld;
		private Matrix _leftWorld;
		private Matrix _rightWorld;
		private Matrix _topWorld;
		private Matrix _bottomWorld;

		private Dimension _frontDimensions;
		private Dimension _backDimensions;
		private Dimension _leftDimensions;
		private Dimension _rightDimensions;
		private Dimension _topDimensions;
		private Dimension _bottomDimensions;

		public SkyboxControl()
			: this(Runtime.Instance.Kernel.Get<ITextureManager>())
		{

		}

		public SkyboxControl(ITextureManager textureManager)
		{
			_textureManager = textureManager;
			HorizontalAlignment = HorizontalAlignment.Stretch;
			VerticalAlignment = VerticalAlignment.Stretch;
			SetupImages();
			Clip = new RectangleGeometry();
		}

		private void SetupImages()
		{
			_frontImage = CreateImage();
			_backImage = CreateImage();
			_leftImage = CreateImage();
			_rightImage = CreateImage();
			_topImage = CreateImage();
			_bottomImage = CreateImage();

			_frontDimensions = new Dimension();
			_backDimensions = new Dimension();
			_leftDimensions = new Dimension();
			_rightDimensions = new Dimension();
			_topDimensions = new Dimension();
			_bottomDimensions = new Dimension();
		}

		private Image CreateImage()
		{
			var image = new Image { Stretch = Stretch.None };
			image.Projection = new Matrix3DProjection();
			Children.Add(image);
			return image;
		}

		private void PrepareTextures(Skybox skybox)
		{
			_frontTexture = _textureManager.GetTextureForMap(skybox.Front).FullDetailLevel;
			_backTexture = _textureManager.GetTextureForMap(skybox.Back).FullDetailLevel;
			_leftTexture = _textureManager.GetTextureForMap(skybox.Left).FullDetailLevel;
			_rightTexture = _textureManager.GetTextureForMap(skybox.Right).FullDetailLevel;
			_topTexture = _textureManager.GetTextureForMap(skybox.Top).FullDetailLevel;
			_bottomTexture = _textureManager.GetTextureForMap(skybox.Bottom).FullDetailLevel;
		}

		private void PrepareSides()
		{
			SetMatrixIfTextureIsNew(_frontTexture, _frontDimensions, 0, 0, 0, 0, 1f, ref _frontWorld);
			SetMatrixIfTextureIsNew(_backTexture, _backDimensions, 0, 180, 0, 0, -1f, ref _backWorld);
			SetMatrixIfTextureIsNew(_leftTexture, _leftDimensions, 0, -90, -1f, 0, 0, ref _leftWorld);
			SetMatrixIfTextureIsNew(_rightTexture, _rightDimensions, 0, 90, 1f, 0, 0, ref _rightWorld);
			SetMatrixIfTextureIsNew(_topTexture, _topDimensions, -90, 0, 0, 1f, 0, ref _topWorld);
			SetMatrixIfTextureIsNew(_bottomTexture, _bottomDimensions, 90, 0, 0, -1f, 0, ref _bottomWorld);
		}

		private void SetMatrixIfTextureIsNew(TextureMipMapLevel texture, Dimension existingDimension, float rotationX, float rotationY, float xPosition, float yPosition, float zPosition, ref Matrix matrix)
		{
			if (existingDimension.Equals(texture.Width, texture.Height))
			{
				return;
			}
			existingDimension.Set(texture.Width, texture.Height);

			var widthScale = 1f / (texture.Width * 0.5f);
			var heightScale = 1f / (texture.Height * 0.5f);

			var invertY = Matrix.CreateScale(new Vector(1f, -1f, 1f));
			var origin = Matrix.CreateTranslation(-(texture.Width >> 1), -(texture.Height >> 1), 0);
			var scale = Matrix.CreateScale(new Vector(widthScale, heightScale, 1f));
			var translate = Matrix.CreateTranslation(xPosition, yPosition, zPosition);
			var rotation = Matrix.CreateRotation(rotationX, rotationY, 0f);

			var world = origin * invertY * scale * rotation * translate;
			matrix = world;
		}


		private void UpdateImage(Image image, TextureMipMapLevel texture, Viewport viewport, Matrix viewMatrix, Matrix world)
		{
			image.Source = texture.WriteableBitmap;
			var matrix = world * viewMatrix * viewport.View.ProjectionMatrix * viewport.ScreenMatrix;
			var m3d = matrix.ToMatrix3D();

			var projection = image.Projection as Matrix3DProjection;
			projection.ProjectionMatrix = m3d;
		}

		private bool done;

		public void Render(Viewport viewport, Skybox skybox)
		{
			Visibility = Visibility.Visible;
			((RectangleGeometry)Clip).Rect = new Rect(0, 0, viewport.Width, viewport.Height);

			PrepareTextures(skybox);
			PrepareSides();

			var viewMatrix = viewport.View.ViewMatrix.Clone();
			viewMatrix.SetTranslation(0, 0, 0);

			UpdateImage(_frontImage, _frontTexture, viewport, viewMatrix, _frontWorld);
			UpdateImage(_backImage, _backTexture, viewport, viewMatrix, _backWorld);
			UpdateImage(_leftImage, _leftTexture, viewport, viewMatrix, _leftWorld);
			UpdateImage(_rightImage, _rightTexture, viewport, viewMatrix, _rightWorld);
			UpdateImage(_topImage, _topTexture, viewport, viewMatrix, _topWorld);
			UpdateImage(_bottomImage, _bottomTexture, viewport, viewMatrix, _bottomWorld);
		}
	}
}