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
using System.Windows.Media.Animation;
using Balder.Math;

namespace Balder.Animation.Silverlight
{
	/// <summary>
	/// Provides extensions for working with <see cref="Coordinate"/> animation in Silverlight
	/// </summary>
	public static class StoryboardExtensions
	{
		/// <summary>
		/// Identifies the Coordinate Animation dependency property 
		/// </summary>
		public static readonly DependencyProperty CoordinateAnimationProperty =
			DependencyProperty.RegisterAttached("CoordinateAnimation", typeof (CoordinateAnimation),
			                                    typeof (StoryboardExtensions), null);

		/// <summary>
		/// Set a <see cref="Coordinate"/> animation for a specific dependency object
		/// </summary>
		/// <param name="dependencyObject"><see cref="DependencyObject"/> to set <see cref="CoordinateAnimation"/> for</param>
		/// <param name="coordinateAnimation">An instance of a <see cref="CoordinateAnimation"/> to set</param>
		public static void SetCoordinateAnimation(DependencyObject dependencyObject, CoordinateAnimation coordinateAnimation)
		{
			var storyboard = dependencyObject as Storyboard;

			var xAnimation = new DoubleAnimation();
			xAnimation.From = coordinateAnimation.From.X;
			xAnimation.To = coordinateAnimation.To.X;
			xAnimation.BeginTime = coordinateAnimation.BeginTime;
			xAnimation.Duration = coordinateAnimation.Duration;
			xAnimation.EasingFunction = coordinateAnimation.EasingFunction;

			if (coordinateAnimation.Target != null)
			{
				Storyboard.SetTarget(xAnimation, coordinateAnimation.Target);
			}
			Storyboard.SetTargetName(xAnimation, coordinateAnimation.TargetName);
			Storyboard.SetTargetProperty(xAnimation, new PropertyPath(string.Format("{0}.(X)",coordinateAnimation.TargetProperty)));
			storyboard.Children.Add(xAnimation);


			var yAnimation = new DoubleAnimation();
			yAnimation.From = coordinateAnimation.From.Y;
			yAnimation.To = coordinateAnimation.To.Y;
			yAnimation.BeginTime = coordinateAnimation.BeginTime;
			yAnimation.Duration = coordinateAnimation.Duration;
			yAnimation.EasingFunction = coordinateAnimation.EasingFunction;
			if (coordinateAnimation.Target != null)
			{
				Storyboard.SetTarget(yAnimation, coordinateAnimation.Target);
			}
			Storyboard.SetTargetName(yAnimation, coordinateAnimation.TargetName);
			Storyboard.SetTargetProperty(yAnimation, new PropertyPath(string.Format("{0}.(Y)", coordinateAnimation.TargetProperty)));
			storyboard.Children.Add(yAnimation);


			var zAnimation = new DoubleAnimation();
			zAnimation.From = coordinateAnimation.From.Z;
			zAnimation.To = coordinateAnimation.To.Z;
			zAnimation.BeginTime = coordinateAnimation.BeginTime;
			zAnimation.Duration = coordinateAnimation.Duration;
			zAnimation.EasingFunction = coordinateAnimation.EasingFunction;
			if (coordinateAnimation.Target != null)
			{
				Storyboard.SetTarget(zAnimation, coordinateAnimation.Target);
			}
			Storyboard.SetTargetName(zAnimation, coordinateAnimation.TargetName);
			Storyboard.SetTargetProperty(zAnimation, new PropertyPath(string.Format("{0}.(Z)", coordinateAnimation.TargetProperty)));
			storyboard.Children.Add(zAnimation);
			
			dependencyObject.SetValue(CoordinateAnimationProperty,coordinateAnimation);
		}

		public static CoordinateAnimation GetCoordinateAnimation(DependencyObject obj)
		{
			var animation = obj.GetValue(CoordinateAnimationProperty) as CoordinateAnimation;
			return animation;
		}
	}
}
#endif