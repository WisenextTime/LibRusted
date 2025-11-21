using System.Collections.Generic;

namespace LibRusted.Core.ECS;

public readonly struct WorldChangeArguments()
{
	private readonly Dictionary<string,object?> _args = [];
	public void SetValue<T>(string key, T value) => _args[key] = value;
	public T? GetValue<T>(string key) => (T?)_args.GetValueOrDefault(key);
}