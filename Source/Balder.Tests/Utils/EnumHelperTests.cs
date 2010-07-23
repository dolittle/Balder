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

using Balder.Core.Utils;
using NUnit.Framework;

namespace Balder.Core.Tests.Utils
{
	[TestFixture]
	public class EnumHelperTests
	{
		public enum SomeEnum
		{
			SomeValue=1,
			SomeOtherValue
		}

		[Test]
		public void GettingValuesGenericallyShouldReturnAllTheValues()
		{
			var values = EnumHelper.GetValues<SomeEnum>();

			Assert.That(values.Length,Is.EqualTo(2));

			Assert.That(values[0], Is.EqualTo(SomeEnum.SomeValue));
			Assert.That(values[1], Is.EqualTo(SomeEnum.SomeOtherValue));
		}

		[Test]
		public void GettingValuesNonGenericallyShouldReturnAllTheValues()
		{
			var values = EnumHelper.GetValues(typeof (SomeEnum));

			Assert.That(values.Length, Is.EqualTo(2));
			Assert.That(values[0], Is.EqualTo(SomeEnum.SomeValue));
			Assert.That(values[1], Is.EqualTo(SomeEnum.SomeOtherValue));
		}
	}
}