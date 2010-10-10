#if(IOS)
namespace Balder.Execution.iOS
{
	public class Platform : IPlatform
	{
		#region IPlatform implementation
		public event PlatformStateChange BeforeStateChange;

		public event PlatformStateChange StateChanged;

		public string PlatformName {
			get {
				throw new System.NotImplementedException ();
			}
		}

		public string EntryAssemblyName {
			get {
				throw new System.NotImplementedException ();
			}
		}

		public bool IsInDesignMode {
			get {
				throw new System.NotImplementedException ();
			}
		}

		public Display.IDisplayDevice DisplayDevice {
			get {
				throw new System.NotImplementedException ();
			}
		}

		public Input.IMouseDevice MouseDevice {
			get {
				throw new System.NotImplementedException ();
			}
		}

		public System.Type DefaultFileLoaderType {
			get {
				throw new System.NotImplementedException ();
			}
		}

		public System.Type GeometryContextType {
			get {
				throw new System.NotImplementedException ();
			}
		}

		public System.Type SpriteContextType {
			get {
				throw new System.NotImplementedException ();
			}
		}

		public System.Type ImageContextType {
			get {
				throw new System.NotImplementedException ();
			}
		}

		public System.Type ShapeContextType {
			get {
				throw new System.NotImplementedException ();
			}
		}

		public System.Type MaterialCalculatorType {
			get {
				throw new System.NotImplementedException ();
			}
		}

		public System.Type SkyboxContextType {
			get {
				throw new System.NotImplementedException ();
			}
		}

		public PlatformState CurrentState {
			get {
				throw new System.NotImplementedException ();
			}
		}
		#endregion
	}
}

#endif