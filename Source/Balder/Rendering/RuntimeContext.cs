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

using Balder.Display;
using Balder.Execution;

namespace Balder.Rendering
{
	[Singleton]
	public class RuntimeContext : IRuntimeContext
	{
		private static readonly PassiveRenderingSignal	RenderingSignal = new PassiveRenderingSignal();
		public RuntimeContext(IDisplayDevice displayDevice, IMessengerContext messengerContext)
		{
			MessengerContext = messengerContext;
			Display = displayDevice.CreateDisplay(this);
		}

		public IMessengerContext MessengerContext { get; private set; }
		public IDisplay Display { get; private set; }

		private bool _passiveRendering;
		public bool PassiveRendering
		{
			get
			{
				if (Runtime.Instance.Platform.IsInDesignMode)
				{
					return true;
				}
				return _passiveRendering;
			}
			set { _passiveRendering = value; }
		}

		public PassiveRenderingMode PassiveRenderingMode { get; set; }
		public bool Paused { get; set; }

		public void SignalRendering()
		{
			// TODO : DesignMode - probably need a better way to handle this
 			// don't want to ask the platform if its in design mode every time a property changes
			// I guess I'll notice this when I actually start supporting the designer.. :)
			if( _passiveRendering )
			{
				MessengerContext.Send(RenderingSignal);	
			}
		}
	}
}
