using LibRusted.Core.ECS.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace LibRusted.World2D.Components;

public class Sprite2DComponent : IComponent
{
	public Texture2D Texture;
	public Rectangle? TextureRect;
	public Color Color = Color.White;
	public float Rotation;
	public Vector2 Origin;
	public Vector2 Scale;
	public Vector2 Offset;
	public int ZIndex;
	public SpriteEffects SpriteEffects = SpriteEffects.None;
	public bool IsVisible { get; set; }
}