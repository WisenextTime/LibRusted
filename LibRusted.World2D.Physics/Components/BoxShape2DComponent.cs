using LibRusted.Core.ECS.Components;
namespace LibRusted.World2D.Physics.Components;

public class BoxShape2DComponent(int left = 0, int top = 0, int right = 0, int bottom = 0) : IComponent
{
	public int Left = left;
	public int Top = top;
	public int Right = right;
	public int Bottom = bottom;

	public int Width => Right - Left;
	public int Height => Bottom - Top;
} 