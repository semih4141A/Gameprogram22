using Microsoft.Xna.Framework;

public class Player : BaseCharacter
{
    public int MaxMP { get; set; }
    public int CurrentMP { get; set; }

    public Player(string name, int hp, int mp, int atk, Vector2 pos) :
     base(name, hp, atk, pos)
    {
        MaxMP = mp;
        CurrentMP = mp;
    }
}