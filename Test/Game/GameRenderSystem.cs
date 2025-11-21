using LibRusted.Core.ECS.Systems;
using LibRusted.Core.ECS.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Test.Game.Components;
namespace Test.Game;

public class GameRenderSystem(GraphicsDevice graphicsDevice) : SystemBase, IDrawableSystem
{
    private SpriteBatch _spriteBatch;
    private EntityQuery _query;
    private Texture2D _pixel;

    protected override void Initialize()
    {
        _spriteBatch = new SpriteBatch(graphicsDevice);
        _query = new EntityQuery(World);
        _pixel = new Texture2D(graphicsDevice, 1, 1);
        _pixel.SetData([Color.White]);
    }

    public void Draw(GameTime gameTime)
    {
        graphicsDevice.Clear(Color.Black);
        _spriteBatch.Begin();

        _query.Process<FoodComponent, ColorComponent>((transform, sprite) =>
        {
            if (!sprite.Enabled) return;
            _spriteBatch.Draw(
                _pixel,
                transform.Position,
                null,
                sprite.Color,
                0f,
                Vector2.Zero,
                new Vector2(18, 18),
                SpriteEffects.None,
                0f
            );
        });
        _query.Process<SnakeComponent, ColorComponent>((transform, sprite) =>
        {
            if (!sprite.Enabled) return;
            _spriteBatch.Draw(
                _pixel,
                transform.Position,
                null,
                sprite.Color,
                0f,
                Vector2.Zero,
                new Vector2(18, 18),
                SpriteEffects.None,
                0f
            );
        });

        _spriteBatch.End();
    }
}