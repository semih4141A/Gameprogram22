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

public class Barbarian : Enemy
{
    public Barbarian(string name, int hp, int atk, Vector2 pos) : base(name, hp, atk, pos) { }

    public override bool ExecuteSkill(int skillIndex, List<Player> allies, List<Enemy> enemies, int targetIndex, bool isultimateunlocked)
    {
        int damage = Attackpower;

        if (CurrentHP < (MaxHP / 2))
        {
            damage *= 2;
            System.Diagnostics.Debug.WriteLine($"{Name} is RAGING! Double damage!");
        }

        allies[targetIndex].TakeDamage(damage);
        return true;
    }
}

public class Beholder : Enemy
{
    public Beholder(string name, int hp, int atk, Vector2 pos) : base(name, hp, atk, pos) { }

    public override bool ExecuteSkill(int skillIndex, List<Player> allies, List<Enemy> enemies, int targetIndex, bool isultimateunlocked)
    {

        allies[targetIndex].TakeDamage(Attackpower);


        allies[targetIndex].CurrentMP -= 15;
        if (allies[targetIndex].CurrentMP < 0) allies[targetIndex].CurrentMP = 0;

        System.Diagnostics.Debug.WriteLine($"{Name} drained MP from {allies[targetIndex].Name}");
        return true;
    }
}

public class HealerGoblin : Enemy
{
    public HealerGoblin(string name, int hp, int atk, Vector2 pos) : base(name, hp, atk, pos) { }

    public override bool ExecuteSkill(int skillIndex, List<Player> allies, List<Enemy> enemies, int targetIndex, bool isultimateunlocked)
    {

        Enemy weakestAlly = enemies[0];
        foreach (var e in enemies)
        {
            if (e.CurrentHP < weakestAlly.CurrentHP) weakestAlly = e;
        }

        weakestAlly.Heal(20);
        System.Diagnostics.Debug.WriteLine($"{Name} protected his team and healed {weakestAlly.Name}!");
        return true;
    }
}

public class Zombie : Enemy
{
    public Zombie(string name, int hp, int atk, Vector2 pos) : base(name, hp, atk, pos) { }

    public override bool ExecuteSkill(int skillIndex, List<Player> allies, List<Enemy> enemies, int targetIndex, bool isultimateunlocked)
    {
        allies[targetIndex].TakeDamage(Attackpower);

        allies[targetIndex].Attackpower = allies[targetIndex].BaseAttackPower / 2;

        System.Diagnostics.Debug.WriteLine($"{allies[targetIndex].Name}'s attack is weakened by Shield Bash!");
        return true;
    }
}