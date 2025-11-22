// Entity.cs

using System;
using System.Collections.Generic;
using LibRusted.Core.ECS.Components;
using LibRusted.Core.Pool;

namespace LibRusted.Core.ECS;

public class Entity : IPoolable
{
	private static ulong _nextId;
	public ulong Id { get; } = _nextId++;
	public bool Enabled { get; set; } = true;
    
	private readonly Dictionary<Type, IComponent> _components = new();

	public T? GetComponent<T>() where T : IComponent
	{
		return (T?)_components.GetValueOrDefault(typeof(T));
	}

	public bool HasComponent<T>() where T : IComponent
	{
		return _components.ContainsKey(typeof(T));
	}

	public bool HasComponent(Type componentType)
	{
		return _components.ContainsKey(componentType);
	}

	public void AddComponent<T>(T component) where T : IComponent
	{
		var type = typeof(T);_components[type] = component;
	}

	public void RemoveComponent<T>() where T : IComponent
	{
		var type = typeof(T);
		_components.Remove(type);
	}

	public IEnumerable<IComponent> GetAllComponents()
	{
		return _components.Values;
	}
	
	public IEnumerable<Type> GetAllComponentTypes() => _components.Keys;
	public void Reset()
	{
		_components.Clear();
	}
}