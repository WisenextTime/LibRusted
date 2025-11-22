using System.Collections.Generic;
using System.Linq;
using LibRusted.Core.ECS.Systems;
using Microsoft.Xna.Framework;

namespace LibRusted.Core.ECS;

public class SystemManager(World world) : IAvailable
{
	private readonly List<ISystem> _systems = [];
	private IEnumerable<ISystem> _orderedSystems = [];

	public SystemManager Add(ISystem system)
	{
		_systems.Add(system);
		system.BeAdded(world);
		_orderedSystems = _systems.OrderBy(s => s.Priority);
		return this;
	}

	public T? GetSystem<T>() where T : ISystem
	{
		return _systems.OfType<T>().FirstOrDefault();
	}
	public void Update(GameTime gameTime)
	{
		foreach (var system in _orderedSystems.Where(s => s.Enabled)) (system as IUpdatableSystem)?.Update(gameTime);
	}
	public void Draw(GameTime gameTime)
	{
		foreach (var system in _orderedSystems.Where(s => s.Enabled))  (system as IDrawableSystem)?.Draw(gameTime);
	}
	public bool Available { get; private set; }
	public void Ready()
	{
		Available = true;
		foreach (var system in _systems) system.Ready();
	}
}