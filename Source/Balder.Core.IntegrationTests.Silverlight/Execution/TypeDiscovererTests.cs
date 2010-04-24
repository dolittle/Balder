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
using Balder.Core.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Balder.Core.Tests.Execution
{
	[TestClass]
	public class TypeDiscovererTests
	{
		public interface IBaseInterface
		{
			
		}

		public class TypeImplementingBaseInterface : IBaseInterface
		{
			
		}

		public interface IInterfaceWithoutImplementors
		{
			
		}

		public interface IInterfaceForMultipleImplementors
		{
			
		}
		public class FirstImplementation : IInterfaceForMultipleImplementors
		{
			
		}

		public class SecondImplementation : IInterfaceForMultipleImplementors
		{
			
		}

		public interface IInterfaceImplementedByAbstractClass
		{
			
		}

		public abstract class AbstractClass : IInterfaceImplementedByAbstractClass
		{
			
		}


		[TestMethod]
		public void GettingSingleWithOnlyOnePresentShouldReturnSingle()
		{
			var typeDiscoverer = new TypeDiscoverer();
			var type = typeDiscoverer.FindSingle<IBaseInterface>();
			Assert.IsNotNull(type);
			Assert.AreEqual(type,typeof(TypeImplementingBaseInterface));
		}

		[TestMethod]
		public void GettingSingleWithoutAnyImplementorsShouldReturnNull()
		{
			var typeDiscoverer = new TypeDiscoverer();
			var type = typeDiscoverer.FindSingle<IInterfaceWithoutImplementors>();
			Assert.IsNull(type);
		}

		[TestMethod, ExpectedException(typeof(ArgumentException))]
		public void GettingSingleWithMultiplePresentShouldCauseException()
		{
			var typeDiscoverer = new TypeDiscoverer();
			typeDiscoverer.FindSingle<IInterfaceForMultipleImplementors>();
		}

		[TestMethod]
		public void GettingMultipleWithMultiplePresentShouldReturnAll()
		{
			var typeDiscoverer = new TypeDiscoverer();
			var types = typeDiscoverer.FindMultiple<IInterfaceForMultipleImplementors>();
			Assert.AreEqual(types.Length,2);
		}

		[TestMethod]
		public void GettingInterfaceImplementedByAnAbstractClassShouldNotReturnTheAbstractClass()
		{
			var typeDiscoverer = new TypeDiscoverer();
			var type = typeDiscoverer.FindSingle<IInterfaceImplementedByAbstractClass>();
			Assert.IsNull(type);
		}
	}
}

