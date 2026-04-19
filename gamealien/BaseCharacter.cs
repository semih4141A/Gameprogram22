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

    public virtual void Draw(SpriteBatch spriteBatch, Color characterColor)
    {
        spriteBatch.Draw(Sprite, new Rectangle((int)Position.X, (int)Position.Y, 50, 50), characterColor);

    }
}