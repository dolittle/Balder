using Microsoft.Windows.Design.Model;

namespace Balder.Design
{
	public class GameInitializer : DefaultInitializer
	{
		public override void InitializeDefaults(ModelItem item)
		{
			base.InitializeDefaults(item);

			item.Properties[GameType.WidthProperty].SetValue("640");
			item.Properties[GameType.HeightProperty].SetValue("480");
		}
	}
}