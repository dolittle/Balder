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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Balder.Assets;
using Balder.Content;
using Balder.Debug;
using Balder.Display;
using Balder.Execution;
using Balder.Imaging;
using Balder.Math;
using Balder.Rendering;
#if(SILVERLIGHT)
using Balder.Silverlight.Helpers;
#endif

#if(DEFAULT_CONSTRUCTOR)
using Ninject;
#endif

namespace Balder.Objects.Flat
{
	public class Sprite : RenderableNode, IAsset
	{
		private readonly IContentManager _contentManager;
		private readonly ISpriteContext _spriteContext;
		private Vector _upperLeft;
		private Vector _upperRight;
		private Vector _lowerLeft;
		private Vector _lowerRight;

		private Image[] _frames;

#if(DEFAULT_CONSTRUCTOR)
		public Sprite()
			: this(Runtime.Instance.Kernel.Get<IContentManager>(),
			Runtime.Instance.Kernel.Get<ISpriteContext>())
		{
		}
#endif

		public Sprite(IContentManager contentManager, ISpriteContext spriteContext)
		{
			_contentManager = contentManager;
			_spriteContext = spriteContext;
		}

		public Image CurrentFrame
		{
			get
			{
				if (null == _frames)
				{
					return null;
				}
				return _frames[0];
			}
		}



		public override void BeforeRendering(Viewport viewport, Matrix view, Matrix projection, Matrix world)
		{
			var size = (float)System.Math.Max(CurrentFrame.Width, CurrentFrame.Height);
			var actualSize = size / 10f;

			var inverseView = Matrix.Invert(view);
			_upperLeft = Vector.TransformNormal(new Vector(-actualSize, actualSize, 0), inverseView);
			_upperRight = Vector.TransformNormal(new Vector(actualSize, actualSize, 0), inverseView);
			_lowerLeft = Vector.TransformNormal(new Vector(-actualSize, -actualSize, 0), inverseView);
			_lowerRight = Vector.TransformNormal(new Vector(actualSize, -actualSize, 0), inverseView);
			base.BeforeRendering(viewport, view, projection, world);
		}


		public override void Render(Viewport viewport, DetailLevel detailLevel)
		{
			var world = RenderingWorld;
			var view = viewport.View.ViewMatrix;
			var projection = viewport.View.ProjectionMatrix;

			var upperLeft = viewport.Project(_upperLeft, world);
			var upperRight = viewport.Project(_upperRight, world);
			var lowerLeft = viewport.Project(_lowerLeft, world);

			var xscale = (upperRight.X - upperLeft.X) / ((float)CurrentFrame.Width);
			var yscale = (lowerLeft.Y - upperLeft.Y) / ((float)CurrentFrame.Height);

			_spriteContext.Render(viewport, this, view, projection, world, xscale, yscale, 0f);
		}


#if(SILVERLIGHT)
		public static DependencyProperty<Sprite, Uri> AssetNameProperty =
			DependencyProperty<Sprite, Uri>.Register(o => o.AssetName);
		public Uri AssetName
		{
			get { return AssetNameProperty.GetValue(this); }
			set { AssetNameProperty.SetValue(this, value); }
		}

		public override void Prepare(Viewport viewport)
		{
			if (null != AssetName)
			{
				_contentManager.LoadInto(this, AssetName.OriginalString);
			}
			base.Prepare(viewport);
		}
#else
		public Uri AssetName { get; set; }
#endif

		public IAssetPart[] GetAssetParts()
		{
			return _frames;
		}

		public void SetAssetParts(IEnumerable<IAssetPart> assetParts)
		{
			var query = from a in assetParts
						where a is Image
						select a as Image;
			_frames = query.ToArray();
		}



		public override float? Intersects(Viewport viewport, Ray pickRay)
		{

			var world = RenderingWorld.Clone();
			world.M11 = 1f;
			world.M12 = 0f;
			world.M13 = 0f;

			world.M21 = 0f;
			world.M22 = 1f;
			world.M23 = 0f;

			world.M31 = 0f;
			world.M32 = 0f;
			world.M33 = 1f;

			var upperLeft = Vector.Transform(_upperLeft, world);
			var upperRight = Vector.Transform(_upperRight, world);
			var lowerLeft = Vector.Transform(_lowerLeft, world);
			var lowerRight = Vector.Transform(_lowerRight, world);

			var u = 0f;
			var v = 0f;

			var lowerTriangleU = 0f;
			var lowerTriangleV = 0f;

			if( upperLeft.X > upperRight.X )
			{
				var tmp = upperLeft;
				upperLeft = upperRight;
				upperRight = tmp;
			}

			if (lowerLeft.X > lowerRight.X)
			{
				var tmp = lowerLeft;
				lowerLeft = lowerRight;
				lowerRight = tmp;
			}

			var distance = pickRay.IntersectsTriangle(upperLeft, upperRight, lowerLeft, out u, out v);
			var lowerTriangleDistance = pickRay.IntersectsTriangle(upperRight, lowerLeft, lowerRight, out lowerTriangleU, out lowerTriangleV);

			if (null != distance || null != lowerTriangleDistance)
			{
				MathHelper.InterpolateBarycentric(
					0,0,
					1,0,
					0,1,
					u,v,
					out u, out v);
				
				if (null == distance)
				{
					u = lowerTriangleU;
					v = lowerTriangleV;
					MathHelper.InterpolateBarycentric(
						1, 0, 
						0, 1, 
						1, 1, 
						u, v, 
						out u, out v);
					distance = lowerTriangleDistance;
				}
				if (IsWithinFrame(u, v))
				{
					return distance;
				}
			}

			return null;
		}

		private bool IsWithinFrame(float u, float v)
		{
			var pixels = CurrentFrame.ImageContext.GetPixelsAs32BppARGB();

			var x = (int)(CurrentFrame.Width * u);
			var y = (int)(CurrentFrame.Height * v);

			var offset = (CurrentFrame.Width * y) + x;
			var pixel = pixels[offset];

			var color = Color.FromInt(pixel);
			if( color.Alpha != 0 )
			{
				return true;
			}
			return false;
		}


		public override void RenderDebugInfo(Viewport viewport, DetailLevel detailLevel)
		{
			if (viewport.DebugInfo.BoundingRectangles)
			{
				DebugRenderer.Instance.RenderRectangle(
					_upperLeft,
					_upperRight,
					_lowerLeft,
					_lowerRight,
					viewport,
					RenderingWorld
					);
			}

			base.RenderDebugInfo(viewport, detailLevel);
		}
	}
}