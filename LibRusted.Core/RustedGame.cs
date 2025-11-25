using System;
using System.Collections.Generic;
using System.Linq;
using LibRusted.Core.ECS;
using LibRusted.Core.ECS.Components;
using LibRusted.Core.ECS.World;
using LibRusted.Core.Pool;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LibRusted.Core;

public class RustedGame : IAvailable
{
	public readonly static RustedGame GameInstance = new(new MainLoop());
	/// <summary>
	/// Get the graphics-device of main-loop
	/// </summary>
	public GraphicsDevice GraphicsDevice => _mainLoop.GraphicsDevice;
	/// <summary>
	/// Get the content-manager of main-loop
	/// </summary>
	public ContentManager Content => _mainLoop.Content;
	/// <summary>
	/// Get the game window of main-loop;
	/// </summary>
	public GameWindow Window => _mainLoop.Window;
	/// <summary>
	/// Will be true after main-loop load content
	/// </summary>
	public bool Available { get; private set; }
	/// <summary>
	/// Will invoke after load content
	/// </summary>
	public Action? InitAction;
	/// <summary>
	/// Will invoke after unload content;
	/// </summary>
	public Action? UnloadAction => _mainLoop.UnloadContentEvent;

	public (World? world, WorldChangeArguments arguments) QueueChangeWorld;
	
	private readonly MainLoop _mainLoop;
	private World? _world;
	private readonly ObjectPool<Entity> _entityPool = new(() => new Entity());
	public static IEnumerable<Type> AllComponentTypes => field??= GetAllComponentTypes();
	internal Entity CreatEntity()
	{
		return _entityPool.Create();
	}

	internal void ReturnEntity(Entity entity)
	{
		_entityPool.Return(entity);
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
	internal RustedGame(MainLoop mainLoop)
	{
		_mainLoop = mainLoop;
		_mainLoop.LoadContentEvent += Load;
		_mainLoop.DrawEvent += Draw;
		_mainLoop.UpdateEvent += Update;
		_entityPool.AddBuffer(128);
	}
	private void Load()
	{
		Ready();
		InitAction?.Invoke();
	}
	private void Update(GameTime gameTime)
	{
		_world?.Update(gameTime);
		if (QueueChangeWorld.world == null) return;
		_world = QueueChangeWorld.world;
		_world.Clear();
		_world.OnWorldChange?.Invoke(QueueChangeWorld.arguments);
		if(Available) _world.Ready();
		QueueChangeWorld.world = null;
		QueueChangeWorld.arguments = default;
	}
	private void Draw(GameTime gameTime)
	{
		_world?.Draw(gameTime);
	}

	public void Ready() 
	{
		Available = true;
		_world?.Ready();
	}

	public void ChangeWorld(World world, WorldChangeArguments arguments = default)
	{
		QueueChangeWorld = (world, arguments);
	}
	
	public void Run() => _mainLoop.Run();

	public void ChangeWindowSize(int width, int height,bool fullscreen)
	{
		var graphicsDeviceManager = _mainLoop.Graphics;
		graphicsDeviceManager.PreferredBackBufferWidth = width;
		graphicsDeviceManager.PreferredBackBufferHeight = height;
		graphicsDeviceManager.IsFullScreen = fullscreen;
		graphicsDeviceManager.ApplyChanges();
	}
}