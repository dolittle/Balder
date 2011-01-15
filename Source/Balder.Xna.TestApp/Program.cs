using Balder.Execution;
using Balder.Execution.Xna;

namespace Balder.Xna.TestApp
{
	class Program
	{
		static void Main(string[] args)
		{
			Display.Xna.Display.Initialize();
			var game = Runtime.Instance.CreateGame<MyGame>();
			((Platform) Runtime.Instance.Platform).Start();
			Runtime.Instance.RegisterGame(game.Display, game);



		}
	}
}
