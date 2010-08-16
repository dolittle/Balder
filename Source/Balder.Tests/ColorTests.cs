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
using NUnit.Framework;

namespace Balder.Tests
{
	[TestFixture]
	public class ColorTests
	{

		[Test]
		public void ConvertingToUInt32ShouldConvertAllComponentsCorrectly()
		{
			var redColor = new Color { Red = 0xff };
			var redResult = redColor.ToUInt32();
			Assert.That(redResult, Is.EqualTo(0x00ff0000));
			var greenColor = new Color { Green = 0xff };
			var greenResult = greenColor.ToUInt32();
			Assert.That(greenResult, Is.EqualTo(0x0000ff00));
			var blueColor = new Color { Blue = 0xff };
			var blueResult = blueColor.ToUInt32();
			Assert.That(blueResult, Is.EqualTo(0x000000ff));
			var alphaColor = new Color { Alpha = 0xff };
			var alphaResult = alphaColor.ToUInt32();
			Assert.That(alphaResult, Is.EqualTo(0xff000000));
			var redAndAlphaColor = new Color { Red = 0xff, Alpha = 0xff };
			var redAndAlphaResult = redAndAlphaColor.ToUInt32();
			Assert.That(redAndAlphaResult, Is.EqualTo(0xffff0000));
			var greenAndAlphaColor = new Color { Green = 0xff, Alpha = 0xff };
			var greenAndAlphaResult = greenAndAlphaColor.ToUInt32();
			Assert.That(greenAndAlphaResult, Is.EqualTo(0xff00ff00));
			var redAndBlueColor = new Color { Red = 0xff, Blue = 0xff };
			var redAndBlueResult = redAndBlueColor.ToUInt32();
			Assert.That(redAndBlueResult, Is.EqualTo(0x00ff00ff));
		}

		[Test]
		public void ConvertingToSystemColorShouldConvertAllComponentsCorrectly()
		{
			const byte red = 0x10;
			const byte green = 0x20;
			const byte blue = 0x30;
			const byte alpha = 0x40;

			var color = new Color
							{
								Red = red,
								Green = green,
								Blue = blue,
								Alpha = alpha
							};
			var sysColor = color.ToSystemColor();
			Assert.That(sysColor.R, Is.EqualTo(red));
			Assert.That(sysColor.G, Is.EqualTo(green));
			Assert.That(sysColor.B, Is.EqualTo(blue));
			Assert.That(sysColor.A, Is.EqualTo(alpha));
		}

		[Test]
		public void AddingTwoColorsShouldClampAtMaxValue()
		{
			var firstColor = new Color {Red = 0x81, Green = 0x81, Blue = 0x81, Alpha = 0x81};
			var secondColor = new Color { Red = 0x81, Green = 0x81, Blue = 0x81, Alpha = 0x81 };

			var result = firstColor.Additive(secondColor);

			Assert.That(result.Red, Is.EqualTo(0xff));
			Assert.That(result.Green, Is.EqualTo(0xff));
			Assert.That(result.Blue, Is.EqualTo(0xff));
			Assert.That(result.Alpha, Is.EqualTo(0xff));
		}

		[Test]
		public void AddingThreeColorsShouldClampAtMaxValue()
		{
			var firstColor = new Color { Red = 0x81, Green = 0x81, Blue = 0x81, Alpha = 0x81 };
			var secondColor = new Color { Red = 0x81, Green = 0x81, Blue = 0x81, Alpha = 0x81 };
			var thirdColor = new Color { Red = 0x81, Green = 0x81, Blue = 0x81, Alpha = 0x81 };

			var result = firstColor.Additive(secondColor);
			result = result.Additive(thirdColor);

			Assert.That(result.Red, Is.EqualTo(0xff));
			Assert.That(result.Green, Is.EqualTo(0xff));
			Assert.That(result.Blue, Is.EqualTo(0xff));
			Assert.That(result.Alpha, Is.EqualTo(0xff));
		}

		[Test]
		public void MultiplyingIntegerColorWithOnlyRedSetShouldProduceCorrectResult()
		{
			var color1 = new Color {Red = 0x80, Alpha = 0};
			var color2 = new Color { Red = 0x80, Alpha = 0};

			var color1AsInt = color1.ToInt();
			var color2AsInt = color2.ToInt();

			var result = Color.Multiply(color1AsInt, color2AsInt);

			Assert.That(result, Is.EqualTo(0x400000));
		}

		[Test]
		public void MultiplyingIntegerColorWithOnlyGreenSetShouldProduceCorrectResult()
		{
			var color1 = new Color { Green = 0x80, Alpha = 0};
			var color2 = new Color { Green = 0x80, Alpha = 0 };

			var color1AsInt = color1.ToInt();
			var color2AsInt = color2.ToInt();

			var result = Color.Multiply(color1AsInt, color2AsInt);

			Assert.That(result, Is.EqualTo(0x4000));
		}

		[Test]
		public void MultiplyingIntegerColorWithOnlyBlueSetShouldProduceCorrectResult()
		{
			var color1 = new Color { Blue = 0x80, Alpha = 0 };
			var color2 = new Color { Blue = 0x80, Alpha = 0 };

			var color1AsInt = color1.ToInt();
			var color2AsInt = color2.ToInt();

			var result = Color.Multiply(color1AsInt, color2AsInt);

			Assert.That(result, Is.EqualTo(0x40));
		}

		[Test]
		public void MultiplyingIntegerColorWithOnlyAlphaSetShouldProduceCorrectResult()
		{
			var color1 = new Color { Alpha = 0x80 };
			var color2 = new Color { Alpha = 0x80 };

			var color1AsInt = color1.ToInt();
			var color2AsInt = color2.ToInt();

			var result = Color.Multiply(color1AsInt, color2AsInt);

			var expected = 0x40000000;
			Assert.That(result, Is.EqualTo((int)expected));
		}

		[Test]
		public void MultiplyingIntegerColorWithAllComponentsSetShouldProduceCorrectResult()
		{
			var color1 = new Color { Red = 0xc0, Green = 0xc0, Blue = 0xc0, Alpha = 0xc0 };
			var color2 = new Color { Red = 0xc0, Green = 0xc0, Blue = 0xc0, Alpha = 0xc0 };

			var color1AsInt = color1.ToInt();
			var color2AsInt = color2.ToInt();

			var result = Color.Multiply(color1AsInt, color2AsInt);

			var expected = 0x90909090;
			Assert.That(result, Is.EqualTo((int)expected));
		}

	}
}
