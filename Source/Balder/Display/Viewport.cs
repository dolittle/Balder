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
using Balder.Debug;
using Balder.Execution;
using Balder.Input;
using Balder.Math;
using Balder.Objects;
using Balder.Rendering;
using Balder.View;
#if(XAML)
using System.Windows;
using Ninject;
#endif
using Vector = Balder.Math.Vector;

namespace Balder.Display
{
	/// <summary>
	/// Represents a viewport within a display - the Viewport is a 2D rectangle representing a clipping region.
	/// The viewport also holds the view used to render and also holds the scene that contains the objects that
	/// will be rendered within the viewport
	/// </summary>
#if(XAML)
	public class Viewport : FrameworkElement
#else
	public class Viewport
#endif
	{
		private readonly IRuntimeContext _runtimeContext;
		public const float MinDepth = 0f;
		public const float MaxDepth = 1f;

		private Ray _mousePickRay;

#if(XAML)
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
			_runtimeContext = runtimeContext;
			DebugInfo = new DebugInfo();
			Statistics = new ViewportStatistics();

			_mousePickRay = new Ray(Vector.Zero, Vector.Forward);


			runtimeContext.MessengerContext.SubscriptionsFor<PassiveRenderingSignal>().AddListener(this, PassiveRender);

			runtimeContext.MessengerContext.SubscriptionsFor<RenderMessage>().AddListener(this, Render);
			runtimeContext.MessengerContext.SubscriptionsFor<PrepareMessage>().AddListener(this, Prepare);
		}

		public void Uninitialize()
		{
			_runtimeContext.MessengerContext.SubscriptionsFor<PassiveRenderingSignal>().RemoveListener(this, PassiveRender);
			_runtimeContext.MessengerContext.SubscriptionsFor<RenderMessage>().RemoveListener(this, Render);
			_runtimeContext.MessengerContext.SubscriptionsFor<PrepareMessage>().RemoveListener(this, Prepare);
		}

		/// <summary>
		/// Gets or sets the x position in pixelsof the viewport within the display, where 0 is the left
		/// </summary>
		public int XPosition { get; set; }

		/// <summary>
		/// Gets or sets the y position in pixels of the viewport within the display, where 0 is the top.
		/// </summary>
		public int YPosition { get; set; }

		/// <summary>
		/// Gets or sets the width in pixels of the viewport within the display
		/// </summary>
#if(XAML)
		public new int Width { get; set; }
#else
		public int Width { get; set; }
#endif

		/// <summary>
		/// Gets or sets the height in pixels of the viewport within the display
		/// </summary>
#if(XAML)
		public new int Height { get; set; }
#else
		public int Height { get; set; }
#endif

		/// <summary>
		/// Gets or sets the scene to use during rendering
		/// </summary>
		public Scene Scene { get; set; }

		/// <summary>
		/// Gets or sets the view to be used during rendering
		/// </summary>
		public IView View { get; set; }

		/// <summary>
		/// Gets or sets the debug info for the Viewport
		/// </summary>
		public DebugInfo DebugInfo { get; set; }

		/// <summary>
		/// Get the display in which the viewport is rendered to
		/// </summary>
		public IDisplay Display { get; internal set; }

		/// <summary>
		/// Gets or sets the Skybox for the display
		/// </summary>
		public Skybox Skybox { get; set; }


		/// <summary>
		/// Get the matrix that represents the matrix for converting to screen coordinates
		/// </summary>
		public Matrix ScreenMatrix { get; private set; }

		/// <summary>
		/// Get the matrix representing the combined View * Projection matrices
		/// </summary>
		public Matrix ViewProjectionMatrix { get; private set; }

		/// <summary>
		/// Get the matrix representing the combined View * Projection * Screen matrices
		/// </summary>
		public Matrix ViewProjectionScreenMatrix { get; private set; }

		/// <summary>
		/// Get the aspect ratio for the viewport
		/// </summary>
		public float AspectRatio { get { return ((float)Width) / ((float)Height); } }

		/// <summary>
		/// Get the statistics for the viewport
		/// </summary>
		public ViewportStatistics Statistics { get; private set; }


		/// <summary>
		/// Identifies the MousePickRayStart property
		/// </summary>
		public static readonly Property<Viewport, Coordinate> MousePickRayStartProperty =
			Property<Viewport, Coordinate>.Register(v => v.MousePickRayStart);

		/// <summary>
		/// Gets or sets the ray start for a mouse
		/// </summary>
		public Coordinate MousePickRayStart
		{
			get { return MousePickRayStartProperty.GetValue(this); }
			set { MousePickRayStartProperty.SetValue(this, value); }
		}

		/// <summary>
		/// Identifies the MousePickRayDirection property
		/// </summary>
		public static readonly Property<Viewport, Coordinate> MousePickRayDirectionProperty =
			Property<Viewport, Coordinate>.Register(v => v.MousePickRayDirection);

		/// <summary>
		/// Gets or sets the ray direction for a mouse
		/// </summary>
		public Coordinate MousePickRayDirection
		{
			get { return MousePickRayDirectionProperty.GetValue(this); }
			set { MousePickRayDirectionProperty.SetValue(this, value); }
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

			foreach (var node in Scene.RenderableNodes)
			{
				GetNodeAtPosition(node, pickRay, ref closestNode, ref closestDistance);
			}

			return closestNode;
		}

		private void GetNodeAtPosition(INode node, Ray pickRay, ref RenderableNode closestNode, ref float closestDistance)
		{
			if (!node.IsIntersectionTestEnabled || !IsNodeVisible(node))
			{
				return;
			}
			if (node is ICanGetNodeAtPosition)
			{
				closestDistance = GetClosestDistanceFromNode(node, pickRay, closestDistance, ref closestNode);
			}
			else
			{
				if (node is RenderableNode)
				{
					var pickNode = node as RenderableNode;

					var distance = pickNode.Intersects(this, pickRay);
					if (null != distance && distance.Value < closestDistance)
					{
						closestDistance = distance.Value;
						closestNode = pickNode;
					}
				}

				if (node is IHaveChildren)
				{
					closestDistance = GetClosestDistanceFromChildren(node, pickRay, closestDistance, ref closestNode);
				}
			}
		}


		private float GetClosestDistanceFromChildren(INode node, Ray pickRay, float closestDistance, ref RenderableNode closestNode)
		{
			var childrenNode = node as IHaveChildren;

			foreach (var child in childrenNode.Children)
			{
				GetNodeAtPosition(child, pickRay, ref closestNode, ref closestDistance);
			}
			return closestDistance;
		}

		private float GetClosestDistanceFromNode(INode node, Ray pickRay, float closestDistance, ref RenderableNode closestNode)
		{
			float? distance = null;
			RenderableNode nodeAtPosition = null;
			((ICanGetNodeAtPosition)node).GetNodeAtPosition(this, pickRay, ref nodeAtPosition, ref distance);
			if (null != distance && distance.Value < closestDistance)
			{
				closestNode = nodeAtPosition;
				closestDistance = distance.Value;
			}
			return closestDistance;
		}

		private bool IsNodeVisible(INode node)
		{
			if (node is ICanBeVisible)
			{
				return ((ICanBeVisible)node).IsVisible;
			}
			return true;
		}

		public Ray GetPickRay(int x, int y)
		{
			return View.GetPickRay(x, y);
		}

		private bool _render;

		private void PassiveRender(PassiveRenderingSignal message)
		{
			_render = true;

		}

		public void Render(RenderMessage renderMessage)
		{
			ScreenMatrix = Matrix.CreateScreenTranslation(Width, Height);

			ViewProjectionScreenMatrix = View.ViewMatrix * View.ProjectionMatrix * ScreenMatrix;
			if (null != View)
			{
				View.Update(this);

				if (null != Skybox &&
					Skybox.IsEnabled &&
					(_render || !_runtimeContext.PassiveRendering))
				{
					Skybox.SkyboxContext.Render(this, Skybox);
				}
			}

			Scene.Render(this);

			if (DebugInfo.ShowMouseHitDetectionRay)
			{
				DebugRenderer.Instance.RenderRay(_mousePickRay.Position, _mousePickRay.Direction, this);
			}

			_runtimeContext.MessengerContext.Send(RenderDoneMessage.Default);
			_render = false;
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
			return View.Unproject(source, projection, view, world);
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
			var vector = Vector.TransformNormal(x, y, z, matrix);
			var a = (((x * matrix.M14) + (y * matrix.M24)) + (z * matrix.M34)) + matrix.M44;

			if (!MathHelper.WithinEpsilon(a, 1f))
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


	}
}
