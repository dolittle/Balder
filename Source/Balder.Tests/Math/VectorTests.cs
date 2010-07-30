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

using Balder.Math;
using NUnit.Framework;

namespace Balder.Tests.Math
{
	[TestFixture]
	public class VectorTests
	{
		[Test]
		public void RoundingWithoutSpecifyingDigitsShouldRoundItToTwoDigits()
		{
			var vector = new Vector(10.003f, 20.005f, 30.006f);
			vector.Round();
			Assert.That(vector.X, Is.EqualTo(10));
			Assert.That(vector.Y, Is.EqualTo(20));
			Assert.That(vector.Z, Is.EqualTo(30.01f));
		}

		[Test]
		public void RoundingToSpecifcDigitsShouldRountCorrectly()
		{
			var vector = new Vector(10.0031f, 20.0053f, 30.0066f);
			vector.Round(3);
			Assert.That(vector.X, Is.EqualTo(10.003f));
			Assert.That(vector.Y, Is.EqualTo(20.005f));
			Assert.That(vector.Z, Is.EqualTo(30.007f));
		}
	}
}