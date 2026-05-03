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

    public BaseCharacter(string name, int hp, int atk, Vector2 pos)
    {
        Name = name;
        MaxHP = hp;
        CurrentHP = hp;
        Attackpower = atk;
        Position = pos;
    }

    public virtual void Draw(SpriteBatch spriteBatch, Color characterColor, SpriteFont font)
    {
        Rectangle destRect = new Rectangle((int)Position.X, (int)Position.Y, 50, 50);
        spriteBatch.Draw(Sprite, destRect, characterColor);

        Vector2 nameSize = font.MeasureString(Name);

        Vector2 textPosition = new Vector2(
            Position.X + (50 / 2) - (nameSize.X / 2),
            Position.Y + (50 / 2) - (nameSize.Y / 2)
        );

        spriteBatch.DrawString(font, Name, textPosition, Color.White);
    }

    public virtual void TakeDamage(int damage)
    {
        CurrentHP -= damage;
        if (CurrentHP < 0) CurrentHP = 0;
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