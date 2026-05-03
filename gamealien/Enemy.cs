using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SharpDX.DXGI;

public class Enemy : BaseCharacter
{
    public Enemy(string name, int hp, int atk, Vector2 pos) : base(name, hp, atk, pos) { }

    public void TakeTurn(List<Player> allies, List<Enemy> enemies)
    {
        System.Random rnd = new System.Random();

        int targetIndex = rnd.Next(0, allies.Count);

        if (rnd.Next(1, 101) <= 30)
        {
            ExecuteSkill(0, allies, enemies, targetIndex, false);
        }
        else
        {
            allies[targetIndex].TakeDamage(Attackpower);

        }
    }
}


public class Vampire : Enemy
{
    public Vampire(string name, int hp, int atk, Vector2 pos) : base(name, hp, atk, pos) { }

    public override bool ExecuteSkill(int skillIndex, List<Player> allies, List<Enemy> enemies, int targetIndex, bool isultimateunlocked)
    {
        allies[targetIndex].TakeDamage(Attackpower);
        this.Heal(10);
        System.Diagnostics.Debug.WriteLine("Vampire bit and healed itself!");
        return true;
    }
}

public class Skeleton : Enemy
{
    public Skeleton(string name, int hp, int atk, Vector2 pos) : base(name, hp, atk, pos) { }

    public override bool ExecuteSkill(int skillIndex, List<Player> allies, List<Enemy> enemies, int targetIndex, bool isultimateunlocked)
    {
        allies[targetIndex].TakeDamage(Attackpower + 10);
        System.Diagnostics.Debug.WriteLine("Skeleton kemik fırlattı!");
        return true;
    }
}