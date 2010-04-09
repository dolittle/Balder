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

using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Balder.Core.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Balder.Core.IntegrationTests.Silverlight.Execution
{
	public class ControlStub : Control, INotifyPropertyChanged, ICanNotifyChanges
	{
		public event PropertyChangedEventHandler PropertyChanged = (s, e) => { };

		public static readonly Property<ControlStub, string> SomeStringProperty =
			Property<ControlStub, string>.Register(c => c.SomeString);
		public string SomeString
		{
			get { return SomeStringProperty.GetValue(this); }
			set { SomeStringProperty.SetValue(this, value); }
		}

		public const string DefaultValue = "DefaultValue";

		public static readonly Property<ControlStub, string> SomeStringWithDefaultProperty =
			Property<ControlStub, string>.Register(c => c.SomeStringWithDefault, DefaultValue);
		public string SomeStringWithDefault
		{
			get { return SomeStringWithDefaultProperty.GetValue(this); }
			set { SomeStringWithDefaultProperty.SetValue(this, value); }
		}

		public object OldValue;
		public object NewValue;

		public void Notify(string propertyName, object oldValue, object newValue)
		{
			OldValue = oldValue;
			NewValue = newValue;
			PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}

	public class MyPropertyObject : FrameworkElement
	{

		public bool ValueGet;
		public int ValueSetCount = 0;

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
				ValueSetCount++;
				ValueProperty.SetValue(this, value);
			}
		}
	}

	public class MyBindableObject : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged = (s, e) => { };

		private int _someValue;

		public int SomeValue
		{
			get { return _someValue; }
			set
			{
				_someValue = value;
				PropertyChanged(this, new PropertyChangedEventArgs("SomeValue"));
			}
		}
	}

	[TestClass]
	public class PropertyTests
	{
		[TestMethod]
		public void SettingValueShouldSetDependencyProperty()
		{
			var expected = 5;
			var obj = new MyPropertyObject();
			obj.Value = expected;
			var actual = (int)obj.GetValue(MyPropertyObject.ValueProperty.ActualDependencyProperty);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void SettingValueShouldReturnSameValueWhenGetting()
		{
			var expected = 5;
			var obj = new MyPropertyObject();
			obj.Value = expected;
			var actual = obj.Value;
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void BindingPropertyShouldCauseSetToBeCalled()
		{
			var boundObject = new MyBindableObject();
			boundObject.SomeValue = 5;
			var obj = new MyPropertyObject();

			var binding = new Binding();
			binding.Source = boundObject;
			binding.Path = new PropertyPath("SomeValue");
			binding.Mode = BindingMode.TwoWay;

			obj.SetBinding(MyPropertyObject.ValueProperty.ActualDependencyProperty, binding);

			Assert.AreEqual(1,obj.ValueSetCount);
		}

		[TestMethod]
		public void ChangingValueInBoundObjectShouldCauseSetToBeCalled()
		{
			var boundObject = new MyBindableObject();
			boundObject.SomeValue = 5;
			var obj = new MyPropertyObject();

			var binding = new Binding();
			binding.Source = boundObject;
			binding.Path = new PropertyPath("SomeValue");
			binding.Mode = BindingMode.TwoWay;

			obj.SetBinding(MyPropertyObject.ValueProperty.ActualDependencyProperty, binding);

			boundObject.SomeValue = 76;
			Assert.AreEqual(2,obj.ValueSetCount);
		}

		[TestMethod]
		public void SettingValueShouldSetItInDependencyObjectPropertyBag()
		{
			var control = new ControlStub();
			var expected = "Something";
			ControlStub.SomeStringProperty.SetValue(control, expected);

			var actual = control.GetValue(ControlStub.SomeStringProperty.ActualDependencyProperty);
			Assert.AreEqual(expected,actual);
		}

		[TestMethod]
		public void SettingValueShouldFirePropertyChangedEvent()
		{
			var control = new ControlStub();
			var eventFired = false;
			control.PropertyChanged += (s, e) => eventFired = true;

			ControlStub.SomeStringProperty.SetValue(control, "Something");
			Assert.IsTrue(eventFired);
		}

		[TestMethod]
		public void SettingSameValueTwiceShouldFirePropertyChangedEventOnce()
		{
			var control = new ControlStub();
			var eventFireCount = 0;
			control.PropertyChanged += (s, e) => eventFireCount++;

			var valueToSet = "Something";
			ControlStub.SomeStringProperty.SetValue(control, valueToSet);
			ControlStub.SomeStringProperty.SetValue(control, valueToSet);
			Assert.AreEqual(1,eventFireCount);
		}

		[TestMethod]
		public void SettingValueToNullAfterIthasBeenSetToSomethingElseShouldFirePropertyChangedEvent()
		{
			var control = new ControlStub();
			var eventFireCount = 0;

			var valueToSet = "Something";
			ControlStub.SomeStringProperty.SetValue(control, valueToSet);
			control.PropertyChanged += (s, e) => eventFireCount++;
			ControlStub.SomeStringProperty.SetValue(control, null);
			Assert.AreEqual(1,eventFireCount);
		}

		[TestMethod]
		public void GettingValueAfterSettingShouldReturnSameValue()
		{
			var control = new ControlStub();
			var expected = "Something";
			ControlStub.SomeStringProperty.SetValue(control, expected);
			var actual = ControlStub.SomeStringProperty.GetValue(control);
			Assert.AreEqual(expected,actual);
		}

		[TestMethod]
		public void SettingValueInOtherThreadShouldNotCauseException()
		{
			var control = new ControlStub();
			var expected = "Something";
			var waitEvent = new ManualResetEvent(false);
			var failed = false;
			var thread = new Thread(() =>
			{
				try
				{
					ControlStub.SomeStringProperty.SetValue(control, expected);
				}
				catch
				{
					failed = true;

				}
				waitEvent.Set();
			});
			thread.Start();

			waitEvent.WaitOne(500);

			if (failed)
			{
				Assert.Fail();
			}
		}


		[TestMethod]
		public void SettingValueInOtherThreadShouldSetValue()
		{
			var control = new ControlStub();
			var expected = "Something";
			var waitEvent = new ManualResetEvent(false);
			var thread = new Thread(() =>
			{
				try
				{
					ControlStub.SomeStringProperty.SetValue(control, expected);
				}
				catch
				{
				}
				waitEvent.Set();
			});
			thread.Start();

			waitEvent.WaitOne(500);
			var actual = ControlStub.SomeStringProperty.GetValue(control);
			Assert.AreEqual(expected,actual);
		}

		[TestMethod]
		public void ChangingValueShouldCauseNotifyToBeCalledWithOldAndNewValue()
		{
			var control = new ControlStub();
			var firstValue = "First";
			var secondValue = "Second";

			control.SomeString = firstValue;

			control.OldValue = string.Empty;
			control.NewValue = string.Empty;

			control.SomeString = secondValue;

			Assert.AreEqual(firstValue, control.OldValue);
			Assert.AreEqual(secondValue, control.NewValue);
		}

		[TestMethod]
		public void GettingValueWithoutValueBeingSetAndDefaultDefinedShouldReturnDefaultValue()
		{
			var control = new ControlStub();
			var actual = control.SomeStringWithDefault;
			Assert.AreEqual(ControlStub.DefaultValue,actual);
		}

		[TestMethod]
		public void SettingValueThroughDependencyPropertyShouldOnlyCallSetOnceForProperty()
		{
			var obj = new MyPropertyObject();
			
			obj.SetValue(MyPropertyObject.ValueProperty.ActualDependencyProperty,5);
			Assert.AreEqual(1,obj.ValueSetCount);
		}

		[TestMethod]
		public void SettingValueThroughDependencyPropertyShouldReturnSameValueWhenGetting()
		{
			var obj = new MyPropertyObject();
			var expected = 5;
			obj.SetValue(MyPropertyObject.ValueProperty.ActualDependencyProperty,expected);
			var actual = obj.Value;
			Assert.AreEqual(expected,actual);
		}

		[TestMethod]
		public void SettingValueShouldOnlyCallSetOnPropertyOnce()
		{
			var obj = new MyPropertyObject();
			obj.Value = 5;
			Assert.AreEqual(1,obj.ValueSetCount);
		}
	}
}