using System;
using System.Buffers;
using System.Collections.Concurrent;
namespace LibRusted.Core.Pool;

public class ObjectPool<T>(Func<T> generator) where T : IPoolable
{
	private readonly ConcurrentBag<T> _bag = [];

	public void AddBuffer(int size)
	{
		for (var _ = 0; _ < size; _++)
		{
			_bag.Add(generator());
		}
	}
	public T Create()
	{
		if(_bag.TryTake(out var result))return result;
		result = generator();
		_bag.Add(result);
		return result;
	}
	
	public T Return(T obj)
	{
		obj.Reset();
		_bag.Add(obj);
		return obj;
	}
}