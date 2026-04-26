using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace gamealien;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    List<Player> allies = new List<Player>();
    List<Enemy> enemies = new List<Enemy>();

    enum BattleState { PlayerTurn, EnemyTurn }
    BattleState currentState = BattleState.PlayerTurn;
    int activeunitindex = 0;

    KeyboardState oldState;


    Texture2D pixel;

    int selectedskillind = 0;
    int selectedtargetind = 0;
    bool isskillselected = false;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        allies.Add(new Player.Leroy("Ally1", 100, 50, 20, new Vector2(300, 200)));
        allies.Add(new Player.Renato("Ally2", 60, 15, 20, new Vector2(200, 200)));
        allies.Add(new Player.Yaser("Ally3", 70, 18, 20, new Vector2(100, 200)));

        enemies.Add(new Enemy("Enemy1", 40, 10, new Vector2(500, 200)));
        enemies.Add(new Enemy("Enemy2", 80, 25, new Vector2(580, 200)));
        enemies.Add(new Enemy("Enemy3", 30, 5, new Vector2(660, 200)));

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
        var kstate = Keyboard.GetState();

        if (currentState == BattleState.PlayerTurn)
        {
            if (kstate.IsKeyDown(Keys.Up) && oldState.IsKeyUp(Keys.Up)) selectedskillind--;
            if (kstate.IsKeyDown(Keys.Down) && oldState.IsKeyUp(Keys.Down)) selectedskillind++;
            selectedskillind = MathHelper.Clamp(selectedskillind, 0, 1);

            if (!isskillselected)
            {
                if (kstate.IsKeyDown(Keys.Enter) && oldState.IsKeyUp(Keys.Enter))
                    isskillselected = true;
            }
            else
            {
                if (kstate.IsKeyDown(Keys.Left) && oldState.IsKeyUp(Keys.Left)) selectedtargetind--;
                if (kstate.IsKeyDown(Keys.Right) && oldState.IsKeyUp(Keys.Right)) selectedtargetind++;
                selectedtargetind = MathHelper.Clamp(selectedtargetind, 0, enemies.Count - 1);

                if (kstate.IsKeyDown(Keys.Enter) && oldState.IsKeyUp(Keys.Enter))
                {
                    if (activeunitindex < allies.Count)
                    {
                        allies[activeunitindex].ExecuteSkill(selectedskillind, allies, enemies, selectedtargetind);

                        for (int i = enemies.Count - 1; i >= 0; i--)
                        {
                            if (enemies[i].CurrentHP <= 0)
                            {
                                enemies.RemoveAt(i);
                            }
                        }

                        if (selectedtargetind >= enemies.Count && enemies.Count > 0)
                        {
                            selectedtargetind = enemies.Count - 1;
                        }

                        isskillselected = false;
                        activeunitindex++;

                        if (activeunitindex >= allies.Count)
                        {
                            currentState = BattleState.EnemyTurn;
                            activeunitindex = 0;
                        }
                    }
                }

                if (kstate.IsKeyDown(Keys.Back) && oldState.IsKeyUp(Keys.Back))
                    isskillselected = false;
            }
        }
        else if (currentState == BattleState.EnemyTurn)
        {
            currentState = BattleState.PlayerTurn;
            activeunitindex = 0;
        }

        oldState = kstate;
        base.Update(gameTime);
    }

    private void DrawHealthBar(Vector2 position, int currentHP, int maxHP)
    {
        _spriteBatch.Draw(pixel, new Rectangle((int)position.X, (int)position.Y - 15, 50, 5), Color.Red);

        float healthPercent = (float)currentHP / maxHP;
        _spriteBatch.Draw(pixel, new Rectangle((int)position.X, (int)position.Y - 15, (int)(50 * healthPercent), 5), Color.Green);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        _spriteBatch.Begin();

        for (int i = 0; i < allies.Count; i++)
        {
            Color c = (i == activeunitindex && currentState == BattleState.PlayerTurn) ? Color.Green : Color.Blue;
            allies[i].Draw(_spriteBatch, c);

            DrawHealthBar(allies[i].Position, allies[i].CurrentHP, allies[i].MaxHP);
        }

        for (int i = 0; i < enemies.Count; i++)
        {
            Color c = (i == selectedtargetind && isskillselected) ? Color.Yellow : Color.Red;
            enemies[i].Draw(_spriteBatch, c);

            DrawHealthBar(enemies[i].Position, enemies[i].CurrentHP, enemies[i].MaxHP);
        }

        if (currentState == BattleState.PlayerTurn)
        {
            _spriteBatch.Draw(pixel, new Rectangle(0, 350, 800, 150), Color.Black * 0.7f);

            Color s1Color = (selectedskillind == 0) ? Color.Gold : Color.Gray;
            _spriteBatch.Draw(pixel, new Rectangle(50, 380, 100, 40), s1Color);

            Color s2Color = (selectedskillind == 1) ? Color.Gold : Color.Gray;
            _spriteBatch.Draw(pixel, new Rectangle(160, 380, 100, 40), s2Color);

            if (isskillselected)
            {
                _spriteBatch.Draw(pixel, new Rectangle(380, 320, 40, 10), Color.Yellow);
            }
        }

        _spriteBatch.End();
        base.Draw(gameTime);
    }
}
