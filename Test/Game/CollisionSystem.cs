using System;
using System.Linq;
using LibRusted.Core.ECS.Systems;
using LibRusted.Core.ECS.Utils;
using Microsoft.Xna.Framework;
using Test.Game.Components;
namespace Test.Game;

public class CollisionSystem : SystemBase, IUpdatableSystem
{
    private EntityQuery _query;
    private readonly int _gridSize = 20;

    protected override void Initialize()
    {
        _query = new EntityQuery(World);
    }

    public void Update(GameTime gameTime)
    {
        var gameStateEntities = World.GetEntities<GameInfoComponent>();
        if (gameStateEntities.Count == 0) return;

        var gameState = gameStateEntities[0].GetComponent<GameInfoComponent>();
        if (gameState.IsGameOver) return;

        CheckFoodCollision();
        CheckSelfCollision();
    }

    private void CheckFoodCollision()
    {
        var snakeHeads = _query.WithComponent<SnakeComponent>()
            .Where(x => x.comp1.IsHead);
        var foods = _query.WithComponent<FoodComponent>();

        foreach (var head in snakeHeads)
        {
            foreach (var food in foods)
            {
                if (!(Vector2.Distance(head.comp1.Position, food.comp1.Position) < _gridSize)) continue;
                food.comp1.IsEaten = true;
                GrowSnake();
                SpawnFood();
                IncreaseScore();
            }
        }
    }

    private void CheckSelfCollision()
    {
        var snakeHeads = _query.WithComponent<SnakeComponent>()
            .Where(x => x.comp1.IsHead);
        var snakeBodies = _query.WithComponent<SnakeComponent>()
            .Where(x => !x.comp1.IsHead);

        if (snakeHeads.Any(head =>
                snakeBodies.Any(body => Vector2.Distance(head.comp1.Position, body.comp1.Position) == 0f)))
            GameOver();

    }

    private void GrowSnake()
    {
        var snakeSegments = _query.WithComponent<SnakeComponent>()
            .OrderByDescending(x => x.comp1.Index)
            .ToList();

        if (snakeSegments.Count == 0) return;

        var lastSegment = snakeSegments[0];
        var newPosition = lastSegment.comp1.PreviousPosition;

        CreateSnakeSegment(newPosition, lastSegment.comp1.Index + 1, false);
    }

    private void CreateSnakeSegment(Vector2 position, int index, bool isHead)
    {
        var segment = World.CreateEntity($"SnakeSegment_{index}");
        segment.AddComponent(new ColorComponent { Color = isHead ? Color.Green : Color.LightGreen });
        segment.AddComponent(new SnakeComponent 
        { 
            IsHead = isHead, 
            Index = index,
            PreviousPosition = position,
            Position = position
        });

        if (isHead)
        {
            segment.AddComponent(new DirectionComponent 
            { 
                Direction = Vector2.UnitX,
                NextDirection = Vector2.UnitX
            });
        }
    }

    private void SpawnFood()
    {
        var eatenFoods = _query.WithComponent<FoodComponent>()
            .Where(x => x.comp1.IsEaten);
        foreach (var (entity, _) in eatenFoods)
        {
            World.QueueRemoveEntity(entity);
        }

        var random = Random.Shared;
        var foodEntity = World.CreateEntity("Food");
        foodEntity.AddComponent(new FoodComponent()
        { 
            Position = new Vector2(
                random.Next(0, 800 / 20) * 20,
                random.Next(0, 600 / 20) * 20
            ),
        });
        foodEntity.AddComponent(new ColorComponent { Color = Color.Red });
    }

    private void IncreaseScore()
    {
        var gameState = World.GetEntities<GameInfoComponent>().FirstOrDefault();
        if (gameState != null)
        {
            var state = gameState.GetComponent<GameInfoComponent>();
            state.Score += 10;
        }
    }

    private void GameOver()
    {
        var gameState = World.GetEntities<GameInfoComponent>().FirstOrDefault();
        if (gameState == null) return;
        var state = gameState.GetComponent<GameInfoComponent>();
        state.IsGameOver = true;
    }
}