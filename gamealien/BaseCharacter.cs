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
    public Texture2D spriteIdle;
    public Texture2D spriteAttack;
    public Texture2D spriteDefend;
    public Texture2D spriteHurt;
    public Texture2D spriteDead;

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
        Dead
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

        switch (CurrentState)
        {
            case CharacterState.Idle: currentTexture = spriteIdle; break;
            case CharacterState.Attack: currentTexture = spriteAttack; break;
            case CharacterState.Defend: currentTexture = spriteDefend; break;
            case CharacterState.Hurt: currentTexture = spriteHurt; break;
            case CharacterState.Dead: currentTexture = spriteDead; break;
        }

        if (currentTexture != null)
        {
            int frameCount = 4;
            if (CurrentState == CharacterState.Attack) frameCount = 5;
            else if (CurrentState == CharacterState.Defend) frameCount = 5;
            else if (CurrentState == CharacterState.Dead) frameCount = 6;
            else if (CurrentState == CharacterState.Hurt) frameCount = 2;

            animationTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (animationTimer >= FrameTime)
            {
                animationTimer = 0f;
                currentFrame++;

                if (CurrentState == CharacterState.Hurt && currentFrame >= frameCount)
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

            if (currentFrame >= frameCount) currentFrame = 0;
        }
    }

    public virtual void Draw(SpriteBatch spriteBatch, Color characterColor, SpriteFont font)
    {
        Texture2D currentTexture = spriteIdle;

        switch (CurrentState)
        {
            case CharacterState.Idle: currentTexture = spriteIdle; break;
            case CharacterState.Attack: currentTexture = spriteAttack; break;
            case CharacterState.Defend: currentTexture = spriteDefend; break;
            case CharacterState.Hurt: currentTexture = spriteHurt; break;
            case CharacterState.Dead: currentTexture = spriteDead; break;
        }

        if (currentTexture != null)
        {
            int frameCount = 4;
            if (CurrentState == CharacterState.Attack) frameCount = 5;
            else if (CurrentState == CharacterState.Defend) frameCount = 5;
            else if (CurrentState == CharacterState.Dead) frameCount = 6;
            else if (CurrentState == CharacterState.Hurt) frameCount = 2;

            int frameWidth = currentTexture.Width / frameCount;
            int frameHeight = currentTexture.Height;

            if (currentFrame >= frameCount) currentFrame = 0;

            int xOffset = 0;


            if (CurrentState == CharacterState.Idle)
            {
                switch (currentFrame)
                {
                    case 0: xOffset = 0; break;
                    case 1: xOffset = -4; break;
                    case 2: xOffset = -9; break;
                    case 3: xOffset = -14; break;
                }
            }
            else if (CurrentState == CharacterState.Attack)
            {
                switch (currentFrame)
                {
                    case 0: xOffset = 0; break;
                    case 1: xOffset = -4; break;
                    case 2: xOffset = -8; break;
                    case 3: xOffset = -12; break;
                    case 4: xOffset = -16; break;
                }
            }
            else if (CurrentState == CharacterState.Defend)
            {
                switch (currentFrame)
                {
                    case 0: xOffset = 0; break;
                    case 1: xOffset = -3; break;
                    case 2: xOffset = -6; break;
                    case 3: xOffset = -9; break;
                    case 4: xOffset = -12; break;
                }
            }
            else if (CurrentState == CharacterState.Hurt)
            {
                switch (currentFrame)
                {
                    case 0: xOffset = 0; break;
                    case 1: xOffset = -4; break;
                }
            }

            int currentX = (currentFrame * frameWidth) + xOffset;

            if (currentX < 0) currentX = 0;
            if (currentX + frameWidth > currentTexture.Width) currentX = currentTexture.Width - frameWidth;

            Rectangle sourceRect = new Rectangle(currentX, 0, frameWidth, frameHeight);

            Rectangle destRect = new Rectangle((int)Position.X, (int)Position.Y, frameWidth, frameHeight);

            spriteBatch.Draw(currentTexture, destRect, sourceRect, Color.White);

            int lastFrameWidth = frameWidth;
        }
        else
        {
            if (Sprite != null)
            {
                spriteBatch.Draw(Sprite, new Rectangle((int)Position.X, (int)Position.Y, 50, 50), characterColor);
            }
        }

        if (font != null && !string.IsNullOrEmpty(Name))
        {
            int finalWidth = (currentTexture != null) ? (currentTexture.Width / (CurrentState == CharacterState.Attack || CurrentState == CharacterState.Defend ? 5 : (CurrentState == CharacterState.Dead ? 6 : (CurrentState == CharacterState.Hurt ? 2 : 4)))) : 50;
            Vector2 nameSize = font.MeasureString(Name);
            Vector2 textPosition = new Vector2(Position.X + (finalWidth / 2) - (nameSize.X / 2), Position.Y - 25);
            spriteBatch.DrawString(font, Name, textPosition, Color.White);
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