﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace Balder.WP7.SampleBrowser
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

		protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
		{
			RubicsCube.GoBack();
			
			base.OnBackKeyPress(e);
		}
    }
}
