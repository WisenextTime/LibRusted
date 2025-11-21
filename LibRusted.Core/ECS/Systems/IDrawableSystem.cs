using Microsoft.Xna.Framework;
namespace LibRusted.Core.ECS.Systems;

public interface IDrawableSystem
{
	void Draw(GameTime gameTime);
}