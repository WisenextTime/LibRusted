namespace LibRusted.Core.ECS;

public interface IAvailable
{
	public bool Available { get; }

	public void Ready();
}