
using Balder.Math;
using Balder.View;
using Ninject;

namespace Balder.Silverlight.SampleBrowser
{
	public class ViewModels
	{
		public ViewModels()
		{
			App.Kernel.Inject(this);
		}

		[Inject]
		public Features.Resources.ViewModel Resources { get; set; }

		[Inject]
		public Samples.Materials.Editor.ViewModel MaterialEditor { get; set; }
	}
}
