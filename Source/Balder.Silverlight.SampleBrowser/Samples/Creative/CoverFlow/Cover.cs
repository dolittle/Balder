using System;
using System.Windows;
using System.Windows.Media.Animation;
using Balder.Materials;
using Balder.Objects.Geometries;

namespace Balder.Silverlight.SampleBrowser.Samples.Creative.CoverFlow
{
	public class Cover : Geometry
	{
		public const float CoverWidth = 10f;
		public const float CoverHeight = 10f;
		public const float CoverAngle = 85f;
		public const float CoverMoveDuration = 100f;
		public const float CoverOffset = 10f;

		private readonly Storyboard _moveToLeftStoryboard;
		private readonly Storyboard _moveToRightStoryboard;
		private readonly Storyboard _moveToFrontStoryboard;

		public override void Prepare(Display.Viewport viewport)
		{
			FullDetailLevel.AllocateVertices(4);
			FullDetailLevel.SetVertex(0, new Vertex(-CoverWidth, -CoverHeight, 0));
			FullDetailLevel.SetVertex(1, new Vertex(CoverWidth, -CoverHeight, 0));
			FullDetailLevel.SetVertex(2, new Vertex(-CoverWidth, CoverHeight, 0));
			FullDetailLevel.SetVertex(3, new Vertex(CoverWidth, CoverHeight, 0));

			FullDetailLevel.AllocateTextureCoordinates(4);
			FullDetailLevel.SetTextureCoordinate(0, new TextureCoordinate(0f, 0f));
			FullDetailLevel.SetTextureCoordinate(1, new TextureCoordinate(1f, 0f));
			FullDetailLevel.SetTextureCoordinate(2, new TextureCoordinate(0f, 1f));
			FullDetailLevel.SetTextureCoordinate(3, new TextureCoordinate(1f, 1f));

			FullDetailLevel.AllocateFaces(2);
			FullDetailLevel.SetFace(0, new Face(0, 1, 2));
			FullDetailLevel.SetFace(1, new Face(3, 2, 1));
			
			
			base.Prepare(viewport);
		}

		public Cover()
		{
			//Material material = new Material(new Uri("Empty.png",UriKind.Relative));
			
			// Move left
			_moveToLeftStoryboard = new Storyboard();
			var moveToLeftAnimation = new DoubleAnimation();
			moveToLeftAnimation.To = CoverAngle;
			moveToLeftAnimation.Duration = TimeSpan.FromMilliseconds(CoverMoveDuration);
			_moveToLeftStoryboard.Children.Add(moveToLeftAnimation);
			Storyboard.SetTarget(moveToLeftAnimation, this);
			Storyboard.SetTargetProperty(moveToLeftAnimation, new PropertyPath("(Node.Rotation).Y"));

			var offsetToLeftAnimation = new DoubleAnimation();
			offsetToLeftAnimation.To = -CoverOffset;
			offsetToLeftAnimation.Duration = TimeSpan.FromMilliseconds(CoverMoveDuration);
			_moveToLeftStoryboard.Children.Add(offsetToLeftAnimation);
			Storyboard.SetTarget(offsetToLeftAnimation, this);
			Storyboard.SetTargetProperty(offsetToLeftAnimation, new PropertyPath("(Node.Position).X"));

			var moveLeftBackwardAnimation = new DoubleAnimation();
			moveLeftBackwardAnimation.To = 0;
			moveLeftBackwardAnimation.Duration = TimeSpan.FromMilliseconds(CoverMoveDuration);
			_moveToLeftStoryboard.Children.Add(moveLeftBackwardAnimation);
			Storyboard.SetTarget(moveLeftBackwardAnimation, this);
			Storyboard.SetTargetProperty(moveLeftBackwardAnimation, new PropertyPath("(Node.Position).Z"));


			// Move right
			_moveToRightStoryboard = new Storyboard();
			var moveToRightAnimation = new DoubleAnimation();
			moveToRightAnimation.To = -CoverAngle;
			moveToRightAnimation.Duration = TimeSpan.FromMilliseconds(CoverMoveDuration);
			_moveToRightStoryboard.Children.Add(moveToRightAnimation);
			Storyboard.SetTarget(moveToRightAnimation, this);
			Storyboard.SetTargetProperty(moveToRightAnimation, new PropertyPath("(Node.Rotation).Y"));

			var offsetToRightAnimation = new DoubleAnimation();
			offsetToRightAnimation.To = CoverOffset;
			offsetToRightAnimation.Duration = TimeSpan.FromMilliseconds(CoverMoveDuration);
			_moveToRightStoryboard.Children.Add(offsetToRightAnimation);
			Storyboard.SetTarget(offsetToRightAnimation, this);
			Storyboard.SetTargetProperty(offsetToRightAnimation, new PropertyPath("(Node.Position).X"));

			var moveRightBackwardAnimation = new DoubleAnimation();
			moveRightBackwardAnimation.To = 0;
			moveRightBackwardAnimation.Duration = TimeSpan.FromMilliseconds(CoverMoveDuration);
			_moveToRightStoryboard.Children.Add(moveRightBackwardAnimation);
			Storyboard.SetTarget(moveRightBackwardAnimation, this);
			Storyboard.SetTargetProperty(moveRightBackwardAnimation, new PropertyPath("(Node.Position).Z"));


			// Move front
			_moveToFrontStoryboard = new Storyboard();
			var moveToFrontAnimation = new DoubleAnimation();
			moveToFrontAnimation.To = 0d;
			moveToFrontAnimation.Duration = TimeSpan.FromMilliseconds(CoverMoveDuration);
			_moveToFrontStoryboard.Children.Add(moveToFrontAnimation);
			Storyboard.SetTarget(moveToFrontAnimation, this);
			Storyboard.SetTargetProperty(moveToFrontAnimation, new PropertyPath("(Node.Rotation).Y"));

			var offsetToFrontAnimation = new DoubleAnimation();
			offsetToFrontAnimation.To = 0;
			offsetToFrontAnimation.Duration = TimeSpan.FromMilliseconds(CoverMoveDuration);
			_moveToFrontStoryboard.Children.Add(offsetToFrontAnimation);
			Storyboard.SetTarget(offsetToFrontAnimation, this);
			Storyboard.SetTargetProperty(offsetToFrontAnimation, new PropertyPath("(Node.Position).X"));

			var moveForwardAnimation = new DoubleAnimation();
			moveForwardAnimation.To = 20;
			moveForwardAnimation.Duration = TimeSpan.FromMilliseconds(CoverMoveDuration);
			_moveToFrontStoryboard.Children.Add(moveForwardAnimation);
			Storyboard.SetTarget(moveForwardAnimation, this);
			Storyboard.SetTargetProperty(moveForwardAnimation, new PropertyPath("(Node.Position).Z"));


		}

		public void MoveToLeft()
		{
			_moveToLeftStoryboard.Begin();
			_moveToRightStoryboard.Pause();
			_moveToFrontStoryboard.Pause();
		}

		public void MoveToFront()
		{
			_moveToLeftStoryboard.Pause();
			_moveToRightStoryboard.Pause();
			_moveToFrontStoryboard.Begin();
		}

		public void MoveToRight()
		{
			_moveToLeftStoryboard.Pause();
			_moveToRightStoryboard.Begin();
			_moveToFrontStoryboard.Pause();
		}
	}
}
