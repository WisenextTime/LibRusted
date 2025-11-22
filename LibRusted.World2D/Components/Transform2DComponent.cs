using System;
using LibRusted.Core.ECS.Components;
using Microsoft.Xna.Framework;
namespace LibRusted.World2D.Components;

public class Transform2DComponent(Vector2 position = default, float rotation = 0, Vector2 scale = default, Vector2 orign = default)
	: IComponent
{
	public Vector2 Position
	{
		get;
		set
		{
			if(value != field)
				_isDirty = true;
			field = value;
		}
	} = position;

	public float Rotation
	{
		get;
		set
		{
			if(!value.Equals(field))
				_isDirty = true;
			field = value;
		}
	} = rotation;

	public Vector2 Scale
	{
		get;
		set
		{
			if(value != field)
				_isDirty = true;
			field = value;
		}
	} = scale == default ? Vector2.One : scale;

	public Vector2 Origin
	{
		get;
		set
		{
			if(value != field)
				_isDirty = true;
			field = value;
		}
	} = orign;

	private Matrix? _cache;
	private bool _isDirty = true;

	private Matrix GetWorldMatrix()
	{
		if(_isDirty || _cache is null)
		{
			_cache = Matrix.CreateTranslation(-Origin.X, -Origin.Y, 0) *
			         Matrix.CreateScale(Scale.X, Scale.Y, 0) *
			         Matrix.CreateRotationZ(Rotation) *
			         Matrix.CreateTranslation(Position.X, Position.Y, 0);
			_isDirty = false;
		}
		return _cache.Value;
	}

	public Vector2 TransformPosition(Vector2 position)
	{
		return Vector2.Transform(position, GetWorldMatrix());
	}

	public Vector2 InverseTransformPosition(Vector2 position)
	{
		return Vector2.Transform(position, GetWorldMatrix());
	}
	
	public void LookAt(Vector2 position)
	{
		var direction = position - Position;
		Rotation = MathF.Atan2(direction.Y, direction.X);
	}

	public float DistanceTo(Vector2 position)
	{
		return Vector2.Distance(Position, position);
	}
	
	public float DistanceSquaredTo(Vector2 position)
	{
		return Vector2.DistanceSquared(Position, position);
	}
}