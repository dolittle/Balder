using Microsoft.Windows.Design.Metadata;

namespace Balder.Design
{
	public class GameType
	{
		public static readonly TypeIdentifier TypeId = new TypeIdentifier("Balder.Game");
		public static readonly PropertyIdentifier WidthProperty = new PropertyIdentifier(TypeId, "Width");
		public static readonly PropertyIdentifier HeightProperty = new PropertyIdentifier(TypeId, "Height");
	}
}