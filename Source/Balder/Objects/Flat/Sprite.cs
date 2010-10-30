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

using System;
using System.Collections.Generic;
using System.Linq;
using Balder.Assets;
using Balder.Content;
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


		private Image[] _frames;

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

		public override void Render(Viewport viewport, DetailLevel detailLevel)
		{
			/* From DirectX sample
				w = width passed to D3DXMatrixPerspectiveLH
				h = height passed to D3DXMatrixPerspectiveLH
				n = z near passed to D3DXMatrixPerspectiveLH
				f = z far passed to D3DXMatrixPerspectiveLH
				d = distance of sprite from camera along Z
				qw = width of sprite quad
				qh = height of sprite quad
				vw = viewport height
				vh = viewport width
				scale = n / d
				(i.e. near/distance, such that at d = n, scale = 1.0)
				renderedWidth = vw * qw * scale / w 
				renderedHeight = vh * qh * scale / h
			 */

			var world = RenderingWorld;
			var view = viewport.View.ViewMatrix;
			var projection = viewport.View.ProjectionMatrix;

			//var position = new Vector(0, 0, 0);
			var actualPosition = new Vector(world.M41, world.M42, world.M43);
			//var transformedPosition = Vector.Transform(position, world, view);
			//var translatedPosition = Vector.Translate(transformedPosition, projection, viewport.Width, viewport.Height);

			var scale = GetScale(viewport, actualPosition);

			var xscale = scale;
			var yscale = scale;

			_spriteContext.Render(viewport, this, view, projection, world, xscale, yscale, 0f);
		}

		private float GetScale(Viewport viewport, Vector actualPosition)
		{
			var distanceVector = viewport.View.Position - actualPosition;
			var distance = distanceVector.Length;
			var n = 100.0f;
			distance = MathHelper.Abs(distance);

			var scale = 0.0f + (n / distance);
			if (scale <= 0)
			{
				scale = 0;
			}
			return scale;
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
			/*
			var world = RenderingWorld;
			var view = viewport.View.ViewMatrix;
			var projection = viewport.View.ProjectionMatrix;

			var worldViewProjection = world * view * projection;

			
			var actualPosition = new Vector(world.M41, world.M42, world.M43);

			var scale = GetScale(viewport, actualPosition);
			var screenPosition = viewport.Project(actualPosition, world);

			var halfX = ((float) (CurrentFrame.Width >> 1)*scale);
			var halfY = ((float) (CurrentFrame.Height >> 1)*scale);
			var topX = screenPosition.X - halfX;
			var topY = screenPosition.Y - halfY;
			var bottomX = screenPosition.X + halfX;
			var bottomY = screenPosition.Y + halfY;

			var pixels = CurrentFrame.ImageContext.GetPixelsAs32BppARGB();

			*/


			return base.Intersects(viewport, pickRay);
		}

		public override void PrepareBoundingSphere()
		{
			if( null != CurrentFrame )
			{
				var size = (float)System.Math.Max(CurrentFrame.Width, CurrentFrame.Height);
				var actualSize = size/10f;
				BoundingSphere = new BoundingSphere(Vector.Zero, actualSize);	
			}

			
			base.PrepareBoundingSphere();
		}
		
		


	}
}