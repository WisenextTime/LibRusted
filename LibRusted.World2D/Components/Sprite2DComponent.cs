using LibRusted.Core.ECS.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace LibRusted.World2D.Components;

public class Sprite2DComponent(Texture2D texture) : IComponent
{
	public Texture2D Texture = texture;
	public Rectangle? TextureRect;
	public Color Color = Color.White;
	public float Rotation = 0;
	public Vector2 Origin = Vector2.Zero;
	public Vector2 Scale =  Vector2.One;
	public Vector2 Offset =  Vector2.Zero;
	public int ZIndex = 0;
	public SpriteEffects SpriteEffects = SpriteEffects.None;
	public bool IsVisible { get; set; } = true;
}