using LibRusted.Core.ECS.Components;
using Microsoft.Xna.Framework;
namespace Test.Game.Components;

public class SnakeComponent() : IComponent
{
	public bool Enabled = true;
	public bool IsHead;
	public int Index;
	public Vector2 Position;
	public Vector2 PreviousPosition;
}

public class DirectionComponent() : IComponent
{
	public bool Enabled = true;
	public Vector2 Direction;
	public Vector2 NextDirection;
}

public class FoodComponent() : IComponent
{
	public bool Enabled = true;
	public bool IsEaten;
	public Vector2 Position;
}

public class GameInfoComponent()  : IComponent
{
	public bool Enabled = true;
	public int Score;
	public bool IsGameOver;
	public float Timer;
	public float MoveDelay = 0.15f;
	public bool IsPaused;
}

public class ColorComponent : IComponent
{
	public Color Color = Color.White;
	public bool Enabled { get; set; } =  true;
}