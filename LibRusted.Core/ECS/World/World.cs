using System;
using System.Collections.Generic;
using System.Linq;
using LibRusted.Core.ECS.Components;
using Microsoft.Xna.Framework;

namespace LibRusted.Core.ECS;

public class World : IAvailable
{
    private readonly List<Entity> _entities = [];
    private readonly SystemManager _systemManager;
    private readonly Dictionary<Type, List<Entity>> _componentIndex = new();
    //private readonly Dictionary<ulong, List<Entity>> _maskCache = new();
    private bool _isDirty = true;

    private readonly List<Entity> _queuedRemoveEntities = [];
    private readonly List<Entity> _queuedAddedEntities = [];

    private static IEnumerable<Type> AllComponentTypes => field??= GetAllComponentTypes();

    public Action<WorldChangeArguments>? OnWorldChange;
    internal World()
    {
        _systemManager = new SystemManager(this);
    }

    public SystemManager SystemManager => _systemManager;

    public Entity CreateEntity(string name = "Entity")
    {
        var entity = new Entity(name);
        _queuedAddedEntities.Add(entity);
        return entity;
    }

    public void QueueRemoveEntity(Entity entity)
    {
        _queuedRemoveEntities.Add(entity);
    }

    private void RemoveEntities()
    {
        if(_queuedRemoveEntities.Count > 0) _isDirty =  true;
        foreach (var entity in _queuedRemoveEntities)
        {
            _entities.Remove(entity);
        }
        _queuedRemoveEntities.Clear();
    }

    private void AddEntities()
    {
        if(_queuedAddedEntities.Count > 0) _isDirty =  true;
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

    public List<Entity> GetEntities(params Type[] types)
    {
        List<Entity> entitiesList = [];
        foreach (var type in types)
        {
            if (!_componentIndex.TryGetValue(type, out var entities)) entities = [];
            if(entitiesList.Count == 0)entitiesList = entities;
            else entitiesList = entitiesList.Intersect(entities).ToList();
        }
        return entitiesList;
    }

    //private List<Entity> GetEntitiesByMask(ulong mask)
    //{
    //    if (_maskCache.TryGetValue(mask, out var entities)) return entities;
    //    entities = [];
    //    foreach (var entity in _entities)
    //    {
    //        if ((entity.ComponentMask & mask) == mask)
    //            entities.Add(entity);
    //    }
    //    _maskCache[mask] = entities;
    //    return entities;
    //}
    
    public void RefreshIndexes()
    {
        if (!_isDirty) return;
        _componentIndex.Clear();
        //_maskCache.Clear();
        foreach (var componentType in AllComponentTypes)
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
    private static IEnumerable<Type> GetAllComponentTypes()
    {
        var interfaceType = typeof(IComponent);
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        var implementingTypes = assemblies
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => interfaceType.IsAssignableFrom(type) && type is { IsInterface: false, IsAbstract: false });
        return implementingTypes;
    }
    public void Update(GameTime gameTime)
    {
        RefreshIndexes();
        _systemManager.Update(gameTime);
        RemoveEntities();
        AddEntities();
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