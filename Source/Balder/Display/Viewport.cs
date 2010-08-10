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

using Balder.Debug;
using Balder.Execution;
using Balder.Math;
using Balder.Objects;
using Balder.Rendering;
using Balder.View;
#if(SILVERLIGHT)
using System.Windows;
using Ninject;

#endif

namespace Balder.Display
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
		public const float MinDepth = 0f;
		public const float MaxDepth = 1f;

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
		/// Get or set the Skybox for the display
		/// </summary>
		public Skybox Skybox { get; set; }


		/// <summary>
		/// Get the matrix that represents the matrix for converting to screen coordinates
		/// </summary>
		public Matrix ScreenMatrix { get; private set; }

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
			vector2.X = (((vector.X * matrix[0, 0]) + (vector.Y * matrix[1, 0])) + (vector.Z * matrix[2, 0]));
			vector2.Y = (((vector.X * matrix[0, 1]) + (vector.Y * matrix[1, 1])) + (vector.Z * matrix[2, 1]));
			vector2.Z = (((vector.X * matrix[0, 2]) + (vector.Y * matrix[1, 2])) + (vector.Z * matrix[2, 2]));
			return vector2;
		}


		/*
		public Vector3 Unproject(Vector3 source, Matrix projection, Matrix view, Matrix world)
		{
			Matrix matrix = Matrix.Invert(Matrix.Multiply(Matrix.Multiply(world, view), projection));
			source.X = (((source.X - this.X) / ((float)this.Width)) * 2f) - 1f;
			source.Y = -((((source.Y - this.Y) / ((float)this.Height)) * 2f) - 1f);
			source.Z = (source.Z - this.MinDepth) / (this.MaxDepth - this.MinDepth);
			Vector3 vector = Vector3.Transform(source, matrix);
			float a = (((source.X * matrix.M14) + (source.Y * matrix.M24)) + (source.Z * matrix.M34)) + matrix.M44;
			if (!WithinEpsilon(a, 1f))
			{
				vector = (Vector3)(vector / a);
			}
			return vector;
		}*/



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
			var combinedMatrix = (world * view) * projection;
			var matrix = Matrix.Invert(combinedMatrix);

			source.X = ((source.X / ((float)Width)) * 2f) - 1f;
			source.Y = -((source.Y / ((float)Height)) * 2f) - 1f;
			source.Z = (source.Z - MinDepth) / (MaxDepth - MinDepth);
			source.W = 1f;
			var vector = Vector.TransformNormal(source, matrix);

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
			if (DebugInfo.ShowMouseHitDetectionRay)
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
			var pickRay = GetPickRay(x, y);
			RenderableNode closestNode = null;
			var closestDistance = float.MaxValue;

			foreach ( var node in Scene.RenderableNodes)
			{
				GetNodeAtPosition(node, pickRay, ref closestNode, ref closestDistance);
			}

			return closestNode;
		}

		private void GetNodeAtPosition(INode node, Ray pickRay, ref RenderableNode closestNode, ref float closestDistance)
		{
			
			if (node is RenderableNode)
			{
				var pickNode = node as RenderableNode;

				var distance = pickNode.Intersects(pickRay);
				if (null != distance && distance.Value < closestDistance)
				{
					closestDistance = distance.Value;
					closestNode = pickNode;
				}
			}

			if( node is IHaveChildren )
			{
				var childrenNode = node as IHaveChildren;
				
				foreach( var child in childrenNode.Children )
				{
					GetNodeAtPosition(child, pickRay, ref closestNode, ref closestDistance);
				}
			}
		}

		public Ray GetPickRay(int x, int y)
		{
			var view = View.ViewMatrix;
			var world = Matrix.Identity;
			var projection = View.ProjectionMatrix;

			var v = new Vector
						{
							X = (((2.0f * x) / Width) - 1) / (projection[0, 0]),
							Y = -(((2.0f * y) / Height) - 1) / (projection[1, 1]),
							Z = 1f
						};


			var inverseView = Matrix.Invert(view);

			var ray = new Ray
						{
							Direction = new Vector
											{
												X = (v.X * inverseView[0, 0]) + (v.Y * inverseView[1, 0]) + (v.Z * inverseView[2, 0]),
												Y = (v.X * inverseView[0, 1]) + (v.Y * inverseView[1, 1]) + (v.Z * inverseView[2, 1]),
												Z = (v.X * inverseView[0, 2]) + (v.Y * inverseView[1, 2]) + (v.Z * inverseView[2, 2]),
												W = 1f
											}
						};
			ray.Direction.Normalize();

			ray.Position = new Vector
							{
								X = inverseView[3, 0],
								Y = inverseView[3, 1],
								Z = inverseView[3, 2],
								W = 1f
							};

			ray.Position += (ray.Direction * View.Near);

			return ray;
		}


		public void Render(RenderMessage renderMessage)
		{
			ScreenMatrix = Matrix.CreateScreenTranslation(Width, Height);
			if (null != View)
			{
				View.Update(this);

				if (null != Skybox && Skybox.IsEnabled)
				{
					Skybox.SkyboxContext.Render(this, Skybox);
				}
			}

			Scene.Render(this);

			if (DebugInfo.ShowMouseHitDetectionRay)
			{
				DebugRenderer.Instance.RenderRay(_mousePickRay.Position, _mousePickRay.Direction, this);
			}

			Messenger.DefaultContext.Send(RenderDoneMessage.Default);
		}

		/// <summary>
		/// Projects a vector into screen-coordinates
		/// </summary>
		/// <param name="source">Source vector to project</param>
		/// <param name="world">World matrix for the vector</param>
		/// <returns>Projected vector</returns>
		/// <remarks>
		/// Contribution from Dmitriy Kataskin
		/// </remarks>
		public Vector Project(Vector source, Matrix world)
		{
			var matrix = (world * View.ViewMatrix) * View.ProjectionMatrix;
			return ProjectWithMatrix(source, matrix);
		}

		public Vector ProjectWithMatrix(Vector source, Matrix matrix)
		{
			return ProjectWithMatrix(source.X, source.Y, source.Z, matrix);
		}

		public Vector ProjectWithMatrix(float x, float y, float z, Matrix matrix)
		{
			var vector = Vector.TransformNormal(x,y,z, matrix);
			var a = (((x * matrix[0, 3]) + (y * matrix[1, 3])) + (z * matrix[2, 3])) + matrix[3, 3];
			
			if (!WithinEpsilon(a, 1f))
			{
				vector = vector / (a);
			}

			vector.X = (((vector.X + 1f) * 0.5f) * Width);
			vector.Y = (((-vector.Y + 1f) * 0.5f) * Height);
			vector.Z = (vector.Z * (MaxDepth - MinDepth)) + MinDepth;
			return vector;
		}

		private void Prepare(PrepareMessage prepareMessage)
		{
			Scene.Prepare(this);
		}


		public static bool WithinEpsilon(float a, float b)
		{
			var num = a - b;
			return ((-1.401298E-45f <= num) && (num <= float.Epsilon));
		}
	}
}
