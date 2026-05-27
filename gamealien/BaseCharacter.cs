using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class BaseCharacter
{
    public string Name { get; set; }
    public int MaxHP { get; set; }
    public int CurrentHP { get; set; }
    public int Attackpower { get; set; }
    public Vector2 Position { get; set; }
    public Texture2D Sprite { get; set; }

    public float animationTimer = 0f;
    public int currentFrame = 0;
    public float FrameTime { get; set; } = 0.15f;

    public int frameCountIdle = 4;
    public int frameCountAttack = 5;
    public int frameCountDefend = 5;
    public int frameCountHurt = 2;
    public int frameCountDead = 6;
    public int frameCountUlt = 5;

    public int idleColumns = 2, idleRows = 4;
    public int attackColumns = 8, attackRows = 5;
    public int defendColumns = 5, defendRows = 1;
    public int hurtColumns = 2, hurtRows = 2;
    public int deadColumns = 2, deadRows = 3;
    public int ultColumns = 2, ultRows = 4;
    public Texture2D spriteIdle;
    public Texture2D spriteAttack;
    public Texture2D spriteDefend;
    public Texture2D spriteHurt;
    public Texture2D spriteDead;

    public Texture2D spriteCrouchAttack;
    public Texture2D spriteHeal;
    public Texture2D spritePowerup;

    public Texture2D spriteYaserIdle;
    public Texture2D spriteYaserAttack;
    public Texture2D spriteYaserHurt;
    public Texture2D spriteYaserDead;
    public Texture2D spriteYaserUlt;

    public BaseCharacter(string name, int hp, int atk, Vector2 pos)
    {
        Name = name;
        MaxHP = hp;
        CurrentHP = hp;
        Attackpower = atk;
        Position = pos;
    }

    public enum CharacterState
    {
        Idle,
        Attack,
        Defend,
        Hurt,
        Dead,

        Ult,
        Heal,
        Powerup
    }

    private CharacterState currentState = CharacterState.Idle;
    public CharacterState CurrentState
    {
        get { return currentState; }
        set
        {
            if (currentState != value)
            {
                currentState = value;
                currentFrame = 0;
                animationTimer = 0f;
            }
        }
    }

    public virtual void UpdateAnimation(GameTime gameTime)
    {
        Texture2D currentTexture = spriteIdle;

        if (Name == "Yaser")
        {
            switch (CurrentState)
            {
                case CharacterState.Idle: currentTexture = spriteYaserIdle; break;
                case CharacterState.Attack: currentTexture = spriteYaserAttack; break;
                case CharacterState.Hurt: currentTexture = spriteYaserHurt; break;
                case CharacterState.Dead: currentTexture = spriteYaserDead; break;
                case CharacterState.Ult: currentTexture = spriteYaserUlt; break;
                default: currentTexture = spriteYaserIdle; break;
            }
        }
        else
        {
            switch (CurrentState)
            {
                case CharacterState.Idle: currentTexture = spriteIdle; break;
                case CharacterState.Attack: currentTexture = spriteAttack; break;
                case CharacterState.Defend: currentTexture = spriteDefend; break;
                case CharacterState.Hurt: currentTexture = spriteHurt; break;
                case CharacterState.Dead: currentTexture = spriteDead; break;
                case CharacterState.Ult: currentTexture = spriteCrouchAttack; break;
                case CharacterState.Heal: currentTexture = spriteHeal; break;
                case CharacterState.Powerup: currentTexture = spritePowerup; break;
            }
        }

        if (currentTexture != null)
        {
            int frameCount = 4;
            switch (CurrentState)
            {
                case CharacterState.Idle: frameCount = frameCountIdle; break;
                case CharacterState.Attack: frameCount = frameCountAttack; break;
                case CharacterState.Defend: frameCount = frameCountDefend; break;
                case CharacterState.Hurt: frameCount = frameCountHurt; break;
                case CharacterState.Dead: frameCount = frameCountDead; break;
                case CharacterState.Ult: frameCount = frameCountUlt; break;
                case CharacterState.Heal: frameCount = 12; break;
                case CharacterState.Powerup: frameCount = 7; break;
            }

            if (Name == "Yaser")
            {
                switch (CurrentState)
                {
                    case CharacterState.Idle: frameCount = 6; break;
                    case CharacterState.Attack: frameCount = 8; break;
                    case CharacterState.Hurt: frameCount = 4; break;
                    case CharacterState.Dead: frameCount = 7; break;
                    case CharacterState.Ult: frameCount = 8; break;
                }
            }

            if (Name == "Renato" && CurrentState == CharacterState.Attack)
            {
                frameCount = 8;
            }

            animationTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (animationTimer >= FrameTime)
            {
                animationTimer = 0f;
                currentFrame++;

                if (Name == "Yaser")
                {
                    if ((CurrentState == CharacterState.Attack || CurrentState == CharacterState.Hurt) && currentFrame >= frameCount)
                    {
                        CurrentState = CharacterState.Idle;
                        currentFrame = 0;
                    }
                    else if (CurrentState == CharacterState.Ult && currentFrame >= frameCount)
                    {
                        CurrentState = CharacterState.Idle;
                        currentFrame = 0;
                    }
                    else if (CurrentState == CharacterState.Dead && currentFrame >= frameCount)
                    {
                        if (currentFrame >= frameCount)
                        {
                            currentFrame = frameCount - 1;
                        }
                    }
                }



                else
                {
                    if (CurrentState == CharacterState.Heal && currentFrame >= frameCount)
                    {
                        CurrentState = CharacterState.Idle;
                        currentFrame = 0;
                    }
                    else if (CurrentState == CharacterState.Powerup && currentFrame >= frameCount)
                    {
                        CurrentState = CharacterState.Idle;
                        currentFrame = 0;
                    }
                    else if (CurrentState == CharacterState.Ult && currentFrame >= frameCount)
                    {
                        CurrentState = CharacterState.Dead;
                        currentFrame = 0;
                    }
                    else if (CurrentState == CharacterState.Hurt && currentFrame >= frameCount)
                    {
                        CurrentState = CharacterState.Idle;
                        currentFrame = 0;
                    }
                    else if (CurrentState == CharacterState.Attack && currentFrame >= frameCount)
                    {
                        CurrentState = CharacterState.Idle;
                        currentFrame = 0;
                    }
                    else if (CurrentState == CharacterState.Dead)
                    {
                        if (currentFrame >= frameCount)
                        {
                            currentFrame = frameCount - 1;
                        }
                    }
                    else
                    {
                        currentFrame %= frameCount;
                    }
                }
            }

        }
    }
    public virtual void Draw(SpriteBatch spriteBatch, Color characterColor, SpriteFont font)
    {
        Texture2D currentTexture = spriteIdle;

        if (Name == "Yaser")
        {
            switch (CurrentState)
            {
                case CharacterState.Idle: currentTexture = spriteYaserIdle; break;
                case CharacterState.Attack: currentTexture = spriteYaserAttack; break;
                case CharacterState.Hurt: currentTexture = spriteYaserHurt; break;
                case CharacterState.Dead: currentTexture = spriteYaserDead; break;
                case CharacterState.Ult: currentTexture = spriteYaserUlt; break;
                default: currentTexture = spriteYaserIdle; break;
            }
        }
        else
        {
            switch (CurrentState)
            {
                case CharacterState.Idle: currentTexture = spriteIdle; break;
                case CharacterState.Attack: currentTexture = spriteAttack; break;
                case CharacterState.Defend: currentTexture = spriteDefend; break;
                case CharacterState.Hurt: currentTexture = spriteHurt; break;
                case CharacterState.Dead: currentTexture = spriteDead; break;
                case CharacterState.Ult: currentTexture = spriteCrouchAttack; break;
                case CharacterState.Heal: currentTexture = spriteHeal; break;
                case CharacterState.Powerup: currentTexture = spritePowerup; break;
            }
        }

        if (currentTexture != null)
        {
            int columns = 4, rows = 1;

            if (Name == "Leroy")
            {
                switch (CurrentState)
                {
                    case CharacterState.Idle: columns = 4; rows = 1; break;
                    case CharacterState.Attack: columns = 5; rows = 1; break;
                    case CharacterState.Defend: columns = 5; rows = 1; break;
                    case CharacterState.Hurt: columns = 2; rows = 1; break;
                    case CharacterState.Dead: columns = 6; rows = 1; break;
                }
            }
            else if (Name == "Renato")
            {
                switch (CurrentState)
                {
                    case CharacterState.Idle: columns = idleColumns; rows = idleRows; break;
                    case CharacterState.Attack: columns = attackColumns; rows = attackRows; break;
                    case CharacterState.Defend: columns = defendColumns; rows = defendRows; break;
                    case CharacterState.Hurt: columns = hurtColumns; rows = hurtRows; break;
                    case CharacterState.Dead: columns = 2; rows = 2; break;
                    case CharacterState.Ult: columns = ultColumns; rows = ultRows; break;
                    case CharacterState.Heal: columns = 4; rows = 3; break;
                    case CharacterState.Powerup: columns = 2; rows = 4; break;
                }
            }
            else if (Name == "Yaser")
            {
                rows = 1;
                switch (CurrentState)
                {
                    case CharacterState.Idle: columns = 6; break;
                    case CharacterState.Attack: columns = 8; break;
                    case CharacterState.Hurt: columns = 4; break;
                    case CharacterState.Dead: columns = 7; break;
                    case CharacterState.Ult: columns = 8; break;
                    default: columns = 6; break;
                }
            }

            int frameWidth = currentTexture.Width / columns;
            int frameHeight = currentTexture.Height / rows;

            int totalFrames = columns * rows;
            if (currentFrame >= totalFrames) currentFrame = 0;

            int currentRow = currentFrame / columns;
            int currentCol = currentFrame % columns;

            int xOffset = 0;
            if (Name == "Leroy" && CurrentState == CharacterState.Idle)
            {
                switch (currentFrame)
                {
                    case 1: xOffset = -4; break;
                    case 2: xOffset = -9; break;
                    case 3: xOffset = -14; break;
                }
            }

            int currentX = (currentCol * frameWidth) + xOffset;
            int currentY = currentRow * frameHeight;


            if (currentX < 0) currentX = 0;
            if (currentX + frameWidth > currentTexture.Width) currentX = currentTexture.Width - frameWidth;
            if (currentY + frameHeight > currentTexture.Height) currentY = currentTexture.Height - frameHeight;

            Rectangle sourceRect = new Rectangle(currentX, currentY, frameWidth, frameHeight);
            int finalRenderWidth = frameWidth;
            int finalRenderHeight = frameHeight;

            if (Name == "Renato")
            {
                finalRenderWidth = (int)(frameWidth * 1.35f);
                finalRenderHeight = (int)(frameHeight * 1.35f);
            }

            else if (Name == "Yaser")
            {
                finalRenderWidth = frameWidth;
                finalRenderHeight = frameHeight;
            }

            Rectangle destRect = new Rectangle((int)Position.X, (int)Position.Y, finalRenderWidth, finalRenderHeight);
            spriteBatch.Draw(currentTexture, destRect, sourceRect, Color.White);
        }
    }
    public virtual void TakeDamage(int damage)
    {
        CurrentHP -= damage;
        if (CurrentHP <= 0)
        {
            CurrentHP = 0;
            CurrentState = CharacterState.Dead;
        }
        else
        {
            CurrentState = CharacterState.Hurt;
        }
    }

    public virtual void Heal(int amount)
    {
        CurrentHP += amount;
        if (CurrentHP > MaxHP) CurrentHP = MaxHP;
    }

    public virtual bool ExecuteSkill(int skillIndex, List<Player> allies, List<Enemy> enemies, int targetIndex, bool isultimateunlocked)
    {
        return false;

    }
}