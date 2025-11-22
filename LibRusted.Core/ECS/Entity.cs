// Entity.cs

using System;
using System.Collections.Generic;
using LibRusted.Core.ECS.Components;

namespace LibRusted.Core.ECS;

public class Entity(string name)
{
	private static ulong _nextId;
	public ulong Id { get; } = _nextId++;
	public string Name { get; set; } = name;
	public bool Enabled { get; set; } = true;
	//public ulong ComponentMask { get; private set; }
    
	private readonly Dictionary<Type, IComponent> _components = new();

	//private readonly static Dictionary<Type, ulong> _componentMasks = new();
	private static ulong _nextComponentBit = 1;

	public T? GetComponent<T>() where T : IComponent
	{
		_components.TryGetValue(typeof(T), out var component);
		return (T?)component;
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
		var type = typeof(T);
		//if (!_componentMasks.TryGetValue(type, out var bitmask))
		//{
		//	bitmask = _nextComponentBit;
		//	_componentMasks[type] = bitmask;
		//	_nextComponentBit <<= 1;
		//}
        
		_components[type] = component;
		//ComponentMask |= bitmask;
	}

	public void RemoveComponent<T>() where T : IComponent
	{
		var type = typeof(T);
		//if (_componentMasks.TryGetValue(type, out var bitmask))
		//{
		//	ComponentMask &= ~bitmask;
		//}
		_components.Remove(type);
	}

	public IEnumerable<IComponent> GetAllComponents()
	{
		return _components.Values;
	}

	//public static ulong GetComponentMask<T>() where T : IComponent
	//{
	//	_componentMasks.TryGetValue(typeof(T), out var mask);
	//	return mask;
	//}

	//public static ulong GetComponentMask(Type componentType)
	//{
	//	_componentMasks.TryGetValue(componentType, out var mask);
	//	return mask;
	//}
	
	public IEnumerable<Type> GetAllComponentTypes() => _components.Keys;
}