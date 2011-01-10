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
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Balder.Math;

namespace Balder.Animation.Silverlight
{
	/// <summary>
	/// Represents an animation for a <see cref="Coordinate"/>
	/// </summary>
	public class CoordinateAnimation
	{
		/// <summary>
		/// Gets or sets the target name of the object containing the <see cref="Coordinate"/> to animate
		/// </summary>
		public string TargetName { get; set; }

		/// <summary>
		/// Gets or sets the target coordinate property to animate
		/// </summary>
		public string TargetProperty { get; set; }

		/// <summary>
		/// Gets or sets the target dependency object that holds the <see cref="Coordinate"/> to animate
		/// </summary>
		public DependencyObject Target { get; set; }

		/// <summary>
		/// Gets or sets the from value for the <see cref="Coordinate"/>
		/// </summary>
		public Coordinate From { get; set; }

		/// <summary>
		/// Gets or sets the to value for the <see cref="Coordinate"/>
		/// </summary>
		public Coordinate To { get; set; }


		/// <summary>
		/// Gets or sets the <see cref="IEasingFunction"/> to use as interpolation in the animation
		/// </summary>
		public IEasingFunction EasingFunction { get; set; }

		/// <summary>
		/// Gets or sets the time the animation starts
		/// </summary>
		public TimeSpan? BeginTime { get; set; }

		/// <summary>
		/// Gets or sets the duration of the animation
		/// </summary>
		public Duration Duration { get; set; }
	}
}
#endif