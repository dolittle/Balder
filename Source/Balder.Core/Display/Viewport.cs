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

using Balder.Core.Debug;
using Balder.Core.Execution;
using Balder.Core.Math;
using Balder.Core.Rendering;
using Balder.Core.View;
#if(SILVERLIGHT)
using System.Windows;
using Ninject;

#endif

namespace Balder.Core.Display
{
	/// <summary>
	/// Represents a viewport within a display - the Viewport is a 2D rectangle representing a clipping region.
	/// The viewport also holds the view used to render and also holds the scene that contains the objects that
	/// will be rendered within the viewport
	/// </summary>
#if(SILVERLIGHT)
	public class Viewport : FrameworkElement
#else
	public class Viewport
#endif
	{
		private const float MinDepth = 0f;
		private const float MaxDepth = 1f;

		private Ray _mousePickRay;

#if(SILVERLIGHT)
		/// <summary>
		/// Creates a viewport
		/// </summary>
		public Viewport()
			: this(Runtime.Instance.Kernel.Get<IRuntimeContext>())
		{
		}
#endif

		/// <summary>
		/// Creates a viewport
		/// </summary>
		/// <param name="runtimeContext">RuntimeContext that the viewport belongs to</param>
		public Viewport(IRuntimeContext runtimeContext)
		{
			DebugInfo = new DebugInfo();

			_mousePickRay = new Ray(Vector.Zero, Vector.Forward);

			Messenger.DefaultContext.SubscriptionsFor<RenderMessage>().AddListener(this, Render);
			Messenger.DefaultContext.SubscriptionsFor<PrepareMessage>().AddListener(this, Prepare);
		}

		public void Uninitialize()
		{
			Messenger.DefaultContext.SubscriptionsFor<RenderMessage>().RemoveListener(this, Render);
			Messenger.DefaultContext.SubscriptionsFor<PrepareMessage>().RemoveListener(this, Prepare);
		}

		/// <summary>
		/// Get or set the x position in pixelsof the viewport within the display, where 0 is the left
		/// </summary>
		public int XPosition { get; set; }

		/// <summary>
		/// Get or set the y position in pixels of the viewport within the display, where 0 is the top.
		/// </summary>
		public int YPosition { get; set; }

		/// <summary>
		/// Get or set the width in pixels of the viewport within the display
		/// </summary>
#if(SILVERLIGHT)
        public new int Width { get; set; }
#else
		public int Width { get; set; }
#endif

		/// <summary>
		/// Get or set the height in pixels of the viewport within the display
		/// </summary>
#if(SILVERLIGHT)
        public new int Height { get; set; }
#else
		public int Height { get; set; }
#endif

		/// <summary>
		/// Get or set the scene to use during rendering
		/// </summary>
		public Scene Scene { get; set; }

		/// <summary>
		/// Get or set the view to be used during rendering
		/// </summary>
		public IView View { get; set; }

		/// <summary>
		/// Get or set the debug info for the Viewport
		/// </summary>
		public DebugInfo DebugInfo { get; set; }

		/// <summary>
		/// Get the display in which the viewport is rendered to
		/// </summary>
		public IDisplay Display { get; internal set; }

		/// <summary>
		/// Get the aspect ratio for the viewport
		/// </summary>
		public float AspectRatio { get { return ((float)Width) / ((float)Height); } }


		public static readonly Property<Viewport, Coordinate> MousePickRayStartProperty =
			Property<Viewport, Coordinate>.Register(v => v.MousePickRayStart);
		public Coordinate MousePickRayStart
		{
			get { return MousePickRayStartProperty.GetValue(this); }
			set { MousePickRayStartProperty.SetValue(this, value); }
		}

		public static readonly Property<Viewport, Coordinate> MousePickRayDirectionProperty =
			Property<Viewport, Coordinate>.Register(v => v.MousePickRayDirection);
		public Coordinate MousePickRayDirection
		{
			get { return MousePickRayDirectionProperty.GetValue(this); }
			set { MousePickRayDirectionProperty.SetValue(this, value); }
		}


		public static Vector Transform(Vector vector, Matrix matrix)
		{
			var vector2 = Vector.Zero;
			vector2.X = (((vector.X*matrix[0, 0]) + (vector.Y*matrix[1, 0])) + (vector.Z*matrix[2, 0]));
			vector2.Y = (((vector.X * matrix[0, 1]) + (vector.Y * matrix[1, 1])) + (vector.Z * matrix[2, 1]));
			vector2.Z = (((vector.X * matrix[0, 2]) + (vector.Y * matrix[1, 2])) + (vector.Z * matrix[2, 2]));
			return vector2;
		}


		/// <summary>
		/// Unproject a 2D coordinate into 3D. Basically convert a 2D point with depth
		/// information (Z) into a real 3D coordinate.
		/// </summary>
		/// <param name="source">Point to unproject</param>
		/// <param name="projection">Projection matrix</param>
		/// <param name="view">View matrix</param>
		/// <param name="world">World matrix</param>
		/// <returns>Unprojected 3D coordinate</returns>
		public Vector Unproject(Vector source, Matrix projection, Matrix view, Matrix world)
		{
			var combinedMatrix = (world*view)*projection; 
			var matrix = Matrix.Invert(combinedMatrix);

			source.X = (source.X - (Width / 2f)) / Width;
			source.Y = -((source.Y - (Height / 2f)) / Height);
			source.Z = (source.Z - MinDepth) / (MaxDepth - MinDepth);
			source.W = 1f;
			var vector = Vector.Transform(source, matrix);
			
			var a = (source.X * matrix[0, 3]) + 
						(source.Y * matrix[1, 3]) + 
						(source.Z * matrix[2, 3]) + 
						(matrix[3, 3]);

			if (!WithinEpsilon(a, 1f))
			{
				vector = (Vector)(vector / (a));
			}
			 
			return vector;
		}



		public void HandleMouseDebugInfo(int x, int y, Node hitNode)
		{
			if( DebugInfo.ShowMouseHitDetectionRay )
			{
				_mousePickRay = GetPickRay(x, y);
				MousePickRayStart = _mousePickRay.Position;
				MousePickRayDirection = _mousePickRay.Direction;
			} 
		}


		/// <summary>
		/// Get a node at a specified position relative to a specific viewport
		/// </summary>
		/// <param name="x">X position</param>
		/// <param name="y">Y position</param>
		/// <returns>A RenderableNode - null if it didn't find any node at the position</returns>
		public virtual RenderableNode GetNodeAtPosition(int x, int y)
		{
			var node = Display.GetNodeAtPosition(x, y);
			if( node is RenderableNode )
			{
				return node as RenderableNode;
			}
			return null;
		}

		public Ray GetPickRay(int x, int y)
		{
			var nearSource = new Vector((float) x, (float) y, 0);
			var farSource = new Vector((float) x, (float) y, 1);

			var world = Matrix.CreateTranslation(0,0,0);
			var nearPoint = Unproject(nearSource, View.ProjectionMatrix, View.ViewMatrix, world);
			var farPoint = Unproject(farSource, View.ProjectionMatrix, View.ViewMatrix, world);

			var direction = farPoint - nearPoint;
			direction.Normalize();

			return new Ray(nearPoint, direction);
		}


		public void Render(RenderMessage renderMessage)
		{
			if( null != View )
			{
				View.Update(this);	
			}
			Scene.Render(this);

			if( DebugInfo.ShowMouseHitDetectionRay )
			{
				DebugRenderer.Instance.RenderRay(_mousePickRay.Position,_mousePickRay.Direction,this);
			}
		}

		private void Prepare(PrepareMessage prepareMessage)
		{
			Scene.Prepare(this);
		}


		private static bool WithinEpsilon(float a, float b)
		{
			var num = a - b;
			return ((-1.401298E-45f <= num) && (num <= float.Epsilon));
		}
	}
}
