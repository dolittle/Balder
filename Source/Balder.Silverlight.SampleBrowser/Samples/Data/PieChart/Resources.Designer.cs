﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Balder.Silverlight.SampleBrowser.Samples.Data.PieChart {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Balder.Silverlight.SampleBrowser.Samples.Data.PieChart.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to using System.ComponentModel;
        ///
        ///namespace Balder.Silverlight.SampleBrowser.Samples.Data.PieChart
        ///{
        ///	public class BusinessObject : INotifyPropertyChanged
        ///	{
        ///		public event PropertyChangedEventHandler PropertyChanged = (s, e) =&gt; { };
        ///
        ///		private double _value;
        ///		public double Value
        ///		{
        ///			get { return _value; }
        ///			set
        ///			{
        ///				_value = value;
        ///				OnPropertyChanged(&quot;Value&quot;);
        ///			}
        ///		}
        ///
        ///		private void OnPropertyChanged(string property)
        ///		{
        ///			PropertyChanged(this, new PropertyChangedEventArgs(p [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string BusinessObject_cs {
            get {
                return ResourceManager.GetString("BusinessObject_cs", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;UserControl x:Class=&quot;Balder.Silverlight.SampleBrowser.Samples.Data.PieChart.Content&quot;
        ///    xmlns=&quot;http://schemas.microsoft.com/winfx/2006/xaml/presentation&quot; 
        ///    xmlns:x=&quot;http://schemas.microsoft.com/winfx/2006/xaml&quot; 
        ///    xmlns:Execution=&quot;clr-namespace:Balder.Execution;assembly=Balder&quot; 
        ///    xmlns:View=&quot;clr-namespace:Balder.View;assembly=Balder&quot; 
        ///    xmlns:Lighting=&quot;clr-namespace:Balder.Lighting;assembly=Balder&quot; 
        ///    xmlns:local=&quot;clr-namespace:Balder.Silverlight.SampleBrowser.Samples.Data.PieChart&quot; 
        ///   [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Content_xaml {
            get {
                return ResourceManager.GetString("Content_xaml", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to using System;
        ///
        ///namespace Balder.Silverlight.SampleBrowser.Samples.Data.PieChart
        ///{
        ///	public partial class Content
        ///	{
        ///		private static readonly Random Rnd = new Random();
        ///
        ///		public Content()
        ///		{
        ///			InitializeComponent();
        ///		}
        ///
        ///		private ViewModel ViewModel
        ///		{
        ///			get
        ///			{
        ///				return (ViewModel)DataContext;
        ///			}
        ///		}
        ///
        ///		private void AddValueClick(object sender, System.Windows.RoutedEventArgs e)
        ///		{
        ///			var value = new BusinessObject { Value = Rnd.Next(0, 100) };
        ///			ViewModel.Objects.Add(val [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Content_xaml_cs {
            get {
                return ResourceManager.GetString("Content_xaml_cs", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;Geometries:Geometry x:Class=&quot;Balder.Silverlight.SampleBrowser.Samples.Data.PieChart.PieChart&quot;
        ///    xmlns=&quot;http://schemas.microsoft.com/winfx/2006/xaml/presentation&quot; 
        ///    xmlns:x=&quot;http://schemas.microsoft.com/winfx/2006/xaml&quot; 
        ///    xmlns:Geometries=&quot;clr-namespace:Balder.Objects.Geometries;assembly=Balder&quot; 
        ///    xmlns:Controls=&quot;clr-namespace:Balder.Controls;assembly=Balder&quot; xmlns:Materials=&quot;clr-namespace:Balder.Materials;assembly=Balder&quot;&gt;
        ///	&lt;Controls:NodesControl x:Name=&quot;NodesControl&quot;&gt; &lt;!-- UniqueNodes=&quot;Tru [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string PieChart_xaml {
            get {
                return ResourceManager.GetString("PieChart_xaml", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to using System.Collections;
        ///using System.Collections.Generic;
        ///using System.Collections.ObjectModel;
        ///using System.Collections.Specialized;
        ///using System.ComponentModel;
        ///using System.Globalization;
        ///using Balder.Silverlight.Helpers;
        ///
        ///namespace Balder.Silverlight.SampleBrowser.Samples.Data.PieChart
        ///{
        ///	public partial class PieChart
        ///	{
        ///		private Dictionary&lt;object, PieChartValue&gt; _hashedValues;
        ///		private ObservableCollection&lt;PieChartValue&gt; _values;
        ///
        ///		public PieChart()
        ///		{
        ///			InitializeComponent();
        /// [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string PieChart_xaml_cs {
            get {
                return ResourceManager.GetString("PieChart_xaml_cs", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to using System.ComponentModel;
        ///
        ///namespace Balder.Silverlight.SampleBrowser.Samples.Data.PieChart
        ///{
        ///	public class PieChartValue : INotifyPropertyChanged
        ///	{
        ///		private double _startAngle;
        ///		public double StartAngle
        ///		{
        ///			get { return _startAngle; }
        ///			set
        ///			{
        ///				_startAngle = value;
        ///				OnPropertyChanged(&quot;StartAngle&quot;);
        ///			}
        ///		}
        ///
        ///		private double _endAngle;
        ///		
        ///
        ///		public double EndAngle
        ///		{
        ///			get { return _endAngle; }
        ///			set
        ///			{
        ///				_endAngle = value;
        ///				OnPropertyChanged(&quot;EndAngle&quot; [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string PieChartValue_cs {
            get {
                return ResourceManager.GetString("PieChartValue_cs", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to using System;
        ///using System.Collections.ObjectModel;
        ///
        ///namespace Balder.Silverlight.SampleBrowser.Samples.Data.PieChart
        ///{
        ///	public class ViewModel
        ///	{
        ///		private static readonly Random Rnd = new Random();
        ///		public ViewModel()
        ///		{
        ///			Objects = new ObservableCollection&lt;BusinessObject&gt;();
        ///
        ///			Objects.Add(new BusinessObject { Value = Rnd.Next(0, 100) });
        ///			Objects.Add(new BusinessObject { Value = Rnd.Next(0, 100) });
        ///			Objects.Add(new BusinessObject { Value = Rnd.Next(0, 100) });
        ///		}
        ///
        ///		public Obs [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string ViewModel_cs {
            get {
                return ResourceManager.GetString("ViewModel_cs", resourceCulture);
            }
        }
    }
}
