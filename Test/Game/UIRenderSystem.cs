using LibRusted.Core.ECS;
using LibRusted.Core.ECS.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Test.Game.Components;
namespace Test.Game;

public class UiRenderSystem(GraphicsDevice graphicsDevice, SpriteFont font) : SystemBase, IDrawableSystem
{

	private SpriteBatch _spriteBatch = null!;
	protected override void Initialize()
	{
		_spriteBatch = new SpriteBatch(graphicsDevice);
	}
	public void Update(GameTime gameTime) { }

	public void Draw(GameTime gameTime)
	{
		var gameStateEntities = World.GetEntities<GameInfoComponent>();
		if (gameStateEntities.Count == 0) return;

		var gameState = gameStateEntities[0].GetComponent<GameInfoComponent>();

		_spriteBatch.Begin();

		_spriteBatch.DrawString(font, $"Score: {gameState.Score}", new Vector2(10, 10), Color.White);

		if (gameState.IsGameOver)
		{
			var text = "Game Over! Press R to restart";
			var textSize = font.MeasureString(text);
			var position = new Vector2(400 - textSize.X / 2, 300 - textSize.Y / 2);
			_spriteBatch.DrawString(font, text, position, Color.Red);
		}
		else if (gameState.IsPaused)
		{
			const string text = "Paused - Press SPACE to continue";
			var textSize = font.MeasureString(text);
			var position = new Vector2(400 - textSize.X / 2, 300 - textSize.Y / 2);
			_spriteBatch.DrawString(font, text, position, Color.Yellow);
		}

		_spriteBatch.End();
	}
}