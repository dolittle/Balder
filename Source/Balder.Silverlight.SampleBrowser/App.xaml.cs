﻿using System;
using System.Windows;
using System.Windows.Navigation;
using Balder.Execution;
using Balder.Notification;
using Ninject;
using Ninject.Activation;
using Ninject.Planning.Bindings;

namespace Balder.Silverlight.SampleBrowser
{
	public partial class App
	{
		public static IKernel Kernel;
		private static readonly NotifyingObjectWeaver Weaver;

		static App()
		{
			Weaver = new NotifyingObjectWeaver();
			
			var dispatcher = new Dispatcher(Deployment.Current.Dispatcher);
			DispatcherManager.Current = dispatcher;
			
			var autoKernel = new AutoKernel();
			autoKernel.AddGenericBindingResolver(ResolveViewModel);
			Kernel = autoKernel;
		}

		private static bool ResolveViewModel(Type type, IRequest request)
		{
			if( type.Name.Equals("ViewModel"))
			{
				var proxy = Weaver.GetProxyType(type);
				request.ParentContext.Kernel.Bind(type).To(proxy);
				return true;
			}
			return false;
		}

		public App()
		{
			Startup += Application_Startup;
			Exit += Application_Exit;
			UnhandledException += Application_UnhandledException;

			InitializeComponent();
		}

		public UriMapper	GetUriMapper()
		{
			var uriMapper = Resources["uriMapper"] as UriMapper;
			return uriMapper;
		}

		private void Application_Startup(object sender, StartupEventArgs e)
		{
			
			RootVisual = new MainPage();
		}

		private void Application_Exit(object sender, EventArgs e)
		{

		}



		private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
		{
			if (!System.Diagnostics.Debugger.IsAttached)
			{
				e.Handled = true;
				Deployment.Current.Dispatcher.BeginInvoke(delegate { ReportErrorToDOM(e); });
			}
		}
		private void ReportErrorToDOM(ApplicationUnhandledExceptionEventArgs e)
		{
			try
			{
				string errorMsg = e.ExceptionObject.Message + e.ExceptionObject.StackTrace;
				errorMsg = errorMsg.Replace('"', '\'').Replace("\r\n", @"\n");

				System.Windows.Browser.HtmlPage.Window.Eval("throw new Error(\"Unhandled Error in Silverlight Application " + errorMsg + "\");");
			}
			catch (Exception)
			{
			}
		}
	}
}
