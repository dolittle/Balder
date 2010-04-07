#region License

//
// Author: Einar Ingebrigtsen <einar@dolittle.com>
// Copyright (c) 2007-2009, DoLittle Studios
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

using Balder.Core.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Balder.Core.IntegrationTests.Silverlight
{
	public class MyPropertyObject : PropertyObject
	{

		public bool ValueGet;
		public bool ValueSet;

		public static readonly Property<MyPropertyObject, int> ValueProperty =
			Property<MyPropertyObject, int>.Register(m => m.Value);
		public int Value
		{
			get
			{
				ValueGet = true;
				return ValueProperty.GetValue(this);
			}
			set
			{
				ValueSet = true;
				ValueProperty.SetValue(this,value);
			}
		}
		
	}


	[TestClass]
	public class PropertyObjectTests
	{
		
	}
}

