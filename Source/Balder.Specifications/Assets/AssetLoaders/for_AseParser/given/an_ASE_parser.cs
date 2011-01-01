using Balder.Assets.AssetLoaders;
using Machine.Specifications;

namespace Balder.Specifications.Assets.AssetLoaders.for_AseParser.given
{
	public class an_ase_parser
	{
		protected static AseParser Parser;
		Establish context = () => Parser = new AseParser();
	}
}
