using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.Direct3D9;
using Microsoft.Xna.Framework.Media;

namespace gamealien;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    SpriteFont gameFont;

    List<Player> allies = new List<Player>();
    List<Enemy> enemies = new List<Enemy>();

    List<Texture2D> battlegrounds = new List<Texture2D>();

    enum BattleState { PlayerTurn, MainMenu, EnemyTurn, GameOver, Victory }
    BattleState currentState = BattleState.MainMenu;

    Song mainMenuMusic;
    Song battleMusic;

    int menuIndex = 0;
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
        allies.Add(new Player.Leroy("Leroy", 100, 60, 999, new Vector2(300, 255)));
        allies.Add(new Player.Renato("Renato", 70, 60, 15, new Vector2(150, 255)));
        allies.Add(new Player.Yaser("Yaser", 70, 60, 20, new Vector2(0, 200)));

        var v1 = new Vampire("Vampire", 60, 10, new Vector2(450, 215)) { UIIndex = 0 };
        var s1 = new Skeleton("Skeleton", 40, 15, new Vector2(510, 200)) { UIIndex = 1 };
        var v2 = new Vampire("Vampire", 60, 10, new Vector2(650, 215)) { UIIndex = 2 };

        enemies.Add(v1);
        enemies.Add(s1);
        enemies.Add(v2);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        gameFont = Content.Load<SpriteFont>("GameFont");

        pixel = new Texture2D(GraphicsDevice, 1, 1);
        battlegrounds.Add(Content.Load<Texture2D>("Backgrounds/Battleground1"));
        battlegrounds.Add(Content.Load<Texture2D>("Backgrounds/Battleground2"));
        battlegrounds.Add(Content.Load<Texture2D>("Backgrounds/Battleground3"));
        battlegrounds.Add(Content.Load<Texture2D>("Backgrounds/Battleground4"));
        mainMenuMusic = Content.Load<Song>("Audio/MainMenuMusic");
        battleMusic = Content.Load<Song>("Audio/BattleMusic");

        MediaPlayer.IsRepeating = true;

        MediaPlayer.Play(mainMenuMusic);
        pixel.SetData(new[] { Color.White });

        var leroy = allies.Find(a => a.Name == "Leroy");
        if (leroy != null)
        {

            leroy.spriteIdle = Content.Load<Texture2D>("Characters/Leroy/Idle");
            leroy.spriteAttack = Content.Load<Texture2D>("Characters/Leroy/Attack 1");
            leroy.spriteDefend = Content.Load<Texture2D>("Characters/Leroy/Defend");
            leroy.spriteHurt = Content.Load<Texture2D>("Characters/Leroy/Hurt");
            leroy.spriteDead = Content.Load<Texture2D>("Characters/Leroy/Dead");
        }

        var renato = allies.Find(a => a.Name == "Renato");
        if (renato != null)
        {
            renato.spriteIdle = Content.Load<Texture2D>("Characters/Renato/Idle");
            renato.spriteAttack = Content.Load<Texture2D>("Characters/Renato/Attacks");
            renato.spriteHurt = Content.Load<Texture2D>("Characters/Renato/Hurt");
            renato.spriteDead = Content.Load<Texture2D>("Characters/Renato/Death");
            renato.spriteCrouchAttack = Content.Load<Texture2D>("Characters/Renato/crouch_attacks");
            renato.spriteHeal = Content.Load<Texture2D>("Characters/Renato/Heal");
            renato.spritePowerup = Content.Load<Texture2D>("Characters/Renato/PowerUp");
        }

        renato.idleColumns = 2; renato.idleRows = 4;
        renato.attackColumns = 8; renato.attackRows = 5;
        renato.hurtColumns = 2; renato.hurtRows = 2;
        renato.deadColumns = 2; renato.deadRows = 3;
        renato.ultColumns = 2; renato.ultRows = 4;

        renato.frameCountIdle = renato.idleColumns * renato.idleRows;
        renato.frameCountAttack = renato.attackColumns * renato.attackRows;
        renato.frameCountHurt = renato.hurtColumns * renato.hurtRows;
        renato.frameCountDead = renato.deadColumns * renato.deadRows;
        renato.frameCountUlt = renato.ultColumns * renato.ultRows;

        var yaser = allies.Find(a => a.Name == "Yaser");
        if (yaser != null)
        {
            yaser.spriteYaserIdle = Content.Load<Texture2D>("Characters/Yaser/Idle");
            yaser.spriteYaserAttack = Content.Load<Texture2D>("Characters/Yaser/Attack");
            yaser.spriteYaserHurt = Content.Load<Texture2D>("Characters/Yaser/Hurt");
            yaser.spriteYaserDead = Content.Load<Texture2D>("Characters/Yaser/Death");
            yaser.spriteYaserUlt = Content.Load<Texture2D>("Characters/Yaser/UltimateAttack");

        }
        Vampire.LoadSprites(this.Content);
        System.Diagnostics.Debug.WriteLine("✅ KUTSALLIK: Vampir resimleri static olarak hafızaya alındı.");

        Skeleton.LoadSprites(this.Content);
        System.Diagnostics.Debug.WriteLine("✅ KUTSALLIK: Skeleton resimleri static olarak hafızaya alındı.");

        Bat.LoadSprites(this.Content);
        System.Diagnostics.Debug.WriteLine("✅ KUTSALLIK: Yarasa (Bat) resimleri static olarak hafızaya alındı.");

        EvilWizard.LoadSprites(this.Content);
        System.Diagnostics.Debug.WriteLine("✅ KUTSALLIK: EvilWizard resimleri static olarak hafızaya alındı.");

        Sorcerer.LoadSprites(this.Content);
        System.Diagnostics.Debug.WriteLine("✅ KUTSALLIK: Sorcerer resimleri static olarak hafızaya alındı.");

        Zombie.LoadSprites(this.Content);
        System.Diagnostics.Debug.WriteLine("✅ KUTSALLIK: Zombie resimleri static olarak hafızaya alındı.");

        FinalBoss.LoadSprites(this.Content);
        System.Diagnostics.Debug.WriteLine("✅ KUTSALLIK: FinalBoss tekil resimleri hafızaya alındı.");

        MediaPlayer.Volume = 0.4f;



    }

    protected override void Update(GameTime gameTime)
    {
        var kstate = Keyboard.GetState();

        if (currentState == BattleState.MainMenu)
        {
            if (kstate.IsKeyDown(Keys.Up) && oldState.IsKeyUp(Keys.Up)) menuIndex = 0;
            if (kstate.IsKeyDown(Keys.Down) && oldState.IsKeyUp(Keys.Down)) menuIndex = 1;

            if (kstate.IsKeyDown(Keys.Enter) && oldState.IsKeyUp(Keys.Enter))
            {
                if (menuIndex == 0)
                {
                    ResetWholeGame();
                    currentState = BattleState.PlayerTurn;
                    MediaPlayer.Play(battleMusic);
                }
                else if (menuIndex == 1)
                {
                    Exit();
                }
            }

            oldState = kstate;
            base.Update(gameTime);
            return;
        }

        if (currentState == BattleState.GameOver || currentState == BattleState.Victory)
        {
            if (currentState == BattleState.GameOver)
            {
                if (kstate.IsKeyDown(Keys.R) && oldState.IsKeyUp(Keys.R))
                {
                    ResetWholeGame();
                    currentState = BattleState.PlayerTurn;
                    MediaPlayer.Play(battleMusic);
                }
                if (kstate.IsKeyDown(Keys.M) && oldState.IsKeyUp(Keys.M))
                {
                    currentState = BattleState.MainMenu;
                    MediaPlayer.Play(mainMenuMusic);
                }
            }

            if (currentState == BattleState.Victory && kstate.IsKeyDown(Keys.Enter) && oldState.IsKeyUp(Keys.Enter))
            {
                currentState = BattleState.MainMenu;
                MediaPlayer.Play(mainMenuMusic);
            }

            oldState = kstate;
            base.Update(gameTime);
            return;
        }

        foreach (var ally in allies)
        {
            ally.UpdateAnimation(gameTime);
        }

        foreach (var enemy in enemies)
        {
            enemy.UpdateAnimation(gameTime);
        }

        if (currentState == BattleState.GameOver || currentState == BattleState.Victory)
        {
            if (currentState == BattleState.GameOver && kstate.IsKeyDown(Keys.R) && oldState.IsKeyUp(Keys.R))
            {
                currentWave = 1;
                activeunitindex = 0;
                enemyIndex = 0;
                selectedskillind = 0;
                selectedtargetind = 0;
                isskillselected = false;

                allies.Clear();
                enemies.Clear();

                Initialize();

                currentState = BattleState.PlayerTurn;
            }
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
                enemies.Add(new Zombie("Zombie", 60, 10, new Vector2(470, 240)) { UIIndex = 0 });
                enemies.Add(new Bat("Bat", 80, 15, new Vector2(550, 240)) { UIIndex = 1 });
                enemies.Add(new Zombie("Zombie", 60, 10, new Vector2(620, 240)) { UIIndex = 2 });
            }
            else if (currentWave == 3)
            {
                enemies.Add(new EvilWizard("EvilWizard", 50, 20, new Vector2(350, 120)) { UIIndex = 0 });
                enemies.Add(new Sorcerer("Sorcerer", 50, 5, new Vector2(520, 220)) { UIIndex = 1 });
                enemies.Add(new EvilWizard("EvilWizard", 50, 20, new Vector2(520, 120)) { UIIndex = 2 });
            }
            else if (currentWave == 4)
            {
                enemies.Add(new FinalBoss("FinalBoss", 200, 30, new Vector2(370, 215)) { UIIndex = 0 });
                enemies.Add(new EvilWizard("EvilWizard", 50, 20, new Vector2(450, 120)) { UIIndex = 1 });
                enemies.Add(new Sorcerer("Sorcerer", 50, 5, new Vector2(600, 220)) { UIIndex = 2 });
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
                {
                    isskillselected = true;
                    selectedtargetind = 0;
                }
            }
            else
            {
                bool isFriendlyTarget = (activeunitindex == 1 && (selectedskillind == 1 || selectedskillind == 2)) ||
                                        (activeunitindex == 2 && selectedskillind == 2);

                if (isFriendlyTarget)
                    selectedtargetind = MathHelper.Clamp(selectedtargetind, 0, Math.Max(0, allies.Count - 1));
                else
                    selectedtargetind = MathHelper.Clamp(selectedtargetind, 0, Math.Max(0, enemies.Count - 1));

                if (isFriendlyTarget)
                {
                    if (kstate.IsKeyDown(Keys.Left) && oldState.IsKeyUp(Keys.Left))
                    {
                        selectedtargetind++;
                        if (selectedtargetind >= allies.Count) selectedtargetind = 0;
                    }
                    if (kstate.IsKeyDown(Keys.Right) && oldState.IsKeyUp(Keys.Right))
                    {
                        selectedtargetind--;
                        if (selectedtargetind < 0) selectedtargetind = allies.Count - 1;
                    }

                    selectedtargetind = MathHelper.Clamp(selectedtargetind, 0, allies.Count - 1);
                }
                else
                {
                    if (kstate.IsKeyDown(Keys.Left) && oldState.IsKeyUp(Keys.Left))
                    {
                        selectedtargetind--;
                        if (selectedtargetind < 0) selectedtargetind = enemies.Count - 1;
                    }
                    if (kstate.IsKeyDown(Keys.Right) && oldState.IsKeyUp(Keys.Right))
                    {
                        selectedtargetind++;
                        if (selectedtargetind >= enemies.Count) selectedtargetind = 0;
                    }

                    if (enemies.Count > 0)
                        selectedtargetind = MathHelper.Clamp(selectedtargetind, 0, enemies.Count - 1);
                    else
                        selectedtargetind = 0;
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
                                    if (enemies[i].CurrentState != BaseCharacter.CharacterState.Dead)
                                    {
                                        enemies[i].CurrentState = BaseCharacter.CharacterState.Dead;
                                        enemies[i].currentFrame = 0;
                                    }
                                }
                            }

                            if (selectedtargetind >= enemies.Count && enemies.Count > 0)
                            {
                                selectedtargetind = enemies.Count - 1;
                            }

                            isskillselected = false;



                            activeunitindex++;


                            if (activeunitindex >= allies.Count || allies.Count == 0)
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
                    if (enemies[enemyIndex].CurrentHP > 0 && enemies[enemyIndex].CurrentState != BaseCharacter.CharacterState.Dead)
                    {
                        enemies[enemyIndex].TakeTurn(allies, enemies);
                        enemyIndex++;
                        enemyTurnTimer = 0f;
                    }
                    else
                    {
                        enemyIndex++;
                    }
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
                if (allies[i].CurrentState != BaseCharacter.CharacterState.Dead)
                {
                    allies[i].CurrentState = BaseCharacter.CharacterState.Dead;
                    allies[i].currentFrame = 0;
                    System.Diagnostics.Debug.WriteLine($"{allies[i].Name} elendi, ölüm animasyonu tetiklendi.");
                }



            }
        }

        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            if (enemies[i].CurrentHP <= 0)
            {
                if (enemies[i].CurrentState != BaseCharacter.CharacterState.Dead)
                {
                    enemies[i].CurrentState = BaseCharacter.CharacterState.Dead;
                    enemies[i].currentFrame = 0;
                }
            }
        }

        allies.RemoveAll(ally =>
            (ally.Name == "Yaser" && ally.CurrentState == BaseCharacter.CharacterState.Dead && ally.currentFrame >= 6) ||
            (ally.Name == "Renato" && ally.CurrentState == BaseCharacter.CharacterState.Dead && ally.currentFrame >= 3) ||
            (ally.Name == "Leroy" && ally.CurrentState == BaseCharacter.CharacterState.Dead && ally.currentFrame >= 5)
        );
        enemies.RemoveAll(enemy =>
             (enemy is Vampire && enemy.CurrentState == BaseCharacter.CharacterState.Dead && enemy.currentFrame >= 7) ||
             (enemy is Skeleton && enemy.CurrentState == BaseCharacter.CharacterState.Dead && enemy.currentFrame >= 3) ||
             (enemy is Bat && enemy.CurrentState == BaseCharacter.CharacterState.Dead && enemy.currentFrame >= 3) ||
             (enemy is EvilWizard && enemy.CurrentState == BaseCharacter.CharacterState.Dead && enemy.currentFrame >= 6) ||
             (enemy is Sorcerer && enemy.CurrentState == BaseCharacter.CharacterState.Dead && enemy.currentFrame >= 15) ||
             (enemy is Zombie && enemy.CurrentState == BaseCharacter.CharacterState.Dead && enemy.currentFrame >= 5) ||
             (enemy is FinalBoss && enemy.CurrentState == BaseCharacter.CharacterState.Dead && enemy.currentFrame >= FinalBoss.listBossDead.Count - 1) ||
             (!(enemy is Vampire) && !(enemy is Skeleton) && !(enemy is Bat) && !(enemy is EvilWizard) && !(enemy is Sorcerer) && !(enemy is Zombie) && !(enemy is FinalBoss) && enemy.CurrentState == BaseCharacter.CharacterState.Dead)

         );



        if (currentState == BattleState.PlayerTurn)
        {
            if (activeunitindex < allies.Count && allies[activeunitindex].CurrentHP <= 0)
            {
                activeunitindex++;
            }

            bool anyRealAllyAlive = false;
            foreach (var ally in allies)
            {
                if (ally.CurrentHP > 0) anyRealAllyAlive = true;
            }

            if (activeunitindex >= allies.Count || !anyRealAllyAlive || allies.Count == 0)
            {
                if (anyRealAllyAlive && allies.Count > 0)
                {
                    currentState = BattleState.EnemyTurn;
                    activeunitindex = 0;
                    enemyIndex = 0;
                    enemyTurnTimer = 0f;
                    System.Diagnostics.Debug.WriteLine("Oyuncu turu bitti. SIRA DÜŞMANDA!");
                }
                else
                {
                    currentState = BattleState.GameOver;
                    System.Diagnostics.Debug.WriteLine("Tüm müttefikler öldü! GAME OVER!");
                }
            }

            if (activeunitindex >= allies.Count && allies.Count > 0) activeunitindex = allies.Count - 1;
            if (activeunitindex < 0) activeunitindex = 0;

            bool currentIsFriendlyTarget = isskillselected && (
                (activeunitindex == 1 && (selectedskillind == 1 || selectedskillind == 2)) ||
                (activeunitindex == 2 && selectedskillind == 2)
            );

            if (currentIsFriendlyTarget)
            {
                selectedtargetind = MathHelper.Clamp(selectedtargetind, 0, Math.Max(0, allies.Count - 1));
            }
            else
            {
                if (enemies.Count > 0)
                {
                    selectedtargetind = MathHelper.Clamp(selectedtargetind, 0, enemies.Count - 1);
                }
                else
                {
                    selectedtargetind = 0;
                }
            }
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
        int bX = (int)position.X;
        int bY = (int)position.Y;
        int bWidth = 50;
        int bHeight = 6;

        _spriteBatch.Draw(pixel, new Rectangle(bX - 1, bY - 1, bWidth + 2, bHeight + 2), Color.Black);

        _spriteBatch.Draw(pixel, new Rectangle(bX, bY, bWidth, bHeight), Color.Red);

        float healthPercent = (float)currentHP / maxHP;
        _spriteBatch.Draw(pixel, new Rectangle(bX, bY, (int)(bWidth * healthPercent), bHeight), Color.LimeGreen);

        string hpText = $"{currentHP}/{maxHP}";
        Vector2 textPos = new Vector2(bX + 55, bY - 2);

        _spriteBatch.DrawString(gameFont, hpText, new Vector2(textPos.X + 1, textPos.Y + 1), Color.Black, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 0);
        _spriteBatch.DrawString(gameFont, hpText, textPos, Color.White, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 0);
    }

    private void DrawManaBar(Vector2 position, int currentMP, int maxMP)
    {
        int bX = (int)position.X;
        int bY = (int)position.Y + 12;
        int bWidth = 50;
        int bHeight = 4;

        _spriteBatch.Draw(pixel, new Rectangle(bX - 1, bY - 1, bWidth + 2, bHeight + 2), Color.Black);

        _spriteBatch.Draw(pixel, new Rectangle(bX, bY, bWidth, bHeight), Color.Black * 0.5f);

        float manaPercent = (float)currentMP / maxMP;
        _spriteBatch.Draw(pixel, new Rectangle(bX, bY, (int)(bWidth * manaPercent), bHeight), Color.Cyan);

        string mpText = $"{currentMP}/{maxMP}";
        Vector2 textPos = new Vector2(bX + 55, bY - 4);

        _spriteBatch.DrawString(gameFont, mpText, new Vector2(textPos.X + 1, textPos.Y + 1), Color.Black, 0, Vector2.Zero, 0.55f, SpriteEffects.None, 0);
        _spriteBatch.DrawString(gameFont, mpText, textPos, Color.DeepSkyBlue, 0, Vector2.Zero, 0.55f, SpriteEffects.None, 0);
    }

    private void ResetWholeGame()
    {
        currentWave = 1;
        activeunitindex = 0;
        enemyIndex = 0;
        selectedskillind = 0;
        selectedtargetind = 0;
        isskillselected = false;
        isRenatoUltimateUnlocked = false;
        isYaserUltimateUnlocked = false;
        isLeroyUltimateUnlocked = false;

        allies.Clear();
        enemies.Clear();

        Initialize();
    }
    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        _spriteBatch.Begin();

        if (currentState == BattleState.MainMenu)
        {
            if (battlegrounds.Count > 0)
            {
                Rectangle screenBounds = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
                _spriteBatch.Draw(battlegrounds[0], screenBounds, Color.White * 0.5f);
            }

            string titleText = "TURN BASED RPG";
            Vector2 titleSize = gameFont.MeasureString(titleText) * 2.0f;
            Vector2 titlePos = new Vector2(GraphicsDevice.Viewport.Width / 2 - titleSize.X / 2, 80);
            _spriteBatch.DrawString(gameFont, titleText, titlePos + new Vector2(2, 2), Color.Black, 0f, Vector2.Zero, 2.0f, SpriteEffects.None, 0f);
            _spriteBatch.DrawString(gameFont, titleText, titlePos, Color.Gold, 0f, Vector2.Zero, 2.0f, SpriteEffects.None, 0f);

            string playText = "PLAY GAME";
            Vector2 playSize = gameFont.MeasureString(playText) * 1.2f;
            Vector2 playPos = new Vector2(GraphicsDevice.Viewport.Width / 2 - playSize.X / 2, 220);
            _spriteBatch.DrawString(gameFont, playText, playPos + new Vector2(1, 1), Color.Black, 0f, Vector2.Zero, 1.2f, SpriteEffects.None, 0f);
            _spriteBatch.DrawString(gameFont, playText, playPos, (menuIndex == 0) ? Color.Gold : Color.White, 0f, Vector2.Zero, 1.2f, SpriteEffects.None, 0f);

            string exitText = "EXIT TO DESKTOP";
            Vector2 exitSize = gameFont.MeasureString(exitText) * 1.2f;
            Vector2 exitPos = new Vector2(GraphicsDevice.Viewport.Width / 2 - exitSize.X / 2, 280);
            _spriteBatch.DrawString(gameFont, exitText, exitPos + new Vector2(1, 1), Color.Black, 0f, Vector2.Zero, 1.2f, SpriteEffects.None, 0f);
            _spriteBatch.DrawString(gameFont, exitText, exitPos, (menuIndex == 1) ? Color.Gold : Color.White, 0f, Vector2.Zero, 1.2f, SpriteEffects.None, 0f);

            Vector2 arrowPos = (menuIndex == 0) ? new Vector2(playPos.X - 30, playPos.Y) : new Vector2(exitPos.X - 30, exitPos.Y);
            _spriteBatch.DrawString(gameFont, ">", arrowPos, Color.Gold, 0f, Vector2.Zero, 1.2f, SpriteEffects.None, 0f);

            _spriteBatch.End();
            return;
        }





        int bgIndex = currentWave - 1;
        if (bgIndex >= battlegrounds.Count)
        {
            bgIndex = battlegrounds.Count - 1;
        }

        if (battlegrounds.Count > 0 && bgIndex >= 0)
        {
            Rectangle screenBounds = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            _spriteBatch.Draw(battlegrounds[bgIndex], screenBounds, Color.White);
        }

        for (int i = 0; i < allies.Count; i++)
        {
            bool isHealing = (activeunitindex == 1 && (selectedskillind == 1 || selectedskillind == 2)) ||
                             (activeunitindex == 2 && selectedskillind == 2);

            bool isTargeted = (isskillselected && isHealing && selectedtargetind == i);

            Color c = isTargeted ? Color.Yellow : (i == activeunitindex ? Color.Green : Color.Blue);

            allies[i].Draw(_spriteBatch, c, gameFont);

            int barX = 0;
            int barY = 240;

            if (allies[i].Name == "Yaser") barX = 75;
            if (allies[i].Name == "Renato") barX = 210;
            if (allies[i].Name == "Leroy") barX = 315;

            Vector2 uiPos = new Vector2(barX, barY);

            string name = allies[i].Name;
            Vector2 nameSize = gameFont.MeasureString(name);
            Vector2 namePos = new Vector2(uiPos.X + (50 / 2) - (nameSize.X / 2), uiPos.Y - 20);

            Color nameColor;

            if (isTargeted)
            {
                nameColor = Color.LimeGreen;
            }
            else if (i == activeunitindex)
            {
                nameColor = Color.Gold;
            }
            else
            {
                nameColor = Color.White;
            }

            _spriteBatch.DrawString(gameFont, name, new Vector2(namePos.X + 1, namePos.Y + 1), Color.Black);
            _spriteBatch.DrawString(gameFont, name, namePos, nameColor);

            DrawHealthBar(uiPos, allies[i].CurrentHP, allies[i].MaxHP);
            DrawManaBar(uiPos, allies[i].CurrentMP, allies[i].MaxMP);
        }

        for (int i = 0; i < enemies.Count; i++)
        {
            bool isHealing = (activeunitindex == 1 && (selectedskillind == 1 || selectedskillind == 2)) ||
                             (activeunitindex == 2 && selectedskillind == 2);

            bool isTargeted = (isskillselected && !isHealing && selectedtargetind == i);

            Color c = isTargeted ? Color.Yellow : Color.Red;
            enemies[i].Draw(_spriteBatch, c, gameFont);

            int enemyUiX = 500 + (enemies[i].UIIndex * 80);
            int enemyUiY = 230;

            string enemyName = enemies[i].Name;
            Vector2 enemyNameSize = gameFont.MeasureString(enemyName);
            Vector2 enemyNamePos = new Vector2(enemyUiX + (50 / 2) - (enemyNameSize.X / 2), enemyUiY - 10);

            Color enemyNameColor = isTargeted ? Color.Yellow : Color.IndianRed;
            _spriteBatch.DrawString(gameFont, enemyName, new Vector2(enemyNamePos.X + 1, enemyNamePos.Y + 1), Color.Black);
            _spriteBatch.DrawString(gameFont, enemyName, enemyNamePos, enemyNameColor);

            DrawHealthBar(new Vector2(enemyUiX, enemyUiY + 20), enemies[i].CurrentHP, enemies[i].MaxHP);
        }

        if (currentState == BattleState.PlayerTurn)
        {
            if (activeunitindex >= 0 && activeunitindex < allies.Count)
            {
                var currentPlayer = allies[activeunitindex];
                List<SkillInfo> currentSkills = new List<SkillInfo>();

                if (currentPlayer.Name == "Leroy")
                {
                    currentSkills.Add(new SkillInfo("1. Sword Attack", $"{currentPlayer.Attackpower} DMG", "0 MP", "Gains 10 MP."));
                    currentSkills.Add(new SkillInfo("2. Double Trouble", $"{currentPlayer.Attackpower * 2} DMG", "15 MP", "Deal 2x DMG, take 2x DMG."));
                    currentSkills.Add(new SkillInfo("3. Sword Rain", $"{currentPlayer.Attackpower} AOE", "10 MP", "Blasts all enemies with a sword Rain."));
                    currentSkills.Add(new SkillInfo("4. Guardian Stance", "0 DMG", "10 MP", "Enters stance. Cuts incoming DMG by 4."));
                }
                else if (currentPlayer.Name == "Renato")
                {
                    currentSkills.Add(new SkillInfo("1. Quick Slash", $"{currentPlayer.Attackpower} DMG", "0 MP", "Gains 10 MP"));
                    currentSkills.Add(new SkillInfo("2. Holy Heal", "30 HEAL", "15 MP", "Restores HP to an ally."));
                    currentSkills.Add(new SkillInfo("3. Moonlight Power", "0 DMG", "20 MP", "Gives ATK Power buff to all allies."));
                    currentSkills.Add(new SkillInfo("4. Final Sacrifice", "INSTANT KILL", "30 MP", "Kills single target, but Sacrifices himself."));
                }
                else if (currentPlayer.Name == "Yaser")
                {
                    currentSkills.Add(new SkillInfo("1. Basic Attack", $"{currentPlayer.Attackpower} DMG", "0 MP", "Gains 10 MP on hit."));
                    currentSkills.Add(new SkillInfo("2. Chain Lightning", "30 SPLIT", "10 MP", "Deals 30 DMG split among all living enemies."));
                    currentSkills.Add(new SkillInfo("3. Mana Battery", "+30 MANA", "0 MP", "Gives 30 MP to an ally. Cost: 0 MP."));
                    currentSkills.Add(new SkillInfo("4. Void Apocalypse", "ALL MP DMG", "30 MP", "Consumes ALL MP. Deals DMG equal to spent MP."));
                }

                int menuWidth = 400;
                int menuHeight = currentSkills.Count * 45 + 10;

                int menuX = (int)currentPlayer.Position.X + 25 - (menuWidth / 2);
                int menuY = (int)currentPlayer.Position.Y - menuHeight - 60;

                if (menuX < 10) menuX = 10;
                if (menuY < 10) menuY = 10;

                _spriteBatch.Draw(pixel, new Rectangle(menuX - 2, menuY - 2, menuWidth + 4, menuHeight + 4), Color.Black);
                _spriteBatch.Draw(pixel, new Rectangle(menuX, menuY, menuWidth, menuHeight), Color.Black * 0.8f);

                for (int i = 0; i < currentSkills.Count; i++)
                {
                    bool isSelected = (selectedskillind == i);
                    Color titleColor = isSelected ? Color.Gold : Color.White;
                    Color boxColor = isSelected ? Color.Gold * 0.25f : Color.Transparent;

                    int slotX = menuX + 15;
                    int slotY = menuY + 8 + (i * 45);

                    if (isSelected)
                    {
                        _spriteBatch.Draw(pixel, new Rectangle(menuX + 4, slotY - 4, menuWidth - 8, 40), boxColor);
                        _spriteBatch.Draw(pixel, new Rectangle(menuX + 4, slotY - 4, 3, 40), Color.Gold);
                    }

                    string skillTitle = currentSkills[i].Name;
                    string statsText = $" [{currentSkills[i].DamageText}] [{currentSkills[i].CostText}]";

                    _spriteBatch.DrawString(gameFont, skillTitle, new Vector2(slotX + 1, slotY + 1), Color.Black);
                    _spriteBatch.DrawString(gameFont, skillTitle, new Vector2(slotX, slotY), titleColor);

                    Vector2 titleSize = gameFont.MeasureString(skillTitle);
                    _spriteBatch.DrawString(gameFont, statsText, new Vector2(slotX + titleSize.X + 11, slotY + 1), Color.Black);
                    _spriteBatch.DrawString(gameFont, statsText, new Vector2(slotX + titleSize.X + 10, slotY), isSelected ? Color.Cyan : Color.LightGray);

                    string desc = currentSkills[i].Description;
                    _spriteBatch.DrawString(gameFont, desc, new Vector2(slotX + 1, slotY + 21), Color.Black, 0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0f);
                    _spriteBatch.DrawString(gameFont, desc, new Vector2(slotX, slotY + 20), Color.DarkGray, 0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0f);
                }
            }
        }

        if (currentState == BattleState.GameOver)
        {
            Rectangle screenBounds = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            _spriteBatch.Draw(pixel, screenBounds, Color.Black * 0.65f);
            _spriteBatch.Draw(pixel, screenBounds, Color.Red * 0.08f);

            string goText = "GAME OVER";
            Vector2 goSize = gameFont.MeasureString(goText) * 2.5f;
            Vector2 goPos = new Vector2(GraphicsDevice.Viewport.Width / 2 - goSize.X / 2, GraphicsDevice.Viewport.Height / 2 - goSize.Y / 2 - 40);
            _spriteBatch.DrawString(gameFont, goText, goPos + new Vector2(3, 3), Color.Black, 0f, Vector2.Zero, 2.5f, SpriteEffects.None, 0f);
            _spriteBatch.DrawString(gameFont, goText, goPos, Color.Crimson, 0f, Vector2.Zero, 2.5f, SpriteEffects.None, 0f);

            string retryText = "[R] Retry the Battle";
            string mainMenuText = "[M] Return to Main Menu";

            Vector2 rSize = gameFont.MeasureString(retryText) * 1.0f;
            Vector2 mSize = gameFont.MeasureString(mainMenuText) * 1.0f;

            Vector2 rPos = new Vector2(GraphicsDevice.Viewport.Width / 2 - rSize.X / 2, goPos.Y + goSize.Y + 20);
            Vector2 mPos = new Vector2(GraphicsDevice.Viewport.Width / 2 - mSize.X / 2, rPos.Y + 35);

            _spriteBatch.DrawString(gameFont, retryText, rPos + new Vector2(1, 1), Color.Black, 0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0f);
            _spriteBatch.DrawString(gameFont, retryText, rPos, Color.LightGray, 0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0f);

            _spriteBatch.DrawString(gameFont, mainMenuText, mPos + new Vector2(1, 1), Color.Black, 0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0f);
            _spriteBatch.DrawString(gameFont, mainMenuText, mPos, Color.LightGray, 0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0f);
        }



        if (currentState == BattleState.Victory)
        {
            Rectangle screenBounds = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

            _spriteBatch.Draw(pixel, screenBounds, Color.Black * 0.6f);
            _spriteBatch.Draw(pixel, screenBounds, Color.Gold * 0.05f);

            string vicText = "VICTORY";
            Vector2 vicSize = gameFont.MeasureString(vicText) * 2.5f;
            Vector2 vicPos = new Vector2(GraphicsDevice.Viewport.Width / 2 - vicSize.X / 2, GraphicsDevice.Viewport.Height / 2 - vicSize.Y / 2 - 30);

            _spriteBatch.DrawString(gameFont, vicText, new Vector2(vicPos.X + 3, vicPos.Y + 3), Color.Black, 0f, Vector2.Zero, 2.5f, SpriteEffects.None, 0f);
            _spriteBatch.DrawString(gameFont, vicText, vicPos, Color.Gold, 0f, Vector2.Zero, 2.5f, SpriteEffects.None, 0f);

            string congraText = "You have cleansed the battlefield!";
            Vector2 congraSize = gameFont.MeasureString(congraText) * 0.9f;
            Vector2 congraPos = new Vector2(GraphicsDevice.Viewport.Width / 2 - congraSize.X / 2, vicPos.Y + vicSize.Y + 20);

            _spriteBatch.DrawString(gameFont, congraText, new Vector2(congraPos.X + 1, congraPos.Y + 1), Color.Black, 0f, Vector2.Zero, 0.9f, SpriteEffects.None, 0f);
            _spriteBatch.DrawString(gameFont, congraText, congraPos, Color.White, 0f, Vector2.Zero, 0.9f, SpriteEffects.None, 0f);
        }

        _spriteBatch.End();
        base.Draw(gameTime);
    }
}
