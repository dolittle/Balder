﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Balder.Silverlight.SampleBrowser.Samples.Data.HierarchicalNodesControl {
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Balder.Silverlight.SampleBrowser.Samples.Data.HierarchicalNodesControl.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to #region License
        ///
        /////
        ///// Author: Einar Ingebrigtsen &lt;einar@dolittle.com&gt;
        ///// Copyright (c) 2007-2010, DoLittle Studios
        /////
        ///// Licensed under the Microsoft Permissive License (Ms-PL), Version 1.1 (the &quot;License&quot;)
        ///// you may not use this file except in compliance with the License.
        ///// You may obtain a copy of the license at 
        /////
        /////   http://balder.codeplex.com/license
        /////
        ///// Unless required by applicable law or agreed to in writing, software
        ///// distributed under the License is distributed on an &quot;AS IS&quot; B [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Column_cs {
            get {
                return ResourceManager.GetString("Column_cs", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;UserControl x:Class=&quot;Balder.Silverlight.SampleBrowser.Samples.Data.HierarchicalNodesControl.Content&quot;
        ///    xmlns=&quot;http://schemas.microsoft.com/winfx/2006/xaml/presentation&quot; 
        ///    xmlns:x=&quot;http://schemas.microsoft.com/winfx/2006/xaml&quot; 
        ///    xmlns:Execution=&quot;clr-namespace:Balder.Execution;assembly=Balder&quot; 
        ///    xmlns:Controls=&quot;clr-namespace:Balder.Controls;assembly=Balder&quot; 
        ///	xmlns:local=&quot;clr-namespace:Balder.Silverlight.SampleBrowser.Samples.Data.HierarchicalNodesControl&quot; 
        ///	xmlns:Geometries=&quot;clr-namespace:B [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Content_xaml {
            get {
                return ResourceManager.GetString("Content_xaml", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to using System;
        ///using System.Windows.Controls;
        ///using Balder.Input;
        ///using Balder.Objects.Geometries;
        ///
        ///namespace Balder.Silverlight.SampleBrowser.Samples.Data.HierarchicalNodesControl
        ///{
        ///	public partial class Content : UserControl
        ///	{
        ///		public Content()
        ///		{
        ///			InitializeComponent();
        ///		}
        ///
        ///		private void RotationSlider_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs&lt;double&gt; e)
        ///		{
        ///			Nodes.Rotation.Y = e.NewValue;
        ///
        ///		}
        ///
        ///		private void Box_MouseEnter(object sender, Mouse [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Content_xaml_cs {
            get {
                return ResourceManager.GetString("Content_xaml_cs", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to #region License
        ///
        /////
        ///// Author: Einar Ingebrigtsen &lt;einar@dolittle.com&gt;
        ///// Copyright (c) 2007-2010, DoLittle Studios
        /////
        ///// Licensed under the Microsoft Permissive License (Ms-PL), Version 1.1 (the &quot;License&quot;)
        ///// you may not use this file except in compliance with the License.
        ///// You may obtain a copy of the license at 
        /////
        /////   http://balder.codeplex.com/license
        /////
        ///// Unless required by applicable law or agreed to in writing, software
        ///// distributed under the License is distributed on an &quot;AS IS&quot; B [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Depth_cs {
            get {
                return ResourceManager.GetString("Depth_cs", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to using System;
        ///using System.Globalization;
        ///using System.Windows;
        ///using System.Windows.Data;
        ///using System.Windows.Media;
        ///
        ///namespace Balder.Silverlight.SampleBrowser.Samples.Data.HierarchicalNodesControl
        ///{
        ///	public class MatrixToStringValueConverter : IValueConverter
        ///	{
        ///		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        ///		{
        ///			return value.ToString();
        ///		}
        ///
        ///		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        /// [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string MatrixToStringValueConverter_cs {
            get {
                return ResourceManager.GetString("MatrixToStringValueConverter_cs", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to #region License
        ///
        /////
        ///// Author: Einar Ingebrigtsen &lt;einar@dolittle.com&gt;
        ///// Copyright (c) 2007-2010, DoLittle Studios
        /////
        ///// Licensed under the Microsoft Permissive License (Ms-PL), Version 1.1 (the &quot;License&quot;)
        ///// you may not use this file except in compliance with the License.
        ///// You may obtain a copy of the license at 
        /////
        /////   http://balder.codeplex.com/license
        /////
        ///// Unless required by applicable law or agreed to in writing, software
        ///// distributed under the License is distributed on an &quot;AS IS&quot; B [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Row_cs {
            get {
                return ResourceManager.GetString("Row_cs", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to #region License
        ///
        /////
        ///// Author: Einar Ingebrigtsen &lt;einar@dolittle.com&gt;
        ///// Copyright (c) 2007-2010, DoLittle Studios
        /////
        ///// Licensed under the Microsoft Permissive License (Ms-PL), Version 1.1 (the &quot;License&quot;)
        ///// you may not use this file except in compliance with the License.
        ///// You may obtain a copy of the license at 
        /////
        /////   http://balder.codeplex.com/license
        /////
        ///// Unless required by applicable law or agreed to in writing, software
        ///// distributed under the License is distributed on an &quot;AS IS&quot; B [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string ViewModel_cs {
            get {
                return ResourceManager.GetString("ViewModel_cs", resourceCulture);
            }
        }
    }
}
