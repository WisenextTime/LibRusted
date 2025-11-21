using System.Linq;
using LibRusted.Core.ECS;
using LibRusted.Core.ECS.Systems;
using LibRusted.Core.ECS.Utils;
using Microsoft.Xna.Framework;
using Test.Game.Components;
namespace Test.Game;

public class SnakeMovementSystem : SystemBase, IUpdatableSystem
{
	private EntityQuery _query;
	private readonly int _gridSize = 20;
	private readonly int _screenWidth = 800;
	private readonly int _screenHeight = 600;

	protected override void Initialize()
	{
		_query = new EntityQuery(World);
	}
	public void Update(GameTime gameTime)
	{
		var gameStateEntities = World.GetEntities<GameInfoComponent>();
		if (gameStateEntities.Count == 0) return;

		var gameState = gameStateEntities[0].GetComponent<GameInfoComponent>();
        
		if (gameState.IsGameOver || gameState.IsPaused) return;

		gameState.Timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

		if (!(gameState.Timer >= gameState.MoveDelay)) return;
		gameState.Timer = 0;
		MoveSnake();
	}

	private void MoveSnake()
	{
		var snakeSegments = _query.WithComponent<SnakeComponent>()
			.OrderBy(x => x.comp1.Index)
			.ToList();

		if (snakeSegments.Count == 0) return;

		for (var i = snakeSegments.Count - 1; i > 0; i--)
		{
			var currentSegment = snakeSegments[i];
			var previousSegment = snakeSegments[i - 1];
            
			currentSegment.comp1.Position = previousSegment.comp1.Position;
			currentSegment.comp1.PreviousPosition = previousSegment.comp1.Position;
		}

		var head = snakeSegments[0];
		var directionComp = head.entity.GetComponent<DirectionComponent>();
		
		directionComp.Direction = directionComp.NextDirection;
		
		head.comp1.PreviousPosition = head.comp1.Position;
		head.comp1.Position += directionComp.Direction * _gridSize;
            
		HandleBoundary(head.entity);
        
	}

	private void HandleBoundary(Entity head)
	{
		var transform = head.GetComponent<SnakeComponent>();
        
		if (transform.Position.X < 0) transform.Position = new Vector2(_screenWidth - _gridSize, transform.Position.Y);
		if (transform.Position.X >= _screenWidth) transform.Position = new Vector2(0, transform.Position.Y);
		if (transform.Position.Y < 0) transform.Position = new Vector2(transform.Position.X, _screenHeight - _gridSize);
		if (transform.Position.Y >= _screenHeight) transform.Position = new Vector2(transform.Position.X, 0);
	}
}