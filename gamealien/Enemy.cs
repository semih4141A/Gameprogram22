using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.DXGI;

public class Enemy : BaseCharacter
{
    public Enemy(string name, int hp, int atk, Vector2 pos) : base(name, hp, atk, pos) { }

    public void TakeTurn(List<Player> allies, List<Enemy> enemies)
    {
        if (CurrentHP <= 0)
        {
            System.Diagnostics.Debug.WriteLine($"{Name} öldüğü için turn atlıyor.");
            return;
        }

        if (allies == null || allies.Count == 0)
        {
            System.Diagnostics.Debug.WriteLine("Saldıracak oyuncu kalmadı, yapay zeka turn atlıyor.");
            return;
        }
        System.Random rnd = new System.Random();
        this.CurrentState = CharacterState.Attack;
        this.currentFrame = 0;
        this.animationTimer = 0f;

        int targetIndex = rnd.Next(0, allies.Count);

        if (rnd.Next(1, 101) <= 30)
        {
            ExecuteSkill(0, allies, enemies, targetIndex, false);
        }
        else
        {
            allies[targetIndex].TakeDamage(Attackpower);
            System.Diagnostics.Debug.WriteLine($"{Name} attacks {allies[targetIndex].Name} for {Attackpower} damage!");

        }
    }
}


public class Vampire : Enemy
{
    public static Texture2D spriteVampireIdleStatic;
    public static Texture2D spriteVampireAttackStatic;
    public static Texture2D spriteVampireHurtStatic;
    public static Texture2D spriteVampireDeadStatic;

    public Vampire(string name, int hp, int atk, Vector2 pos) : base(name, hp, atk, pos)
    {
        this.frameCountDead = 8;
    }

    public static void LoadSprites(Microsoft.Xna.Framework.Content.ContentManager content)
    {
        spriteVampireIdleStatic = content.Load<Texture2D>("Characters/Vampire/Idle");
        spriteVampireAttackStatic = content.Load<Texture2D>("Characters/Vampire/Attack");
        spriteVampireHurtStatic = content.Load<Texture2D>("Characters/Vampire/Hurt");
        spriteVampireDeadStatic = content.Load<Texture2D>("Characters/Vampire/Death");
    }

    public override bool ExecuteSkill(int skillIndex, List<Player> allies, List<Enemy> enemies, int targetIndex, bool isultimateunlocked)
    {
        allies[targetIndex].TakeDamage(Attackpower);
        System.Diagnostics.Debug.WriteLine($"{Name} uses Life Drain on {allies[targetIndex].Name}!");
        this.Heal(10);
        return true;
    }
}

public class Skeleton : Enemy
{
    public static Texture2D spriteSkeletonIdleStatic;
    public static Texture2D spriteSkeletonAttackStatic;
    public static Texture2D spriteSkeletonHurtStatic;
    public static Texture2D spriteSkeletonDeadStatic;

    public Skeleton(string name, int hp, int atk, Vector2 pos) : base(name, hp, atk, pos) { }

    public static void LoadSprites(Microsoft.Xna.Framework.Content.ContentManager content)
    {
        spriteSkeletonIdleStatic = content.Load<Texture2D>("Characters/Skeleton/Idle");
        spriteSkeletonAttackStatic = content.Load<Texture2D>("Characters/Skeleton/Attack");
        spriteSkeletonHurtStatic = content.Load<Texture2D>("Characters/Skeleton/Hurt");
        spriteSkeletonDeadStatic = content.Load<Texture2D>("Characters/Skeleton/Death");
    }

    public override bool ExecuteSkill(int skillIndex, List<Player> allies, List<Enemy> enemies, int targetIndex, bool isultimateunlocked)
    {
        allies[targetIndex].TakeDamage(Attackpower + 10);
        System.Diagnostics.Debug.WriteLine("Skeleton uses Shield Crush for extra damage!");
        return true;
    }
}

public class Bat : Enemy
{
    public static Texture2D spriteBatIdleStatic;
    public static Texture2D spriteBatAttackStatic;
    public static Texture2D spriteBatHurtStatic;
    public static Texture2D spriteBatDeadStatic;

    public Bat(string name, int hp, int atk, Vector2 pos) : base(name, hp, atk, pos) { }

    public static void LoadSprites(Microsoft.Xna.Framework.Content.ContentManager content)
    {
        spriteBatIdleStatic = content.Load<Texture2D>("Characters/Bat/idle");
        spriteBatAttackStatic = content.Load<Texture2D>("Characters/Bat/attack");
        spriteBatHurtStatic = content.Load<Texture2D>("Characters/Bat/hurt");
        spriteBatDeadStatic = content.Load<Texture2D>("Characters/Bat/death");
    }

    public override bool ExecuteSkill(int skillIndex, List<Player> allies, List<Enemy> enemies, int targetIndex, bool isultimateunlocked)
    {
        var target = allies[targetIndex];
        int baseDamage = Attackpower;

        // 🔥 KRİTİK: Eğer hedef müttefikin canı %50'nin altındaysa zehir coşar, 2.5 kat vurur!
        if (target.CurrentHP < (target.MaxHP / 2))
        {
            baseDamage = (int)(baseDamage * 2.5f);
            System.Diagnostics.Debug.WriteLine($"⚠️ {Name} enjected Vampiric Venom into a weak target! Deadly Damage!");
        }

        // Hasarı vur
        target.TakeDamage(baseDamage);
        return true;
    }
}

public class EvilWizard : Enemy
{
    // 🔥 EVIL WIZARD İÇİN STATİC TEXTURE ALANLARI
    public static Texture2D spriteWizardIdleStatic;
    public static Texture2D spriteWizardAttackStatic;
    public static Texture2D spriteWizardHurtStatic;
    public static Texture2D spriteWizardDeadStatic;

    public EvilWizard(string name, int hp, int atk, Vector2 pos) : base(name, hp, atk, pos) { }

    // MGCB Editor içindeki büyük/küçük harf durumuna göre (Idle, Attack, Hurt, Death)
    public static void LoadSprites(Microsoft.Xna.Framework.Content.ContentManager content)
    {
        spriteWizardIdleStatic = content.Load<Texture2D>("Characters/EvilWizard/Idle");
        spriteWizardAttackStatic = content.Load<Texture2D>("Characters/EvilWizard/Attack");
        spriteWizardHurtStatic = content.Load<Texture2D>("Characters/EvilWizard/Hurt");
        spriteWizardDeadStatic = content.Load<Texture2D>("Characters/EvilWizard/Death");
    }

    public override bool ExecuteSkill(int skillIndex, List<Player> allies, List<Enemy> enemies, int targetIndex, bool isultimateunlocked)
    {
        allies[targetIndex].TakeDamage(Attackpower);
        allies[targetIndex].CurrentMP -= 15; if (allies[targetIndex].CurrentMP < 0) allies[targetIndex].CurrentMP = 0;
        System.Diagnostics.Debug.WriteLine($"{Name} drained {15} MP from {allies[targetIndex].Name}"); return true;
    }
}

public class Sorcerer : Enemy
{
    // 🔥 SORCERER İÇİN STATİC TEXTURE ALANLARI
    public static Texture2D spriteSorcererIdleStatic;
    public static Texture2D spriteSorcererAttackStatic;
    public static Texture2D spriteSorcererDeadStatic;

    public Sorcerer(string name, int hp, int atk, Vector2 pos) : base(name, hp, atk, pos) { }

    // MGCB Editor'e eklediğin verbatim dosya isimlerine göre yükleme
    public static void LoadSprites(Microsoft.Xna.Framework.Content.ContentManager content)
    {
        spriteSorcererIdleStatic = content.Load<Texture2D>("Characters/Sorcerer/Idle");
        spriteSorcererAttackStatic = content.Load<Texture2D>("Characters/Sorcerer/Attack");
        spriteSorcererDeadStatic = content.Load<Texture2D>("Characters/Sorcerer/Death");
    }

    public override bool ExecuteSkill(int skillIndex, List<Player> allies, List<Enemy> enemies, int targetIndex, bool isultimateunlocked)
    {
        Enemy weakestAlly = enemies[0];
        foreach (var e in enemies) { if (e.CurrentHP < weakestAlly.CurrentHP) weakestAlly = e; }
        weakestAlly.Heal(20);
        System.Diagnostics.Debug.WriteLine($"{Name} healed {weakestAlly.Name}!"); return true;
    }
}

public class Zombie : Enemy
{
    public Zombie(string name, int hp, int atk, Vector2 pos) : base(name, hp, atk, pos) { }

    public override bool ExecuteSkill(int skillIndex, List<Player> allies, List<Enemy> enemies, int targetIndex, bool isultimateunlocked)
    {
        allies[targetIndex].TakeDamage(Attackpower);

        allies[targetIndex].Attackpower = allies[targetIndex].BaseAttackPower / 2;

        System.Diagnostics.Debug.WriteLine($"{allies[targetIndex].Name}'s attack is weakened by bite from {Name}!");
        return true;
    }
}

public class FinalBoss : Enemy
{
    private int turncount = 0;

    public FinalBoss(string name, int hp, int atk, Vector2 pos) : base(name, hp, atk, pos)
    {
        MaxHP = 200;
        CurrentHP = 200;
    }

    public override bool ExecuteSkill(int skillIndex, List<Player> allies, List<Enemy> enemies, int targetIndex, bool isultimateunlocked)
    {
        turncount++;

        if (turncount % 2 == 0)
        {
            System.Diagnostics.Debug.WriteLine($"{Name} uses METEOR STRIKE! Everyone takes damage!");
            foreach (var player in allies)
            {
                player.TakeDamage(Attackpower - 5);
            }
        }
        else
        {
            System.Diagnostics.Debug.WriteLine($"{Name} uses VOID BLAST on {allies[targetIndex].Name}!");
            allies[targetIndex].TakeDamage(Attackpower);
        }

        if (CurrentHP < (MaxHP * 0.3f))
        {
            Attackpower = 40;
            System.Diagnostics.Debug.WriteLine($"{Name} is ENRAGED! Attack power increased!");
        }

        return true;
    }
}