using Microsoft.Xna.Framework;
namespace LibRusted.Core.ECS.Systems;

public interface IUpdatableSystem
{
	void Update(GameTime gameTime);
}