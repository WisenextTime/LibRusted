using System;
using LibRusted.Core;
using LibRusted.Core.ECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Test.Game.Components;
namespace Test.Game;

public class GameWorldBuilder
{
    
    public static World Get()
    {
        var world =
            (new WorldBuilder() >>
             new GameControlSystem() >>
             new CollisionSystem() >>
             new SnakeMovementSystem() >>
             new GameRenderSystem(RustedGame.GameInstance.GraphicsDevice) >>
             new UiRenderSystem(RustedGame.GameInstance.GraphicsDevice, Program.Font)
            ).Build();
	    return world;
    }
}