namespace LibRusted.Core.ECS.Systems;

public interface ISystem
{
	bool Enabled { get; set; }
	bool Available { get; }

	void BeAdded(World world);

	void Ready();
}