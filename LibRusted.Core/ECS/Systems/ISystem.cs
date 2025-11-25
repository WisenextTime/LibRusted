namespace LibRusted.Core.ECS.Systems;

public interface ISystem
{
	bool Enabled { get; }
	
	int Priority { get; }
	bool Available { get; }

	void BeAdded(World.World world);

	void Ready();
}