// World.cs

using System;
using System.Collections.Generic;
using LibRusted.Core.ECS.Components;
using Microsoft.Xna.Framework;

namespace LibRusted.Core.ECS;

public class World : IAvailable
{
    private readonly List<Entity> _entities = [];
    private readonly SystemManager _systemManager;
    private readonly Dictionary<Type, List<Entity>> _componentIndex = new();
    private readonly Dictionary<ulong, List<Entity>> _maskCache = new();
    private bool _isDirty = true;

    private List<Entity> QueuedRemoveEntities = [];

    public Action<WorldChangeArguments>? OnWorldChange;
    internal World()
    {
        _systemManager = new SystemManager(this);
    }

    public SystemManager SystemManager => _systemManager;

    public Entity CreateEntity(string name = "Entity")
    {
        var entity = new Entity(name);
        _entities.Add(entity);
        _isDirty = true;
        return entity;
    }

    public void QueueRemoveEntity(Entity entity)
    {
        QueuedRemoveEntities.Add(entity);
    }

    private void RemoveEntity(Entity entity)
    {
        _entities.Remove(entity);
        _isDirty = true;
        foreach (var index in _componentIndex.Values)
        {
            index.Remove(entity);
        }
        
        _maskCache.Clear();
    }

    public void OnEntityChanged(Entity entity)
    {
        _isDirty = true;
    }

    public List<Entity> GetEntities()
    {
        return _entities;
    }

    public List<Entity> GetEntities<T>() where T : IComponent
    {
        var componentType = typeof(T);

        if (_componentIndex.TryGetValue(componentType, out var entityList)) return entityList;
        entityList = [];
        _componentIndex[componentType] = entityList;
            
        RefreshIndexes();

        return GetEntities<T>();
    }

    public List<Entity> GetEntities<T1, T2>() where T1 : IComponent where T2 : IComponent
    {
        var mask = Entity.GetComponentMask<T1>() | Entity.GetComponentMask<T2>();
        return GetEntitiesByMask(mask);
    }

    public List<Entity> GetEntities<T1, T2, T3>() 
        where T1 : IComponent where T2 : IComponent where T3 : IComponent
    {
        var mask = Entity.GetComponentMask<T1>() | Entity.GetComponentMask<T2>() | Entity.GetComponentMask<T3>();
        return GetEntitiesByMask(mask);
    }

    private List<Entity> GetEntitiesByMask(ulong mask)
    {
        if (_maskCache.TryGetValue(mask, out var entities)) return entities;
        entities = [];
        foreach (var entity in _entities)
        {
            if ((entity.ComponentMask & mask) == mask)
                entities.Add(entity);
        }
        _maskCache[mask] = entities;

        return entities;
    }
    
    public void RefreshIndexes()
    {
        if (!_isDirty) return;
        _componentIndex.Clear();
        _maskCache.Clear();
        foreach (var entity in _entities)
        {
            if (!entity.Enabled) continue;
            
            foreach (var componentType in entity.GetAllComponentTypes())
            {
                if (!_componentIndex.TryGetValue(componentType, out var entityList))
                {
                    entityList = [];
                    _componentIndex[componentType] = entityList;
                }
                
                if (!entityList.Contains(entity))
                    entityList.Add(entity);
            }
        }
        
        _isDirty = false;
    }
    public void Update(GameTime gameTime)
    {
        if(_isDirty)RefreshIndexes();
        _systemManager.Update(gameTime);
        foreach (var removeEntity in QueuedRemoveEntities)
        {
            RemoveEntity(removeEntity);
        }
    }

    public void Draw(GameTime gameTime)
    {
        _systemManager.Draw(gameTime);
    }
    public bool Available { get; private set; }
    public void Ready()
    {
        Available = true;
        SystemManager.Ready();
    }

    public void Clear()
    {
        _entities.Clear();
        _isDirty = true;
    }
}