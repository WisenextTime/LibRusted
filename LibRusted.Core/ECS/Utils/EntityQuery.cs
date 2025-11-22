using System;
using System.Collections.Generic;
using System.Linq;
using LibRusted.Core.ECS.Components;
namespace LibRusted.Core.ECS.Utils;

public class EntityQuery(World world)
{

    public IEnumerable<Entity> GetEntities<T1>() where T1 : IComponent
    {
        return world.GetEntities(typeof(T1));
    }

    public IEnumerable<Entity> GetEntities<T1, T2>() where T1 : IComponent where T2 : IComponent
    {
        return world.GetEntities(typeof(T1), typeof(T2));
    }

    public List<Entity> GetEntities<T1, T2, T3>() 
        where T1 : IComponent where T2 : IComponent where T3 : IComponent
    {
        return world.GetEntities(typeof(T1), typeof(T2), typeof(T3));
    }

    public IEnumerable<(Entity entity, T1? comp1)> WithComponent<T1>() where T1 : IComponent
    {
        return world.GetEntities(typeof(T1)).Select(entity => (entity, entity.GetComponent<T1>()));
    }

    public IEnumerable<(Entity entity, T1? comp1, T2? comp2)> WithComponents<T1, T2>() 
        where T1 : IComponent where T2 : IComponent
    {
        return world.GetEntities(typeof(T1),typeof(T2)).Select(entity => (entity, entity.GetComponent<T1>(), entity.GetComponent<T2>()));
    }

    public IEnumerable<(Entity entity, T1? comp1, T2? comp2, T3? comp3)> WithComponents<T1, T2, T3>() 
        where T1 : IComponent where T2 : IComponent where T3 : IComponent
    {
        return world.GetEntities(typeof(T1),typeof(T2),typeof(T3)).Select(entity =>
            (entity, entity.GetComponent<T1>(), entity.GetComponent<T2>(), entity.GetComponent<T3>()));
    }

    public void Process<T1>(Action<T1?> process) where T1 : IComponent
    {
        var entities = world.GetEntities(typeof(T1));
        foreach (var entity in entities)
        {
            process(entity.GetComponent<T1>());
        }
    }

    public void Process<T1, T2>(Action<T1?, T2?> process) 
        where T1 : IComponent where T2 : IComponent
    {
        var entities = world.GetEntities(typeof(T1), typeof(T2));
        foreach (var entity in entities)
        {
            process(entity.GetComponent<T1>(), entity.GetComponent<T2>());
        }
    }

    public void Process<T1, T2, T3>(Action<T1?, T2?, T3?> process) 
        where T1 : IComponent where T2 : IComponent where T3 : IComponent
    {
        var entities = world.GetEntities(typeof(T1), typeof(T2), typeof(T3));
        foreach (var entity in entities)
        {
            process(entity.GetComponent<T1>(), entity.GetComponent<T2>(), entity.GetComponent<T3>());
        }
    }

    public IEnumerable<(Entity entity, T1 comp1)> Where<T1>(Func<T1, bool> predicate) 
        where T1 : IComponent
    {
        foreach (var entity in world.GetEntities(typeof(T1)))
        {
            var comp = entity.GetComponent<T1>();
            if (comp != null && predicate(comp))
                yield return (entity, comp);
        }
    }

    public IEnumerable<(Entity entity, T1 comp1, T2 comp2)> Where<T1, T2>(
        Func<T1, T2, bool> predicate) 
        where T1 : IComponent where T2 : IComponent
    {
        foreach (var entity in world.GetEntities(typeof(T1), typeof(T2)))
        {
            var comp1 = entity.GetComponent<T1>();
            var comp2 = entity.GetComponent<T2>();
            if (comp2 != null && comp1 != null && predicate(comp1, comp2))
                yield return (entity, comp1, comp2);
        }
    }

    public int Count<T1>() where T1 : IComponent
    {
        return world.GetEntities(typeof(T1)).Count;
    }

    public int Count<T1, T2>() where T1 : IComponent where T2 : IComponent
    {
        return world.GetEntities(typeof(T1), typeof(T2)).Count;
    }

    public bool Any<T1>() where T1 : IComponent
    {
        return world.GetEntities(typeof(T1)).Count > 0;
    }

    public bool Any<T1>(Func<T1?, bool> predicate) where T1 : IComponent
    {
        return world.GetEntities(typeof(T1)).Any(entity => predicate(entity.GetComponent<T1>()));
    }

}