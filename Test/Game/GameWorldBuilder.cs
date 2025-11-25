using System;
using LibRusted.Core;
using LibRusted.Core.ECS;
using LibRusted.Core.ECS.World;
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
             new SnakeMovementSystem() >>
             new CollisionSystem() >>
             new GameRenderSystem() >>
             new UiRenderSystem(Program.Font)
            ).Build();
	    return world;
    }
}