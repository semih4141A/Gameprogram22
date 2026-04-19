using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace gamealien;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    List<BaseCharacter> allies = new List<BaseCharacter>();
    List<BaseCharacter> enemies = new List<BaseCharacter>();

    Texture2D pixel;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        allies.Add(new Player("Player", 100, 50, 20, new Vector2(300, 200)));
        allies.Add(new BaseCharacter("Ally1", 60, 15, new Vector2(200, 200)));
        allies.Add(new BaseCharacter("Ally2", 70, 18, new Vector2(100, 200)));

        enemies.Add(new BaseCharacter("Enemy1", 40, 10, new Vector2(500, 200)));
        enemies.Add(new BaseCharacter("Enemy2", 80, 25, new Vector2(580, 200)));
        enemies.Add(new BaseCharacter("Enemy3", 30, 5, new Vector2(660, 200)));

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        pixel = new Texture2D(GraphicsDevice, 1, 1);
        pixel.SetData(new[] { Color.White });


        foreach (var a in allies) a.Sprite = pixel;
        foreach (var e in enemies) e.Sprite = pixel;


    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();


        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();

        foreach (var a in allies)
        {
            a.Draw(_spriteBatch, Color.Blue);
        }

        foreach (var e in enemies)
        {
            e.Draw(_spriteBatch, Color.Red);
        }

        _spriteBatch.End();


        base.Draw(gameTime);
    }
}
