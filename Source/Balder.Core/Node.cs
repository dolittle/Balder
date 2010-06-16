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
using System.ComponentModel;
using Balder.Core.Display;
using Balder.Core.Execution;
using Balder.Core.Input;
using Balder.Core.Math;
using Balder.Core.Rendering;
using Matrix = Balder.Core.Math.Matrix;

namespace Balder.Core
{
	/// <summary>
	/// Abstract class representing a node in a scene
	/// </summary>
	public abstract partial class Node : INode, ICloneable, ICanPrepare, IHaveIdentity
	{
		private static readonly EventArgs DefaultEventArgs = new EventArgs();

		public static readonly BubbledEvent<Node, BubbledEventHandler> PreparedEvent =
			BubbledEvent<Node, BubbledEventHandler>.Register(n => n.Prepared);

		public static readonly BubbledEvent<Node, BubbledEventHandler> ContentPreparedEvent =
			BubbledEvent<Node, BubbledEventHandler>.Register(n => n.ContentPrepared);

		public static readonly BubbledEvent<Node, ManipulationDeltaEventHandler> ManipulationStartedEvent =
			BubbledEvent<Node, ManipulationDeltaEventHandler>.Register(n => n.ManipulationStarted);

		public static readonly BubbledEvent<Node, BubbledEventHandler> ManipulationStoppedEvent =
			BubbledEvent<Node, BubbledEventHandler>.Register(n => n.ManipulationStopped);

		public static readonly BubbledEvent<Node, ManipulationDeltaEventHandler> ManipulationDeltaEvent =
			BubbledEvent<Node, ManipulationDeltaEventHandler>.Register(n => n.ManipulationDelta);

		public event BubbledEventHandler Prepared = (s, e) => { };
		public event BubbledEventHandler ContentPrepared = (s, e) => { };
		public event ManipulationDeltaEventHandler ManipulationStarted = (s, e) => { };
		public event BubbledEventHandler ManipulationStopped = (s, e) => { };
		public event ManipulationDeltaEventHandler ManipulationDelta = (s, e) => { }; 

		public event EventHandler Hover = (s, e) => { };
		public event EventHandler Click = (s, e) => { };

		private bool _isWired = false;
		private bool _isPrepared = false;
		private bool _isInitialized = false;
		private bool _isWorldInvalidated = false;
		private bool _isForcePrepareMatrices = true;

		protected Node()
			: this(Runtime.Instance.Kernel.Get<IIdentityManager>())
		{
		}

		protected Node(IIdentityManager identityManager)
		{
			Id = identityManager.AllocateIdentity<Node>();
			InitializeTransform();
			Construct();
		}

		partial void Construct();

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
		public Scene Scene { get; set; }

		#region Transform

		public static readonly Property<Node, Coordinate> PivotPointProperty = Property<Node, Coordinate>.Register(n => n.PivotPoint);
		private Coordinate _pivotPoint;
		public Coordinate PivotPoint
		{
			get { return PivotPointProperty.GetValue(this); }
			set
			{
				if (null != _pivotPoint)
				{
					_pivotPoint.PropertyChanged -= TransformChanged;
				}
				if (null == value)
				{
					value = new Coordinate();
				}
				PivotPointProperty.SetValue(this, value);
				_pivotPoint = value;
				_pivotPoint.PropertyChanged += TransformChanged;
				InvalidateWorld();
			}
		}



		public static readonly Property<Node, Coordinate> PositionProp =
			Property<Node, Coordinate>.Register(t => t.Position);
		private Coordinate _position;
		/// <summary>
		/// Gets or sets the position of the node in 3D space
		/// </summary>
		public Coordinate Position
		{
			get { return PositionProp.GetValue(this); }
			set
			{
				if (null != _position)
				{
					_position.PropertyChanged -= TransformChanged;
				}
				if (null == value)
				{
					value = new Coordinate(0, 0, 0);
				}
				PositionProp.SetValue(this, value);
				_position = value;
				_position.PropertyChanged += TransformChanged;
				InvalidateWorld();
			}
		}


		public static readonly Property<Node, Coordinate> ScaleProp =
			Property<Node, Coordinate>.Register(t => t.Scale);

		private Coordinate _scale;

		/// <summary>
		/// Gets or sets the scale of the node
		/// </summary>
		/// <remarks>
		/// Default is X:1 Y:1 Z:1, which represents the node in a non-scaled form
		/// </remarks>
		public Coordinate Scale
		{
			get { return ScaleProp.GetValue(this); }
			set
			{
				if (null != _scale)
				{
					_scale.PropertyChanged -= TransformChanged;
				}
				if (null == value)
				{
					value = new Coordinate(1, 1, 1);
				}
				ScaleProp.SetValue(this, value);
				_scale = value;
				_scale.PropertyChanged += TransformChanged;
				InvalidateWorld();
			}
		}

		public static readonly Property<Node, Coordinate> RotationProp =
			Property<Node, Coordinate>.Register(t => t.Rotation);

		private Coordinate _rotation;

		/// <summary>
		/// Gets or sets the rotation of the node in angles, 0-360 degrees
		/// </summary>
		public Coordinate Rotation
		{
			get { return RotationProp.GetValue(this); }
			set
			{
				if (null != _rotation)
				{
					_rotation.PropertyChanged -= TransformChanged;
				}
				if (null == value)
				{
					value = new Coordinate(0, 0, 0);
				}
				RotationProp.SetValue(this, value);
				_rotation = value;
				_rotation.PropertyChanged += TransformChanged;
				InvalidateWorld();
			}
		}

		private Matrix _world;
		public Matrix World
		{
			get { return _world; }
			set { _world = value; }
		}

#if(SILVERLIGHT)
		public new INode Parent { get; internal set; }
#else
		public INode Parent { get; internal set; }
#endif

		public Matrix ActualWorld { get; private set; }
		public Matrix RenderingWorld { get; set; }

		private void PrepareActualWorld()
		{
			var isWorldIdentity = World.IsIdentity;

			if (!_isWorldInvalidated && isWorldIdentity)
			{
				return;
			}

			var matrix = Matrix.Identity;


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

		private void TransformChanged(object sender, PropertyChangedEventArgs e)
		{
			InvalidateWorld();
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
		}

		public virtual void BeforeRendering(Viewport viewport, Matrix view, Matrix projection, Matrix world) { }

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

			if (!_isWired)
			{
				Runtime.Instance.WireUpDependencies(this);
				_isWired = true;
			}

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
		#endregion

		public UInt16 Id { get; private set; }
	}
}
