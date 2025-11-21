using System;
using LibRusted.Core.ECS.Systems;

namespace LibRusted.Core.ECS;

public class WorldBuilder
{
	private World _world = new();
	public WorldBuilder WithSystems<T>(T system)where T : ISystem { _world.SystemManager.Add(system); return this; }
	public WorldBuilder BindChangeAction(Action<WorldChangeArguments> action)
	{
		_world.OnWorldChange += action;
		return this;
	}
	public World Build() => _world;

	public static WorldBuilder operator >> (WorldBuilder builder, ISystem system) => builder.WithSystems(system);

	public static WorldBuilder operator + (WorldBuilder builder, Action<WorldChangeArguments> action) =>
		builder.BindChangeAction(action);

	public void operator += (Action<WorldChangeArguments> action) => BindChangeAction(action);
}