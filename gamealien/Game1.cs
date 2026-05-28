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
        allies.Add(new Player.Leroy("Leroy", 100, 60, 999, new Vector2(300, 215)));
        allies.Add(new Player.Renato("Renato", 70, 60, 15, new Vector2(150, 215)));
        allies.Add(new Player.Yaser("Yaser", 70, 60, 20, new Vector2(0, 160)));

        enemies.Add(new Vampire("Vampire", 60, 10, new Vector2(450, 175)));
        enemies.Add(new Skeleton("Skeleton", 40, 15, new Vector2(550, 200)));
        enemies.Add(new Vampire("Vampire", 60, 10, new Vector2(650, 175)));

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        gameFont = Content.Load<SpriteFont>("GameFont");

        pixel = new Texture2D(GraphicsDevice, 1, 1);
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



    }

    protected override void Update(GameTime gameTime)
    {
        var kstate = Keyboard.GetState();

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
                enemies.Add(new Bat("Bat", 80, 15, new Vector2(580, 200)));
                enemies.Add(new Zombie("Zombie", 60, 10, new Vector2(660, 200)));
            }
            else if (currentWave == 3)
            {
                enemies.Add(new EvilWizard("EvilWizard", 50, 20, new Vector2(500, 200)));
                enemies.Add(new Sorcerer("Sorcerer", 50, 5, new Vector2(580, 200)));
                enemies.Add(new EvilWizard("EvilWizard", 50, 20, new Vector2(660, 200)));
            }
            else if (currentWave == 4)
            {
                enemies.Add(new FinalBoss("FinalBoss", 200, 30, new Vector2(580, 200)));
                enemies.Add(new EvilWizard("EvilWizard", 50, 20, new Vector2(500, 200)));
                enemies.Add(new Sorcerer("Sorcerer", 50, 5, new Vector2(660, 200)));
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

            if (enemies.Count > 0)
            {
                if (selectedtargetind >= enemies.Count)
                {
                    selectedtargetind = enemies.Count - 1;
                }
            }
            else
            {
                selectedtargetind = 0;
            }
            if (selectedtargetind < 0) selectedtargetind = 0;
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

        _spriteBatch.Draw(pixel, new Rectangle(bX, bY, 50, 6), Color.Red);

        float healthPercent = (float)currentHP / maxHP;
        _spriteBatch.Draw(pixel, new Rectangle(bX, bY, (int)(50 * healthPercent), 6), Color.Green);

        string hpText = $"{currentHP}/{maxHP}";
        _spriteBatch.DrawString(gameFont, hpText, new Vector2(bX + 55, bY), Color.White, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 0);
    }

    private void DrawManaBar(Vector2 position, int currentMP, int maxMP)
    {
        int bX = (int)position.X;
        int bY = (int)position.Y + 10;

        _spriteBatch.Draw(pixel, new Rectangle(bX, bY, 50, 4), Color.Black * 0.5f);

        float manaPercent = (float)currentMP / maxMP;
        _spriteBatch.Draw(pixel, new Rectangle(bX, bY, (int)(50 * manaPercent), 4), Color.DeepSkyBlue);

        string mpText = $"{currentMP}/{maxMP}";
        _spriteBatch.DrawString(gameFont, mpText, new Vector2(bX + 55, bY - 4), Color.Cyan, 0, Vector2.Zero, 0.6f, SpriteEffects.None, 0);
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

            int barX = 0;
            int barY = 200;

            if (allies[i].Name == "Yaser") barX = 75;
            if (allies[i].Name == "Renato") barX = 210;
            if (allies[i].Name == "Leroy") barX = 315;

            Vector2 uiPos = new Vector2(barX, barY);

            string name = allies[i].Name;
            Vector2 nameSize = gameFont.MeasureString(name);
            Vector2 namePos = new Vector2(uiPos.X + (50 / 2) - (nameSize.X / 2), uiPos.Y - 20);
            _spriteBatch.DrawString(gameFont, name, namePos, Color.White);

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

            int enemyUiX = 500 + (i * 80);
            int enemyUiY = 190;

            string enemyName = enemies[i].Name;
            Vector2 enemyNameSize = gameFont.MeasureString(enemyName);
            Vector2 enemyNamePos = new Vector2(enemyUiX + (50 / 2) - (enemyNameSize.X / 2), enemyUiY - 10);
            _spriteBatch.DrawString(gameFont, enemyName, enemyNamePos, Color.White);

            DrawHealthBar(new Vector2(enemyUiX, enemyUiY + 20), enemies[i].CurrentHP, enemies[i].MaxHP);
        }

        if (currentState == BattleState.PlayerTurn)
        {
            if (activeunitindex < 0 || activeunitindex >= allies.Count)
            {
                _spriteBatch.End();
                return;
            }
            Vector2 pPos = allies[activeunitindex].Position;

            for (int i = 0; i < 4; i++)
            {
                Color boxColor = (selectedskillind == i) ? Color.Gold : Color.Gray * 0.6f;

                int offsetX = 0;
                int offsetY = -180;

                if (activeunitindex == 2)
                {
                    offsetX = 20;
                }
                else if (activeunitindex == 1)
                {
                    offsetX = 60;
                }
                else if (activeunitindex == 0)
                {
                    offsetX = 10;
                }

                Rectangle skillBox = new Rectangle((int)pPos.X + offsetX, (int)pPos.Y + offsetY + (i * 40), 50, 30);

                _spriteBatch.Draw(pixel, skillBox, boxColor);
            }

            if (isskillselected)
            {
                bool isFriendlyTarget = (activeunitindex == 1 && (selectedskillind == 1 || selectedskillind == 2)) ||
                                        (activeunitindex == 2 && selectedskillind == 2);


                if (isFriendlyTarget && selectedtargetind < allies.Count && selectedtargetind >= 0)
                {
                    Vector2 targetPos = allies[selectedtargetind].Position;
                    _spriteBatch.Draw(pixel, new Rectangle((int)targetPos.X, (int)targetPos.Y + 100, 40, 10), Color.Yellow);
                }
                else if (!isFriendlyTarget && selectedtargetind < enemies.Count && selectedtargetind >= 0)
                {
                    Vector2 targetPos = enemies[selectedtargetind].Position;
                    _spriteBatch.Draw(pixel, new Rectangle((int)targetPos.X, (int)targetPos.Y + 100, 40, 10), Color.Yellow);
                }
            }
        }

        _spriteBatch.End();
        base.Draw(gameTime);
    }
}
