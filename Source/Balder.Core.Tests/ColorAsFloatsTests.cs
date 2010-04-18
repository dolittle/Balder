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
using NUnit.Framework;

namespace Balder.Core.Tests
{
	[TestFixture]
	public class ColorAsFloatsTests
	{
		[Test]
		public void AddingShouldClampToMaximumValue()
		{
			var firstColor = new ColorAsFloats { Red = 1f };
			var secondColor = new ColorAsFloats { Red = 1f };

			var result = firstColor + secondColor;
			Assert.That(result.Red, Is.EqualTo(1f));
		}

		[Test]
		public void SubtractingShouldClampToMinimumValue()
		{
			var firstColor = new ColorAsFloats { Red = 0f };
			var secondColor = new ColorAsFloats { Red = 1f };

			var result = firstColor - secondColor;
			Assert.That(result.Red, Is.EqualTo(0f));
		}

		[Test]
		public void ClampingShouldKeepValuesWithinLegalRange()
		{
			var firstColor = new ColorAsFloats { Red = 1f };
			var secondColor = new ColorAsFloats { Red = 1f };

			var result = firstColor + secondColor;
			result.Clamp();
			Assert.That(result.Red, Is.EqualTo(1f));

			result = firstColor - secondColor - secondColor;
			result.Clamp();
			Assert.That(result.Red, Is.EqualTo((0f)));
		}


		[Test]
		public void EqualColorsShouldReturnTrueWhenComparing()
		{
			var firstColor = new ColorAsFloats { Red = 1, Green = 0, Blue = 1, Alpha = 1 };
			var secondColor = new ColorAsFloats { Red = 1, Green = 0, Blue = 1, Alpha = 1 };
			var actual = firstColor.Equals(secondColor);
			Assert.That(actual, Is.True);
		}

		[Test]
		public void ScalingColorShouldScaleAllComponentsCorrectly()
		{
			var color = new ColorAsFloats
			{
				Red = 0.1f,
				Green = 0.2f,
				Blue = 0.3f,
				Alpha = 0.4f
			};
			var scaledColor = color * 2f;
			Assert.That(scaledColor.Red, Is.EqualTo(0.2f));
			Assert.That(scaledColor.Green, Is.EqualTo(0.4f));
			Assert.That(scaledColor.Blue, Is.EqualTo(0.6f));
			Assert.That(scaledColor.Alpha, Is.EqualTo(0.8f));
		}


		[Test, Ignore]
		public void ConvertingToColorShouldGiveCorrectValues()
		{
			throw new NotImplementedException();
		}

		[Test, Ignore]
		public void ConvertingToSystemColorShouldGiveCorrectValues()
		{
			throw new NotImplementedException();
		}
	}
}

