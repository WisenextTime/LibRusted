using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
namespace LibRusted.Core.ECS.World;

public class World : IAvailable
{
    private readonly List<Entity> _entities = [];
    private readonly SystemManager _systemManager;
    private readonly Dictionary<Type, List<Entity>> _componentIndex = new();
    private bool _isDirty = true;

    private readonly List<Entity> _queuedRemoveEntities = [];
    private readonly List<Entity> _queuedAddedEntities = [];

    public Action<WorldChangeArguments>? OnWorldChange;
    internal World()
    {
        _systemManager = new SystemManager(this);
    }

    public SystemManager SystemManager => _systemManager;

    public Entity CreateEntity()
    {
        var entity = RustedGame.GameInstance.CreatEntity();
        _queuedAddedEntities.Add(entity);
        return entity;
    }

    public void QueueRemoveEntity(Entity entity)
    {
        _queuedRemoveEntities.Add(entity);
    }

    private void RemoveEntities()
    {
        if (_queuedRemoveEntities.Count <= 0) return;
        _isDirty = true;
        foreach (var entity in _queuedRemoveEntities)
        {
            _entities.Remove(entity);
            RustedGame.GameInstance.ReturnEntity(entity);
        }
        _queuedRemoveEntities.Clear();
    }

    private void AddEntities()
    {
        if (_queuedAddedEntities.Count <= 0) return;
        _isDirty = true;
        foreach (var entity in _queuedAddedEntities)
        {
            _entities.Add(entity);
        }
        _queuedAddedEntities.Clear();
    }

    public List<Entity> GetEntities()
    {
        return _entities;
    }

    public Entity? GetEntity(ulong id)
    {
        return _entities.FirstOrDefault(e => e.Id == id);
    }

    public List<Entity> GetEntities(params Type[] types)
    {
        List<Entity> entitiesList = [];
        foreach (var type in types)
        {
            if (!_componentIndex.TryGetValue(type, out var entities)) entities = [];
            entitiesList = entitiesList.Count == 0 ? entities : entitiesList.Intersect(entities).ToList();
        }
        return entitiesList;
    }
    
    public void RefreshIndexes()
    {
        if (!_isDirty) return;
        _componentIndex.Clear();
        foreach (var componentType in RustedGame.AllComponentTypes)
        {
            List<Entity>entityList = [];
            
            _componentIndex[componentType] = entityList;

            foreach (var entity in _entities.Where(entity => entity.Enabled)
                         .Where(entity => !entityList.Contains(entity) && entity.HasComponent(componentType)))
            {
                entityList.Add(entity);
            }
        }
        _isDirty = false;
    }
    
    public void Update(GameTime gameTime)
    {
        RefreshIndexes();
        _systemManager.Update(gameTime);
        AddEntities();
        RemoveEntities();
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
        foreach (var entity in _entities)
        {
            _queuedAddedEntities.Add(entity);
        }
        RemoveEntities();
        _isDirty = true;
    }
}