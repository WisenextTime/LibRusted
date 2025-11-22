using LibRusted.Core;
using LibRusted.Core.ECS.Systems;
using LibRusted.Core.ECS.Utils;
using LibRusted.World2D.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace LibRusted.World2D.Systems;

public class Sprite2DRenderSystem : SystemBase, IDrawableSystem
{
	private GraphicsDevice _graphicsDevice;
	private SpriteBatch _spriteBatch = null!;
	public Camera2D Camera;
	private EntityQuery _query;
	protected override void Initialize()
	{
		_graphicsDevice = RustedGame.GameInstance.GraphicsDevice;
		_spriteBatch = new SpriteBatch(_graphicsDevice);
		Camera = new Camera2D(_graphicsDevice.Viewport);
		_query = new EntityQuery(World);
	}
	public void Draw(GameTime gameTime)
	{
		_spriteBatch.Begin(transformMatrix: Camera.ViewMatrix);
		_query.Process<Transform2DComponent, Sprite2DComponent>((transform, sprite) =>
		{
			if(sprite is null)return;
			if (!sprite.IsVisible) return;
			_spriteBatch.Draw(
				sprite.Texture,
				transform.Position + sprite.Offset,
				sprite.TextureRect,
				sprite.Color,
				transform.Rotation + sprite.Rotation,
				transform.Origin + sprite.Origin,
				transform.Scale * sprite.Scale,
				sprite.SpriteEffects,
				sprite.ZIndex
			);
		});
	}
}