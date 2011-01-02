using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Animation;
using Balder.Math;

namespace Balder.Silverlight.SampleBrowser.Samples.Creative.CoverFlow
{
	public partial class CoverFlow
	{
		public static DependencyProperty CoverProperty =
			DependencyProperty.Register("Cover", typeof(double), typeof(CoverFlow), new PropertyMetadata(0d, OnCoverChanged));


		public const float CameraYPos = 0f;
		public const float CameraZPos = 100f;
		public const float CoverSpace = 5f;
		public const float CoverChangeDuration = 500f;
		public const float CurrentCoverOpeningSpace = 5f;

		private readonly List<Cover> _covers;
		private int _currentCoverIndex;

		private readonly Storyboard _coverStoryboard;
		private readonly DoubleAnimation _coverAnimation;


		public CoverFlow()
		{
			InitializeComponent();
			_covers = new List<Cover>();

			_camera.Position = new Vector(0, CameraYPos, CameraZPos);

			_coverStoryboard = new Storyboard();
			_coverAnimation = new DoubleAnimation();
			_coverAnimation.Duration = TimeSpan.FromMilliseconds(CoverChangeDuration);
			_coverStoryboard.Children.Add(_coverAnimation);
			Storyboard.SetTarget(_coverAnimation, this);
			Storyboard.SetTargetProperty(_coverAnimation, new PropertyPath("(CoverFlow.Cover)"));
		}

		public void AddCover()
		{
			var coverPos = _covers.Count * CoverSpace;
			var cover = new Cover();
			_covers.Add(cover);
			_game.Scene.AddNode(cover);

			cover.Position = new Vector((float) coverPos, 0, 0);
			Update();

		}

		public void Update()
		{
			var index = 0;
			foreach (var cover in _covers)
			{
				if (index == _currentCoverIndex)
				{
					cover.MoveToFront();
				}
				else if (index < _currentCoverIndex)
				{
					cover.MoveToLeft();
				}
				else
				{
					cover.MoveToRight();
				}

				index++;
			}
		}

		private int CurrentCoverIndex
		{
			get
			{
				return _currentCoverIndex;
			}
			set
			{
				_currentCoverIndex = value;
				Update();
			}
		}

		public double Cover
		{
			get
			{
				return (double)GetValue(CoverProperty);
			}
			set
			{
				SetValue(CoverProperty, value);
			}
		}



		public static void OnCoverChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			var coverFlow = (CoverFlow)obj;
			coverFlow.CurrentCoverIndex = Convert.ToInt32(e.NewValue);

			var xpos = ((double)e.NewValue) * CoverSpace;

			coverFlow._camera.Target = new Vector((float)xpos, 0, 0);
			coverFlow._camera.Position = new Vector((float)xpos, CameraYPos, CameraZPos);

			coverFlow.Cover = (double)e.NewValue;

		}

		public int CurrentlyBeingMovedTo
		{
			get; private set;
		}

		public void MoveTo(int cover)
		{
			CurrentlyBeingMovedTo = cover;
			_coverAnimation.To = cover;
			_coverStoryboard.Begin();
		}

		public void MoveNext()
		{
			var next = CurrentlyBeingMovedTo + 1;
			if( next >= _covers.Count )
			{
				next = _covers.Count - 1;
			}
			MoveTo(next);
		}

		public void MovePrevious()
		{
			var next = CurrentlyBeingMovedTo - 1;
			if( next < 0 )
			{
				next = 0;
			}
			MoveTo(next);
		}
	}
}
