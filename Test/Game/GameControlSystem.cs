using System;
using System.Linq;
using LibRusted.Core.ECS;
using LibRusted.Core.ECS.Systems;
using LibRusted.Core.ECS.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Test.Game.Components;
namespace Test.Game;

public class GameControlSystem : SystemBase, IUpdatableSystem
{
	protected override void Initialize()
    {
        _query = new EntityQuery(World);
        InitializeGame();
    }
    private EntityQuery _query = null!;
    private KeyboardState _previousKeyboardState;
	public void Update(GameTime gameTime)
    {
        var keyboardState = Keyboard.GetState();
        
        var snakeHeads = _query.WithComponents<SnakeComponent, DirectionComponent>()
            .Where(x => x.comp1.IsHead);
        
        foreach (var (_, _, direction) in snakeHeads)
        {
            if (keyboardState.IsKeyDown(Keys.Up) && direction.Direction != Vector2.UnitY)
                direction.NextDirection = -Vector2.UnitY;
            else if (keyboardState.IsKeyDown(Keys.Down) && direction.Direction != -Vector2.UnitY)
                direction.NextDirection = Vector2.UnitY;
            else if (keyboardState.IsKeyDown(Keys.Left) && direction.Direction != Vector2.UnitX)
                direction.NextDirection = -Vector2.UnitX;
            else if (keyboardState.IsKeyDown(Keys.Right) && direction.Direction != -Vector2.UnitX)
                direction.NextDirection = Vector2.UnitX;
            else if (keyboardState.IsKeyDown(Keys.Space) && !_previousKeyboardState.IsKeyDown(Keys.Space))
                TogglePause();
        }
        if (Keyboard.GetState().IsKeyDown(Keys.R))
        {
            RestartGame();
        }
        _previousKeyboardState = keyboardState;
    }

    private void TogglePause()
    {
        var gameState = World.GetEntities<GameInfoComponent>().FirstOrDefault();
        if (gameState == null) return;
        var state = gameState.GetComponent<GameInfoComponent>();
        state.IsPaused = !state.IsPaused;
    }
    
    private void RestartGame()
    {
        World.Clear();
        InitializeGame();
    }
    private void InitializeGame()
    {
        var gameState = World.CreateEntity("GameState");
        gameState.AddComponent(new GameInfoComponent());
        InitializeSnake();
        SpawnInitialFood();
    }

    private void InitializeSnake()
    {
        var head = World.CreateEntity("SnakeHead");
        head.AddComponent(new SnakeComponent { Position = new Vector2(400, 300) });
        head.AddComponent(new ColorComponent { Color = Color.Green });
        head.AddComponent(new SnakeComponent { IsHead = true, Index = 0 });
        head.AddComponent(
            new DirectionComponent
            {
                Direction = Vector2.UnitX,
                NextDirection = Vector2.UnitX
            });

        for (var i = 1; i < 3; i++)
        {
            var segment = World.CreateEntity($"SnakeSegment_{i}");
            segment.AddComponent(new SnakeComponent { Position = new Vector2(400 - i * 20, 300) });
            segment.AddComponent(new ColorComponent { Color = Color.LightGreen });
            segment.AddComponent(new SnakeComponent { IsHead = false, Index = i });
        }
    }

    private void SpawnInitialFood()
    {
        var random = new Random();
        var food = World.CreateEntity("Food");
        food.AddComponent(
            new FoodComponent
            {
                Position = new Vector2(
                    random.Next(0, 800 / 20) * 20,
                    random.Next(0, 600 / 20) * 20
                )
            });
        food.AddComponent(new ColorComponent { Color = Color.Red });
        food.AddComponent(new FoodComponent());
    }
}