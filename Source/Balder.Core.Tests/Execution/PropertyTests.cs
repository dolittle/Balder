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
using Balder.Core.Materials;
using Balder.Core.Math;
using Balder.Core.Objects.Geometries;
using CThru.Silverlight;
using NUnit.Framework;
#if(SILVERLIGHT)
using System.Windows;
#endif

namespace Balder.Core.Tests.Execution
{
	[TestFixture]
	public class PropertyTests
	{
		public class SomeOtherClass
		{

		}


#if(SILVERLIGHT)
		public class SomeClass : FrameworkElement,
#else
		public class SomeClass : 
#endif
 ICanNotifyChanges
		{
			public const int IntDefault = 42;
			public const float FloatDefault = 42.42f;
			public const string StringDefault = "42";

			public static readonly Property<SomeClass, int> IntProperty = Property<SomeClass, int>.Register(s => s.Int);
			public int Int
			{
				get { return IntProperty.GetValue(this); }
				set { IntProperty.SetValue(this, value); }
			}

			public static readonly Property<SomeClass, float> FloatProperty = Property<SomeClass, float>.Register(s => s.Float);
			public float Float
			{
				get { return FloatProperty.GetValue(this); }
				set { FloatProperty.SetValue(this, value); }
			}

			public static readonly Property<SomeClass, string> StringProperty = Property<SomeClass, string>.Register(s => s.String);
			public string String
			{
				get { return StringProperty.GetValue(this); }
				set { StringProperty.SetValue(this, value); }
			}

			public static readonly Property<SomeClass, int> IntWithDefaultProperty = Property<SomeClass, int>.Register(s => s.IntWithDefault, IntDefault);
			public int IntWithDefault
			{
				get { return IntWithDefaultProperty.GetValue(this); }
				set { IntWithDefaultProperty.SetValue(this, value); }
			}

			public static readonly Property<SomeClass, float> FloatWithDefaultProperty = Property<SomeClass, float>.Register(s => s.FloatWithDefault, FloatDefault);
			public float FloatWithDefault
			{
				get { return FloatWithDefaultProperty.GetValue(this); }
				set { FloatWithDefaultProperty.SetValue(this, value); }
			}

			public static readonly Property<SomeClass, string> StringWithDefaultProperty = Property<SomeClass, string>.Register(s => s.StringWithDefault, StringDefault);
			public string StringWithDefault
			{
				get { return StringWithDefaultProperty.GetValue(this); }
				set { StringWithDefaultProperty.SetValue(this, value); }
			}

			public static readonly Property<SomeClass, SomeOtherClass> SomeOtherClassProperty =
				Property<SomeClass, SomeOtherClass>.Register(s => s.SomeOtherClass);

			public SomeOtherClass SomeOtherClass
			{
				get { return SomeOtherClassProperty.GetValue(this); }
				set { SomeOtherClassProperty.SetValue(this, value); }
			}



			public object OldValue;
			public object NewValue;
			public string PropertyName;
			public bool WasNotified;

			public void Notify(string propertyName, object oldValue, object newValue)
			{
				WasNotified = true;
				PropertyName = propertyName;
				OldValue = oldValue;
				NewValue = newValue;
			}
		}


		[Test, SilverlightUnitTest]
		public void SettingIntValueShouldReturnSameValue()
		{
			var instance = new SomeClass();
			var expected = 42;

			instance.Int = expected;
			var actual = instance.Int;

			Assert.That(actual, Is.EqualTo(expected));
		}

		[Test, SilverlightUnitTest]
		public void SettingFloatValueShouldReturnSameValue()
		{
			var instance = new SomeClass();
			var expected = 42.42f;

			instance.Float = expected;
			var actual = instance.Float;

			Assert.That(actual, Is.EqualTo(expected));
		}

		[Test, SilverlightUnitTest]
		public void SettingStringValueShouldReturnSameValue()
		{
			var instance = new SomeClass();
			var expected = "42";

			instance.String = expected;
			var actual = instance.String;

			Assert.That(actual, Is.EqualTo(expected));
		}

		[Test, SilverlightUnitTest]
		public void IntPropertyWithDefaultValueShouldHaveItsDefaultValueSetWhenObjectIsInstantiated()
		{
			var instance = new SomeClass();
			Assert.That(instance.IntWithDefault, Is.EqualTo(SomeClass.IntDefault));
		}

		[Test, SilverlightUnitTest]
		public void FloatPropertyWithDefaultValueShouldHaveItsDefaultValueSetWhenObjectIsInstantiated()
		{
			var instance = new SomeClass();
			Assert.That(instance.FloatWithDefault, Is.EqualTo(SomeClass.FloatDefault));
		}

		[Test, SilverlightUnitTest]
		public void StringPropertyWithDefaultValueShouldHaveItsDefaultValueSetWhenObjectIsInstantiated()
		{
			var instance = new SomeClass();
			Assert.That(instance.StringWithDefault, Is.EqualTo(SomeClass.StringDefault));
		}

		[Test, SilverlightUnitTest]
		public void SettingValueShouldCallNotifyIfNotificationInterfaceIsImplemented()
		{
			var instance = new SomeClass();
			instance.Int = 42;
			Assert.That(instance.WasNotified, Is.True);
		}

		[Test, SilverlightUnitTest]
		public void SettingValueWhenNoValueHasBeenSetOrDefaultValueExistForStringShouldHaveOldValueBeNullDuringNotify()
		{
			var instance = new SomeClass();
			instance.String = "Something";
			Assert.That(instance.OldValue, Is.Null);
		}

		[Test, SilverlightUnitTest]
		public void SettingValueShouldCallNotifyWithCorrectNameOfProperty()
		{
			var instance = new SomeClass();
			instance.String = "Something";
			Assert.That(instance.PropertyName, Is.EqualTo("String"));
		}

		[Test, SilverlightUnitTest]
		public void SettingValueShouldCallNotifyWithNewValue()
		{
			var instance = new SomeClass();
			var expected = "Something";
			instance.String = expected;
			Assert.That(instance.NewValue, Is.EqualTo(expected));
		}

		[Test, SilverlightUnitTest]
		public void SettingCustomRefTypeShouldReturnSameInstanceWhenGetting()
		{
			var instance = new SomeClass();
			var expected = new SomeOtherClass();
			instance.SomeOtherClass = expected;
			var actual = instance.SomeOtherClass;
			Assert.That(actual,Is.EqualTo(expected));
		}
	}
}