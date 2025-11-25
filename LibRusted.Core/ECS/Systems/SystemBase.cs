namespace LibRusted.Core.ECS.Systems;

public abstract class SystemBase : ISystem
{
	public bool Enabled { get; set; } = true;
	public int Priority { get; set; }
	public bool Available { get; private set; }
	
	protected World.World World = null!;

	public void Ready()
	{
		if (Available) return;
		Available = true;
		Initialize();
	}

	public void BeAdded(World.World world)
	{
		World = world;
	}

	protected virtual void Initialize() { }
}