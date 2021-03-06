﻿#region License
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
using System.ComponentModel;

using Balder.Display;
using Balder.Execution;

using Balder.Input;
using Balder.Math;
using Balder.Rendering;
#if(DEFAULT_CONSTRUCTOR)
using Ninject;
#endif
using Matrix = Balder.Math.Matrix;

#if(XAML)
using System.Windows.Controls;
using MouseButtonEventHandler = Balder.Input.MouseButtonEventHandler;
using MouseEventHandler = Balder.Input.MouseEventHandler;
using ManipulationDeltaEventArgs = Balder.Input.ManipulationDeltaEventArgs;
using System.Windows;
using System.Windows.Input;
using Balder.Silverlight.Helpers;
using Balder.Silverlight.TypeConverters;
using Balder.Extensions.Silverlight;
using System.Windows.Media;
#endif

namespace Balder
{
	/// <summary>
	/// Abstract class representing a node in a scene
	/// </summary>
#if(XAML)
	public abstract class Node : ItemsControl, INotifyPropertyChanged,
#else
	public abstract partial class Node :
#endif
 INode, ICanBeCloned, ICanPrepare, IHaveRuntimeContext, IHaveLabel, IHavePropertyContainer
	{
#if(XAML)
		public event PropertyChangedEventHandler PropertyChanged;
#endif
		private static readonly EventArgs DefaultEventArgs = new EventArgs();

#if(XAML)
		internal static readonly BubbledEvent<Node, MouseEventHandler> MouseMoveEvent =
			BubbledEvent<Node, MouseEventHandler>.Register(n => n.MouseMove);
		internal static readonly BubbledEvent<Node, MouseEventHandler> MouseEnterEvent =
			BubbledEvent<Node, MouseEventHandler>.Register(n => n.MouseEnter);
		internal static readonly BubbledEvent<Node, MouseEventHandler> MouseLeaveEvent =
			BubbledEvent<Node, MouseEventHandler>.Register(n => n.MouseLeave);
		internal static new readonly BubbledEvent<Node, MouseButtonEventHandler> MouseLeftButtonDownEvent =
			BubbledEvent<Node, MouseButtonEventHandler>.Register(n => n.MouseLeftButtonDown);
		internal static new readonly BubbledEvent<Node, MouseButtonEventHandler> MouseLeftButtonUpEvent =
			BubbledEvent<Node, MouseButtonEventHandler>.Register(n => n.MouseLeftButtonUp);

		public new event MouseEventHandler MouseMove;
		public new event MouseEventHandler MouseEnter;
		public new event MouseEventHandler MouseLeave;
		public new event MouseButtonEventHandler MouseLeftButtonDown;
		public new event MouseButtonEventHandler MouseLeftButtonUp;
#endif

		public static readonly BubbledEvent<Node, BubbledEventHandler> PreparedEvent =
			BubbledEvent<Node, BubbledEventHandler>.Register(n => n.Prepared);

		public static readonly BubbledEvent<Node, BubbledEventHandler> ContentPreparedEvent =
			BubbledEvent<Node, BubbledEventHandler>.Register(n => n.ContentPrepared);

		public static readonly new BubbledEvent<Node, ManipulationDeltaEventHandler> ManipulationStartedEvent =
			BubbledEvent<Node, ManipulationDeltaEventHandler>.Register(n => n.ManipulationStarted);

		public static readonly BubbledEvent<Node, BubbledEventHandler> ManipulationStoppedEvent =
			BubbledEvent<Node, BubbledEventHandler>.Register(n => n.ManipulationStopped);

		public static readonly new BubbledEvent<Node, ManipulationDeltaEventHandler> ManipulationDeltaEvent =
			BubbledEvent<Node, ManipulationDeltaEventHandler>.Register(n => n.ManipulationDelta);

		public event BubbledEventHandler Prepared = (s, e) => { };
		public event BubbledEventHandler ContentPrepared = (s, e) => { };
		public new event ManipulationDeltaEventHandler ManipulationStarted = (s, e) => { };
		public event BubbledEventHandler ManipulationStopped = (s, e) => { };
		public new event ManipulationDeltaEventHandler ManipulationDelta = (s, e) => { };

		public event EventHandler Hover = (s, e) => { };
		public event EventHandler Click = (s, e) => { };

		private bool _isPrepared = false;
		private bool _isInitialized = false;
		private bool _isWorldInvalidated = false;
		private bool _isForcePrepareMatrices = true;



		protected Node()
		{
			InitializeTransform();
			Construct();
			SetupManipulation();
		}

		private void SetupManipulation()
		{
			InteractionEnabled = false;
			ManipulationDelta += NodeManipulationDelta;
		}

		void NodeManipulationDelta(object sender, ManipulationDeltaEventArgs args)
		{
			if (InteractionEnabled)
			{
				var world = World;
				var rotation = Matrix.CreateRotation((float)-args.DeltaY, (float)-args.DeltaX, 0);
				World = world * rotation;
			}
		}

		public string Label { get; set; }

		public bool InteractionEnabled { get; set; }
		private NodeStatistics _statistics;
		public NodeStatistics Statistics
		{
			get { return _statistics; }
			private set
			{
				_statistics = value;
#if(XAML)
				PropertyChanged.Notify(() => Statistics);
#endif
			}
		}

		protected virtual NodeStatistics GetStatisticsObject()
		{
			return new NodeStatistics();
		}


		private void Construct()
		{
#if(XAML)
			Loaded += NodeLoaded;
			Width = 0;
			Height = 0;
			MouseLeftButtonUp += (s, e) => OnCommand();
#endif
		}

#if(XAML)
		private void NodeLoaded(object sender, RoutedEventArgs e)
		{
			OnInitialize();
		}

		public static readonly Property<Node, ICommand> CommandProperty =
			Property<Node, ICommand>.Register(o => o.Command);
		public ICommand Command
		{
			get { return CommandProperty.GetValue(this); }
			set { CommandProperty.SetValue(this, value); }
		}

		public static readonly Property<Node, object> CommandParameterProperty =
			Property<Node, object>.Register(o => o.CommandParameter);
		public object CommandParameter
		{
			get { return CommandParameterProperty.GetValue(this); }
			set { CommandParameterProperty.SetValue(this, value); }
		}

		public static readonly Property<Node, ToolTip> ToolTipProperty =
			Property<Node, ToolTip>.Register(o => o.ToolTip);

		public virtual object DataItem { get; set; }


		/// <summary>
		/// Tooltip to use on node
		/// </summary>
		/// <remarks>
		/// The property has a TypeConverter which enables one to enter
		/// anything in the Xaml and it will be converted to a ToolTip
		/// object.
		/// </remarks>
		[TypeConverter(typeof(ToolTipTypeConverter))]
		public ToolTip ToolTip
		{
			get { return ToolTipProperty.GetValue(this); }
			set
			{
				ToolTipProperty.SetValue(this, value);
				NodeTooltipHelper.Register(this);
			}
		}

		public static readonly Property<Node, int> ToolTipStartDelayProperty =
			Property<Node, int>.Register(o => o.ToolTipStartDelay, 400);

		/// <summary>
		/// Gets or sets the delay when a node has the mouse over till the tooltip shows up 
		/// in milliseconds.
		/// </summary>
		/// <remarks>
		/// Default value is 400. Almost half a second.
		/// </remarks>
		public int ToolTipStartDelay
		{
			get { return ToolTipStartDelayProperty.GetValue(this); }
			set { ToolTipStartDelayProperty.SetValue(this, value); }
		}

		public static readonly Property<Node, int> ToolTipShowPeriodProperty =
			Property<Node, int>.Register(o => o.ToolTipShowPeriod, 5000);

		/// <summary>
		/// Gets or sets the period a tooltip should be visible while mouse is hovering over,
		/// in milliseconds
		/// </summary>
		/// <remarks>
		/// Default value is 5000 - 5 seconds.
		/// </remarks>
		public int ToolTipShowPeriod
		{
			get { return ToolTipShowPeriodProperty.GetValue(this); }
			set { ToolTipShowPeriodProperty.SetValue(this, value); }
		}


		protected void OnCommand()
		{
			if (null != Command)
			{
				if (Command.CanExecute(CommandParameter))
				{
					Command.Execute(CommandParameter);
				}
			}
		}

#endif

		private void InvalidateWorld()
		{
			_isWorldInvalidated = true;
		}


		private void InitializeTransform()
		{
			Position = new Coordinate();
			PivotPoint = new Coordinate();
			Scale = new Coordinate(1f, 1f, 1f);
			Rotation = new Coordinate();

			World = Matrix.Identity;
			ActualWorld = Matrix.Identity;

			InvalidateWorld();
		}

		public BoundingSphere BoundingSphere { get; set; }
		public BoundingSphere ActualBoundingSphere { get; protected set; }

		private Scene _scene;
		public Scene Scene
		{
			get { return _scene; }
			set
			{
				_scene = value;
				OnSceneSet(value);
			}
		}

		protected virtual void OnSceneSet(Scene scene)
		{
			if (this is IHaveChildren)
			{
				foreach (var child in ((IHaveChildren)this).Children)
				{
					child.Scene = scene;
				}
			}
		}


		public static readonly Property<Node, bool> IsHitTestEnabledProperty =
			Property<Node, bool>.Register(n => n.IsHitTestEnabled, true);
		public bool IsHitTestEnabled
		{
			get { return IsHitTestEnabledProperty.GetValue(this); }
			set { IsHitTestEnabledProperty.SetValue(this, value); }
		}


		public static readonly Property<Node, bool> IsIntersectionTestEnabledProperty =
			Property<Node, bool>.Register(n => n.IsIntersectionTestEnabled, true);
		public bool IsIntersectionTestEnabled
		{
			get { return IsIntersectionTestEnabledProperty.GetValue(this); }
			set { IsIntersectionTestEnabledProperty.SetValue(this, value); }
		}


		#region Transform

		public static readonly Property<Node, Coordinate> PivotPointProperty =
			Property<Node, Coordinate>.Register(n => n.PivotPoint, TransformChanged);
		public Coordinate PivotPoint
		{
			get { return PivotPointProperty.GetValue(this); }
			set
			{
				if (null == value)
				{
					value = new Coordinate();
				}
				PivotPointProperty.SetValue(this, value);
			}
		}



		public static readonly Property<Node, Coordinate> PositionProperty =
			Property<Node, Coordinate>.Register(t => t.Position, TransformChanged);
		/// <summary>
		/// Gets or sets the position of the node in 3D space
		/// </summary>
		public Coordinate Position
		{
			get { return PositionProperty.GetValue(this); }
			set
			{
				if (null == value)
				{
					value = new Coordinate(0, 0, 0);
				}
				PositionProperty.SetValue(this, value);
			}
		}


		public static readonly Property<Node, Coordinate> ScaleProperty =
			Property<Node, Coordinate>.Register(t => t.Scale, TransformChanged);

		/// <summary>
		/// Gets or sets the scale of the node
		/// </summary>
		/// <remarks>
		/// Default is X:1 Y:1 Z:1, which represents the node in a non-scaled form
		/// </remarks>
		public Coordinate Scale
		{
			get { return ScaleProperty.GetValue(this); }
			set
			{
				if (null == value)
				{
					value = new Coordinate(1, 1, 1);
				}
				ScaleProperty.SetValue(this, value);
			}
		}

		public static readonly Property<Node, Coordinate> RotationProperty =
			Property<Node, Coordinate>.Register(t => t.Rotation, TransformChanged);

		/// <summary>
		/// Gets or sets the rotation of the node in angles, 0-360 degrees
		/// </summary>
		public Coordinate Rotation
		{
			get { return RotationProperty.GetValue(this); }
			set
			{
				if (null == value)
				{
					value = new Coordinate(0, 0, 0);
				}
				RotationProperty.SetValue(this, value);
			}
		}

		private Matrix _previousWorld;
		private Matrix _world;
		public Matrix World
		{
			get { return _world; }
			set
			{
				_world = value;
				SignalRendering();
			}
		}

		protected void SignalRendering()
		{
			if (null != Scene)
			{
				Scene.RuntimeContext.SignalRendering();
			}
		}

#if(XAML)
		public new INode Parent { get; internal set; }
#else
		public INode Parent { get; internal set; }
#endif

		public Matrix ActualWorld { get; internal set; }


		private Matrix _renderingWorld;
		public Matrix RenderingWorld
		{
			get { return _renderingWorld; }
			set
			{
				_renderingWorld = value;
				ActualBoundingSphere = BoundingSphere.Transform(value);
			}
		}

		// TODO: this method should be split up and renamed!
		internal void PrepareActualWorld()
		{
			var matrix = Matrix.Identity;
			if (PivotPoint != null && (PivotPoint.X != 0f || PivotPoint.Y != 0f || PivotPoint.Z != 0f))
			{
				var negativePivot = PivotPoint.ToVector().Negative();
				var pivotMatrix = Matrix.CreateTranslation(negativePivot);
				matrix = matrix * pivotMatrix;
			}

			if (!World.IsIdentity)
			{
				matrix = matrix * World;
			}

			if (Scale != null && (Scale.X != 1f || Scale.Y != 1f || Scale.Z != 1f))
			{
				var scaleMatrix = Matrix.CreateScale(Scale);
				matrix = matrix * scaleMatrix;
			}

			if (Rotation != null && (Rotation.X != 0f || Rotation.Y != 0f || Rotation.Z != 0f))
			{
				var rotationMatrix = Matrix.CreateRotation((float)Rotation.X, (float)Rotation.Y, (float)Rotation.Z);
				matrix = matrix * rotationMatrix;
			}

			if (Position != null && (Position.X != 0f || Position.Y != 0f || Position.Z != 0f))
			{
				var translationMatrix = Matrix.CreateTranslation(Position);
				matrix = matrix * translationMatrix;
			}

			ActualWorld = matrix;
		}

#if(false)		
		internal void PrepareActualWorld	()
		{
			var matrix = Matrix.Identity;
			var negativePivot = PivotPoint.ToVector().Negative();
			var pivotMatrix = Matrix.CreateTranslation(negativePivot);
			matrix = matrix * pivotMatrix;
			matrix = matrix * World;
			var scaleMatrix = Matrix.CreateScale(Scale);
			matrix = matrix * scaleMatrix;
			var rotationMatrix = Matrix.CreateRotation((float)Rotation.X, (float)Rotation.Y, (float)Rotation.Z);
			matrix = matrix * rotationMatrix;
			var translationMatrix = Matrix.CreateTranslation(Position);
			matrix = matrix * translationMatrix;
			ActualWorld = matrix;
	

			var isDifferent = false;
			if (null == _previousWorld || !_previousWorld.EqualsTo(World))
			{
				isDifferent = true;
				_previousWorld = World.Clone();
			}
			var isWorldIdentity = World.IsIdentity;
			var matrix = Matrix.Identity;
			if (!_isWorldInvalidated && isWorldIdentity && !isDifferent)
			{
				return;
			}
			if (_isForcePrepareMatrices || PivotPoint.X != 0f || PivotPoint.Y != 0f || PivotPoint.Z != 0f)
			{
				var negativePivot = PivotPoint.ToVector().Negative();
				var pivotMatrix = Matrix.CreateTranslation(negativePivot);
				matrix = matrix * pivotMatrix;
			}

			if (!isWorldIdentity)
			{
				matrix = matrix * World;
			}

			if (_isForcePrepareMatrices || Scale.X != 1f || Scale.Y != 1f || Scale.Z != 1f)
			{
				var scaleMatrix = Matrix.CreateScale(Scale);
				matrix = matrix * scaleMatrix;
			}

			if (_isForcePrepareMatrices || Rotation.X != 0f || Rotation.Y != 0f || Rotation.Z != 0f)
			{
				var rotationMatrix = Matrix.CreateRotation((float)Rotation.X, (float)Rotation.Y, (float)Rotation.Z);
				matrix = matrix * rotationMatrix;
			}

			if (_isForcePrepareMatrices || Position.X != 0f || Position.Y != 0f || Position.Z != 0f)
			{
				var translationMatrix = Matrix.CreateTranslation(Position);
				matrix = matrix * translationMatrix;
			}

			ActualWorld = matrix;

			_isForcePrepareMatrices = false;
			_isWorldInvalidated = false;
		}
#endif

		private static void TransformChanged(object node, Coordinate oldCoordinate, Coordinate newCoordinate)
		{
			((Node)node).InvalidateWorld();
		}
		#endregion



		public object Clone()
		{
			return Clone(false);
		}

		public virtual object Clone(bool unique)
		{
			var clone = NodeCloner.Instance.Clone(this, unique) as Node;
			clone.IsClone = !unique;
			return clone;
		}

		protected bool IsClone { get; private set; }

		public void InvalidatePrepare()
		{
			_isPrepared = false;
			if (null != RuntimeContext)
			{
				RuntimeContext.SignalRendering();
			}
		}

		public virtual void BeforeRendering(Viewport viewport, Matrix view, Matrix projection, Matrix world) { }

		public virtual void PrepareBoundingSphere()
		{

		}

		public virtual void Prepare(Viewport viewport)
		{
			OnPrepared();
		}

		protected virtual void Initialize() { }


		protected void OnPrepared()
		{
			PreparedEvent.Raise(this, this, new BubbledEventArgs());
		}

		protected void OnContentPrepared()
		{
			ContentPreparedEvent.Raise(this, this, new BubbledEventArgs());
		}

		#region Internal EventModel
		internal void OnInitialize()
		{
			if (_isInitialized)
			{
				return;
			}

			Statistics = GetStatisticsObject();
			Initialize();
			_isInitialized = true;
		}


		internal void OnHover()
		{
			Hover(this, DefaultEventArgs);
		}

		internal void OnClick()
		{
			Click(this, DefaultEventArgs);
		}


		internal void OnPrepare(Viewport viewport)
		{
			PrepareActualWorld();
			if (IsClone || _isPrepared)
			{
				return;
			}
			if (!_isInitialized)
			{
				OnInitialize();
			}

			_isPrepared = true;
			Prepare(viewport);
		}

		internal void OnPrepareBoundingSphere()
		{
			PrepareBoundingSphere();
		}
		#endregion

		IPropertyContainer _propertyContainer;
		public IPropertyContainer PropertyContainer
		{
			get
			{
				if (_propertyContainer == null)
					_propertyContainer = new PropertyContainer(this);

				return _propertyContainer;
			}
		}

		public IRuntimeContext RuntimeContext
		{
			get
			{
				if (null != Scene)
				{
					return Scene.RuntimeContext;
				}
#if(XAML)
				else
				{
					var parent = VisualTreeHelper.GetParent(this);
					if (parent is IHaveRuntimeContext)
					{
						return ((IHaveRuntimeContext)parent).RuntimeContext;
					}
				}
#endif
				return null;
			}
		}
	}
}
