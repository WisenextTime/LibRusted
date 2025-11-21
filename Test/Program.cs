using LibRusted.Core;
using Microsoft.Xna.Framework.Graphics;
using Test.Game;
namespace Test;

public class Program
{
	public static SpriteFont Font = null!;
	static void Main(string[] args)
	{
		RustedGame.GameInstance.InitAction += Init;
		RustedGame.GameInstance.ChangeWindowSize(800, 600, false);
		RustedGame.GameInstance.Run();
	}

	static void Init()
	{
		Font = RustedGame.GameInstance.Content.Load<SpriteFont>("Font");
		RustedGame.GameInstance.ChangeWorld(GameWorldBuilder.Get());
	}
}