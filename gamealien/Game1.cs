using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.Direct3D9;

namespace gamealien;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    SpriteFont gameFont;

    List<Player> allies = new List<Player>();
    List<Enemy> enemies = new List<Enemy>();

    enum BattleState { PlayerTurn, EnemyTurn, GameOver, Victory }
    BattleState currentState = BattleState.PlayerTurn;
    int activeunitindex = 0;

    int currentWave = 1;

    float enemyTurnTimer = 0;
    int enemyIndex = 0;
    bool isRenatoUltimateUnlocked = false;
    bool isYaserUltimateUnlocked = false;
    bool isLeroyUltimateUnlocked = false;

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
        allies.Add(new Player.Leroy("Leroy", 100, 50, 20, new Vector2(300, 200)));
        allies.Add(new Player.Renato("Renato", 60, 50, 15, new Vector2(200, 200)));
        allies.Add(new Player.Yaser("Yaser", 70, 50, 20, new Vector2(100, 200)));

        enemies.Add(new Vampire("Vampire", 60, 12, new Vector2(500, 200)));
        enemies.Add(new Skeleton("Skeleton", 40, 15, new Vector2(580, 200)));
        enemies.Add(new Vampire("Vampire", 60, 12, new Vector2(660, 200)));

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        gameFont = Content.Load<SpriteFont>("GameFont");

        pixel = new Texture2D(GraphicsDevice, 1, 1);
        pixel.SetData(new[] { Color.White });


        foreach (var a in allies) a.Sprite = pixel;
        foreach (var e in enemies) e.Sprite = pixel;


    }

    protected override void Update(GameTime gameTime)
    {
        var kstate = Keyboard.GetState();

        if (currentState == BattleState.GameOver || currentState == BattleState.Victory)
        {

            return;
        }

        if (enemies.Count == 0)
        {

            currentWave++;


            if (currentWave == 2) isRenatoUltimateUnlocked = true;
            if (currentWave == 3) isYaserUltimateUnlocked = true;
            if (currentWave == 4) isLeroyUltimateUnlocked = true;

            if (currentWave == 2)
            {
                enemies.Add(new Zombie("Zombie", 60, 10, new Vector2(500, 200)));
                enemies.Add(new Barbarian("Barbarian", 80, 15, new Vector2(580, 200)));
                enemies.Add(new Zombie("Zombie", 60, 10, new Vector2(660, 200)));
            }
            else if (currentWave == 3)
            {
                enemies.Add(new Witch("Witch", 50, 20, new Vector2(500, 200)));
                enemies.Add(new HealerGoblin("HealerGoblin", 50, 5, new Vector2(580, 200)));
                enemies.Add(new Witch("Witch", 50, 20, new Vector2(660, 200)));
            }
            else if (currentWave == 4)
            {
                enemies.Add(new FinalBoss("FinalBoss", 300, 30, new Vector2(580, 200)));
                enemies.Add(new Witch("Witch", 50, 20, new Vector2(500, 200)));
                enemies.Add(new HealerGoblin("HealerGoblin", 50, 5, new Vector2(660, 200)));

            }

            foreach (var e in enemies) e.Sprite = pixel;
            System.Diagnostics.Debug.WriteLine("New Wave " + currentWave);


        }

        if (currentState == BattleState.PlayerTurn)
        {
            if (kstate.IsKeyDown(Keys.Up) && oldState.IsKeyUp(Keys.Up)) selectedskillind--;
            if (kstate.IsKeyDown(Keys.Down) && oldState.IsKeyUp(Keys.Down)) selectedskillind++;
            selectedskillind = MathHelper.Clamp(selectedskillind, 0, 3);

            if (!isskillselected)
            {
                if (kstate.IsKeyDown(Keys.Enter) && oldState.IsKeyUp(Keys.Enter))
                    isskillselected = true;
            }
            else
            {
                bool isFriendlyTarget = false;
                if (activeunitindex == 1 && (selectedskillind == 1 || selectedskillind == 2)) isFriendlyTarget = true;
                if (activeunitindex == 2 && selectedskillind == 2) isFriendlyTarget = true;

                if (isFriendlyTarget)
                {
                    if (kstate.IsKeyDown(Keys.Left) && oldState.IsKeyUp(Keys.Left)) selectedtargetind++;
                    if (kstate.IsKeyDown(Keys.Right) && oldState.IsKeyUp(Keys.Right)) selectedtargetind--;

                    selectedtargetind = MathHelper.Clamp(selectedtargetind, 0, allies.Count - 1);

                }

                else
                {
                    if (kstate.IsKeyDown(Keys.Left) && oldState.IsKeyUp(Keys.Left)) selectedtargetind--;
                    if (kstate.IsKeyDown(Keys.Right) && oldState.IsKeyUp(Keys.Right)) selectedtargetind++;

                    selectedtargetind = MathHelper.Clamp(selectedtargetind, 0, enemies.Count - 1);
                }


                if (kstate.IsKeyDown(Keys.Enter) && oldState.IsKeyUp(Keys.Enter))
                {
                    if (activeunitindex < allies.Count)
                    {
                        bool success = false;
                        var currentPlayer = allies[activeunitindex];

                        if (currentPlayer is Player.Renato)
                            success = currentPlayer.ExecuteSkill(selectedskillind, allies, enemies, selectedtargetind, isRenatoUltimateUnlocked);
                        else if (currentPlayer is Player.Yaser)
                            success = currentPlayer.ExecuteSkill(selectedskillind, allies, enemies, selectedtargetind, isYaserUltimateUnlocked);
                        else if (currentPlayer is Player.Leroy)
                            success = currentPlayer.ExecuteSkill(selectedskillind, allies, enemies, selectedtargetind, isLeroyUltimateUnlocked);

                        if (success)
                        {
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
                                enemyIndex = 0;
                                enemyTurnTimer = 0f;
                            }

                        }

                        else
                        {
                            System.Console.WriteLine("Not enough MP or skill is locked");
                        }


                    }
                }

                if (kstate.IsKeyDown(Keys.Back) && oldState.IsKeyUp(Keys.Back))
                    isskillselected = false;
            }






        }

        else if (currentState == BattleState.EnemyTurn)
        {
            enemyTurnTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (enemyTurnTimer > 1.0f)
            {
                if (enemyIndex < enemies.Count)
                {
                    enemies[enemyIndex].TakeTurn(allies, enemies);
                    enemyIndex++;
                    enemyTurnTimer = 0f;
                }
                else
                {
                    currentState = BattleState.PlayerTurn;
                    activeunitindex = 0;
                    enemyIndex = 0;
                    enemyTurnTimer = 0f;


                }
            }
        }

        for (int i = allies.Count - 1; i >= 0; i--)
        {
            if (allies[i].CurrentHP <= 0)
            {
                allies.RemoveAt(i);
                System.Diagnostics.Debug.WriteLine("Bir müttefik elendi!");
            }
        }

        if (allies.Count == 0)
        {
            System.Diagnostics.Debug.WriteLine("GAME OVER!");
            currentState = BattleState.GameOver;
        }

        if (enemies.Count == 0 && currentWave == 4)
        {
            currentState = BattleState.Victory;
        }







        oldState = kstate;
        base.Update(gameTime);
    }

    private void DrawHealthBar(Vector2 position, int currentHP, int maxHP)
    {
        _spriteBatch.Draw(pixel, new Rectangle((int)position.X, (int)position.Y - 20, 50, 6), Color.Red);

        float healthPercent = (float)currentHP / maxHP;
        _spriteBatch.Draw(pixel, new Rectangle((int)position.X, (int)position.Y - 20, (int)(50 * healthPercent), 6), Color.Green);

        string hpText = $"{currentHP}/{maxHP}";
        _spriteBatch.DrawString(gameFont, hpText, new Vector2(position.X, position.Y - 35), Color.White, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 0);
    }

    private void DrawManaBar(Vector2 position, int currentMP, int maxMP)
    {
        _spriteBatch.Draw(pixel, new Rectangle((int)position.X, (int)position.Y - 12, 50, 4), Color.Black * 0.5f);

        float manaPercent = (float)currentMP / maxMP;
        _spriteBatch.Draw(pixel, new Rectangle((int)position.X, (int)position.Y - 12, (int)(50 * manaPercent), 4), Color.DeepSkyBlue);

        string mpText = $"{currentMP}/{maxMP}";
        _spriteBatch.DrawString(gameFont, mpText, new Vector2(position.X + 55, position.Y - 15), Color.Cyan, 0, Vector2.Zero, 0.6f, SpriteEffects.None, 0);
    }
    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        _spriteBatch.Begin();

        for (int i = 0; i < allies.Count; i++)
        {
            bool isHealing = (activeunitindex == 1 && (selectedskillind == 1 || selectedskillind == 2)) ||
                             (activeunitindex == 2 && selectedskillind == 2);

            bool isTargeted = (isskillselected && isHealing && selectedtargetind == i);

            Color c = isTargeted ? Color.Yellow : (i == activeunitindex ? Color.Green : Color.Blue);
            allies[i].Draw(_spriteBatch, c, gameFont);
            DrawHealthBar(allies[i].Position, allies[i].CurrentHP, allies[i].MaxHP);
            DrawManaBar(allies[i].Position, allies[i].CurrentMP, allies[i].MaxMP);
        }

        for (int i = 0; i < enemies.Count; i++)
        {
            bool isHealing = (activeunitindex == 1 && (selectedskillind == 1 || selectedskillind == 2)) ||
                             (activeunitindex == 2 && selectedskillind == 2);

            bool isTargeted = (isskillselected && !isHealing && selectedtargetind == i);

            Color c = isTargeted ? Color.Yellow : Color.Red;
            enemies[i].Draw(_spriteBatch, c, gameFont);
            DrawHealthBar(enemies[i].Position, enemies[i].CurrentHP, enemies[i].MaxHP);
        }

        if (currentState == BattleState.PlayerTurn)
        {
            Vector2 pPos = allies[activeunitindex].Position;

            for (int i = 0; i < 4; i++)
            {
                Color boxColor = (selectedskillind == i) ? Color.Gold : Color.Gray * 0.6f;

                Rectangle skillBox = new Rectangle((int)pPos.X, (int)pPos.Y - 180 + (i * 40), 50, 30);

                _spriteBatch.Draw(pixel, skillBox, boxColor);
            }


            if (isskillselected)
            {

                bool isFriendlyTarget = (activeunitindex == 1 && (selectedskillind == 1 || selectedskillind == 2)) ||
                                        (activeunitindex == 2 && selectedskillind == 2);

                Vector2 targetPos;
                if (isFriendlyTarget)
                    targetPos = allies[selectedtargetind].Position;
                else
                    targetPos = enemies[selectedtargetind].Position;

                _spriteBatch.Draw(pixel, new Rectangle((int)targetPos.X, (int)targetPos.Y + 100, 40, 10), Color.Yellow);
            }
        }

        _spriteBatch.End();
        base.Draw(gameTime);
    }
}
