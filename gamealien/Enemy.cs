using Microsoft.Xna.Framework;
using SharpDX.DXGI;

public abstract class Enemy : BaseCharacter
{
    public Enemy(string name, int hp, int atk, Vector2 pos) :
     base(name, hp, atk, pos)
    {
        
    }

    public abstract void Attack(Player target);


}