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
	private EntityQuery _query;
	protected override void Initialize()
	{
		_graphicsDevice = RustedGame.GameInstance.GraphicsDevice;
		_spriteBatch = new SpriteBatch(_graphicsDevice);
		_query = new EntityQuery(World);
	}
	public virtual void Draw(GameTime gameTime) { }

	protected void RenderSprites<T1, T2>(Matrix drawMatrix = default) where T1 : Transform2DComponent where T2 : Sprite2DComponent
	{
		_spriteBatch.Begin(transformMatrix: drawMatrix);
		_query.Process<T1, T2>((transform, sprite) =>
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