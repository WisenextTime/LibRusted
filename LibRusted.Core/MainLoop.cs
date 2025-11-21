using System;
using LibRusted.Core.ECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace LibRusted.Core;

internal class MainLoop : Game
{
	public GraphicsDeviceManager Graphics;
	private SpriteBatch _spriteBatch = null!;
	public MainLoop()
	{
		Graphics = new GraphicsDeviceManager(this);
		Content.RootDirectory = "Content";
		IsMouseVisible = true;
	}

	public Action? InitializeEvent;
	public Action? LoadContentEvent;
	public Action? UnloadContentEvent;
	public Action<GameTime>? UpdateEvent;
	public Action<GameTime>? DrawEvent;

	protected override void Initialize()
	{
		InitializeEvent?.Invoke();
		base.Initialize();
	}

	protected override void LoadContent() 
	{
		_spriteBatch = new SpriteBatch(GraphicsDevice);
		LoadContentEvent?.Invoke();
	}

	protected override void UnloadContent() => UnloadContentEvent?.Invoke();

	protected override void Update(GameTime gameTime)
	{
		UpdateEvent?.Invoke(gameTime);
		base.Update(gameTime);
	}

	protected override void Draw(GameTime gameTime)
	{
		DrawEvent?.Invoke(gameTime);
		base.Draw(gameTime);
	}
}