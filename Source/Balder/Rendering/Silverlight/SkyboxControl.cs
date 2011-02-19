#region License

//
// Author: Einar Ingebrigtsen <einar@dolittle.com>
// Copyright (c) 2007-2011, DoLittle Studios
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
#if(SILVERLIGHT)
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Balder.Display;
using Balder.Execution;
using Balder.Materials;
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

		private IMap _previousFrontMap;
		private IMap _previousBackMap;
		private IMap _previousLeftMap;
		private IMap _previousRightMap;
		private IMap _previousTopMap;
		private IMap _previousBottomMap;

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
		}

		private Image CreateImage()
		{
			var image = new Image { Stretch = Stretch.None };
			image.Projection = new Matrix3DProjection();
			Children.Add(image);
			return image;
		}

		private void PrepareSides(Skybox skybox)
		{
			PrepareSide(_frontImage, skybox.Front, ref _previousFrontMap	,0, 0, 0, 0, 1f, ref _frontWorld);
			PrepareSide(_backImage, skybox.Back, ref _previousBackMap		,0, 180, 0, 0, -1f, ref _backWorld);
			PrepareSide(_leftImage, skybox.Left, ref _previousLeftMap		,0, -90, -1f, 0, 0, ref _leftWorld);
			PrepareSide(_rightImage, skybox.Right, ref _previousRightMap	,0, 90, 1f, 0, 0, ref _rightWorld);
			PrepareSide(_topImage, skybox.Top, ref _previousTopMap			,-90, 0, 0, 1f, 0, ref _topWorld);
			PrepareSide(_bottomImage, skybox.Bottom, ref _previousBottomMap	,90, 0, 0, -1f, 0, ref _bottomWorld);
		}

		private void PrepareSide(
			Image image, 
			IMap map, 
			ref IMap previousMap, 
			float rotationX, 
			float rotationY, 
			float xPosition, 
			float yPosition, 
			float zPosition, 
			ref Matrix matrix)
		{
			if( null == previousMap || !map.Equals(previousMap))
			{
				var texture = _textureManager.GetTextureForMap(map).FullDetailLevel;
				image.Source = texture.WriteableBitmap;
				previousMap = map;
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
		}

		private void UpdateSide(Image image, Viewport viewport, Matrix viewMatrix, Matrix world)
		{
			var matrix = world * viewMatrix * viewport.View.ProjectionMatrix * viewport.ScreenMatrix;
			var m3d = matrix.ToMatrix3D();

			var projection = image.Projection as Matrix3DProjection;
			projection.ProjectionMatrix = m3d;
		}

		

		public void Render(Viewport viewport, Skybox skybox)
		{
			Visibility = Visibility.Visible;
			((RectangleGeometry)Clip).Rect = new Rect(0, 0, viewport.Width, viewport.Height);

			PrepareSides(skybox);

			var viewMatrix = viewport.View.ViewMatrix.Clone();
			viewMatrix.SetTranslation(0, 0, 0);

			UpdateSide(_frontImage, viewport, viewMatrix, _frontWorld);
			UpdateSide(_backImage, viewport, viewMatrix, _backWorld);
			UpdateSide(_leftImage, viewport, viewMatrix, _leftWorld);
			UpdateSide(_rightImage, viewport, viewMatrix, _rightWorld);
			UpdateSide(_topImage, viewport, viewMatrix, _topWorld);
			UpdateSide(_bottomImage, viewport, viewMatrix, _bottomWorld);
		}
	}
}
#endif