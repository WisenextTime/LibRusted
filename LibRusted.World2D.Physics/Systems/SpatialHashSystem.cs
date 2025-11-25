using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using LibRusted.Core.ECS;
using LibRusted.Core.ECS.Systems;
using LibRusted.Core.ECS.Utils;
using LibRusted.World2D.Components;
using LibRusted.World2D.Physics.Components;
using Microsoft.Xna.Framework;
namespace LibRusted.World2D.Physics.Systems;

public class SpatialHashSystem(int cellSize) : SystemBase, IUpdatableSystem
{
	private readonly Dictionary<ulong, EntitySpatialHashCache>
		_entityCache = [];
	private readonly Dictionary<Point, List<ulong>> _hash = [];
	private EntityQuery _query = null!;

	protected override void Initialize()
	{
		_query = new EntityQuery(World);
		var entities = _query.WithComponents<Transform2DComponent, BoxShape2DComponent>();
		foreach (var entity in entities) 
			AddToHash(entity.comp1, entity.comp2, entity.entity);
		
	}
	public void Update(GameTime gameTime)
	{
		var entities = _query.WithComponents<Transform2DComponent, BoxShape2DComponent>();
		foreach (var entity in entities) 
			if(!_entityCache.ContainsKey(entity.entity.Id)) AddToHash(entity.comp1, entity.comp2, entity.entity);
		
	}
	private void AddToHash(Transform2DComponent? transform, BoxShape2DComponent? shape, Entity entity)
	{
		if (transform is null || shape is null) return;

		transform.Locked = true;
		var posX = transform.Position.X;
		var posY = transform.Position.Y;

		var sourceMinX = (int)(posX + shape.Left);
		var sourceMaxX = (int)(posX + shape.Right);
		var sourceMinY = (int)(posY + shape.Bottom);
		var sourceMaxY = (int)(posY + shape.Top);

		var minX = sourceMinX / cellSize;
		var maxX = sourceMaxX / cellSize;
		var minY = sourceMinY / cellSize;
		var maxY = sourceMaxY / cellSize;

		var width = maxX - minX + 1;
		var height = maxY - minY + 1;
		var totalCells = width * height;
		var points = totalCells <= 64
			? stackalloc Point[totalCells]
			: new Point[totalCells];

		var index = 0;
		for (var x = minX; x <= maxX; x++)
		{
			for (var y = minY; y <= maxY; y++)
			{
				points[index++] = new Point(x, y);

				ref var entityList = ref CollectionsMarshal.GetValueRefOrAddDefault(_hash, new Point(x, y), out var exists);
				if (!exists)
				{
					entityList = [];
				}
				entityList!.Add(entity.Id);
			}
		}

		_entityCache[entity.Id] = new EntitySpatialHashCache( entity,
			new Rectangle(sourceMinX, sourceMinY, sourceMaxX - sourceMinX, sourceMaxY - sourceMinY))
		{
			Points = points.ToArray(),
		};

	}
	private void RemoveFromHash(Transform2DComponent? transform, BoxShape2DComponent? shape, Entity entity)
	{
		if (transform is null || shape is null) return;

		transform.Locked = false;
		var posX = transform.Position.X;
		var posY = transform.Position.Y;

		var minX = (int)(posX + shape.Left) / cellSize;
		var maxX = (int)(posX + shape.Right) / cellSize;
		var minY = (int)(posY + shape.Bottom) / cellSize;
		var maxY = (int)(posY + shape.Top) / cellSize;

		var width = maxX - minX + 1;
		var height = maxY - minY + 1;
		var totalCells = width * height;
		var points = totalCells <= 64
			? stackalloc Point[totalCells]
			: new Point[totalCells];

		var index = 0;
		for (var x = minX; x <= maxX; x++)
		{
			for (var y = minY; y <= maxY; y++)
			{
				points[index++] = new Point(x, y);

				ref var entityList = ref CollectionsMarshal.GetValueRefOrAddDefault(_hash, new Point(x, y), out var exists);
				if (!exists)
				{
					entityList = [];
				}
				entityList!.Remove(entity.Id);
			}
		}
		_entityCache.Remove(entity.Id);	
	}

	private EntitySpatialHashCache CollisionInitialScreen(ulong entityId)
	{
		return _entityCache.GetValueOrDefault(entityId);
	}

	public IEnumerable<Entity> CollisionScreen(ulong entityId)
	{
		var source = CollisionInitialScreen(entityId);
		var targets = new List<ulong>();
		var finalTargets = new List<Entity>();
		foreach (var targetCell in source.Points)
		{
			foreach (var cellEntity in _hash[targetCell].Where(cellEntity => !targets.Contains(cellEntity)))
			{
				var tagetTest = _entityCache[cellEntity];
				if (!tagetTest.Rectangle.Intersects(source.Rectangle)) continue;
				targets.Add(cellEntity);
				finalTargets.Add(tagetTest.Entity);
			}
		}
		return finalTargets;
	}

	public void Move(ulong id, Vector2 newPosition)
	{
		var entity = World.GetEntity(id);
		if (entity is null) return;
		var transform = entity.GetComponent<Transform2DComponent>();
		var shape = entity.GetComponent<BoxShape2DComponent>();
		RemoveFromHash(transform, shape, entity);
		transform?.Position = newPosition;
		AddToHash(transform, shape, entity);
	}
}

public struct EntitySpatialHashCache(Entity entity ,Rectangle rectangle)
{
	public Entity Entity = entity;
	public IEnumerable<Point> Points = [];
	public Rectangle Rectangle = rectangle;
}