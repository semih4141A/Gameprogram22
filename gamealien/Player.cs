using System.Collections.Generic;
using Microsoft.Xna.Framework;

public class Player : BaseCharacter
{
    public int MaxMP { get; set; }
    public int CurrentMP { get; set; }

    public int BaseAttackPower { get; set; }

    public void GainMP(int amount)
    {
        CurrentMP += amount;

        if (CurrentMP > MaxMP)
        {
            CurrentMP = MaxMP;
        }


    }

    public Player(string name, int hp, int mp, int atk, Vector2 pos) :
     base(name, hp, atk, pos)
    {
        MaxMP = mp;
        CurrentMP = mp;
        BaseAttackPower = atk;
    }



    public class Leroy : Player
    {

        public bool IsGuarding { get; set; } = false;
        public bool DoubleTroubleActive { get; set; } = false;
        public Leroy(string name, int hp, int mp, int atk, Vector2 pos) :
         base(name, hp, mp, atk, pos)
        {


        }

        public override bool ExecuteSkill(int skillIndex, List<Player> allies, List<Enemy> enemies, int targetIndex, bool isultimateunlocked)
        {
            int skill1manacost = 0;
            int skill2manacost = 20;
            int skill3manacost = 10;
            int skill4manacost = 10;
            System.Diagnostics.Debug.WriteLine($"LEROY ATAK GUCU: {this.Attackpower}");
            if (skillIndex == 0)
            {
                if (CurrentMP >= skill1manacost)
                {
                    CurrentMP -= skill1manacost;
                    GainMP(10);
                    enemies[targetIndex].TakeDamage(Attackpower);
                    System.Console.WriteLine($"{enemies[targetIndex].Name} takes {Attackpower} damage from {Name}!");
                    this.Attackpower = this.BaseAttackPower;
                    return true;
                }
            }
            else if (skillIndex == 1)//Double Trouble
            {
                if (CurrentMP >= skill2manacost)
                {
                    DoubleTroubleActive = true;
                    CurrentMP -= skill2manacost;
                    enemies[targetIndex].TakeDamage(Attackpower * 2);
                    System.Console.WriteLine($"{enemies[targetIndex].Name} takes {Attackpower * 2} damage from {Name}!");
                    this.Attackpower = this.BaseAttackPower;
                    return true;
                }
            }

            else if (skillIndex == 2)//Sword Rain
            {

                if (CurrentMP >= skill3manacost)
                {
                    foreach (var e in enemies) e.TakeDamage(Attackpower);
                    CurrentMP -= skill3manacost;
                    System.Console.WriteLine($"{Name} uses Sword Rain, hitting all enemies for {Attackpower} damage!");
                    this.Attackpower = this.BaseAttackPower;

                    return true;
                }
            }

            else if (skillIndex == 3 && isultimateunlocked)//Guardian Stance
            {
                if (CurrentMP >= skill4manacost)
                {
                    IsGuarding = true;
                    CurrentMP -= skill4manacost;
                    System.Console.WriteLine($"{Name} enters Guardian Stance!");
                    return true;
                }
            }




            return false;
        }

        public override void TakeDamage(int damage)
        {

            if (DoubleTroubleActive) damage *= 2;
            if (IsGuarding) damage /= 4;
            base.TakeDamage(damage);
            DoubleTroubleActive = false; IsGuarding = false;
        }
    }

    public class Renato : Player
    {
        public Renato(string name, int hp, int mp, int atk, Vector2 pos) :
         base(name, hp, mp, atk, pos)
        {
        }

        public override bool ExecuteSkill(int skillIndex, List<Player> allies, List<Enemy> enemies, int targetIndex, bool isultimateunlocked)
        {
            int skill1manacost = 0;
            int skill2manacost = 20;
            int skill3manacost = 10;
            int skill4manacost = 30;
            if (skillIndex == 0)
            {
                if (CurrentMP >= skill1manacost)
                {

                    CurrentMP -= skill1manacost;
                    GainMP(10);
                    enemies[targetIndex].TakeDamage(Attackpower);
                    System.Console.WriteLine($"{enemies[targetIndex].Name} takes {Attackpower} damage from {Name}!");
                    this.Attackpower = this.BaseAttackPower;
                    return true;
                }
            }
            else if (skillIndex == 1)//Heal 
            {
                if (CurrentMP >= skill2manacost)
                {
                    CurrentMP -= skill2manacost;
                    allies[targetIndex].Heal(30);
                    System.Console.WriteLine($"{allies[targetIndex].Name} is healed by {Name}!");
                    return true;
                }
            }



            else if (skillIndex == 2)// powerup
            {
                if (CurrentMP >= skill3manacost)
                {
                    CurrentMP -= skill3manacost;
                    foreach (var a in allies)
                    {
                        a.Attackpower = a.BaseAttackPower + 20;
                        System.Diagnostics.Debug.WriteLine($"BUFFLANDI -> {a.Name}, Yeni Atak: {a.Attackpower}");

                    }
                    return true;
                }
            }

            else if (skillIndex == 3 && isultimateunlocked)// Ulti Final sacrfice
            {
                if (CurrentMP >= skill4manacost)
                {
                    CurrentMP -= skill4manacost;

                    enemies[targetIndex].CurrentHP = 0;
                    this.CurrentHP = 0;


                    return true;
                }
            }
            return false;
        }
    }


    public class Yaser : Player
    {
        public Yaser(string name, int hp, int mp, int atk, Vector2 pos) :
         base(name, hp, mp, atk, pos)
        {
        }

        public override bool ExecuteSkill(int skillIndex, List<Player> allies, List<Enemy> enemies, int targetIndex, bool isultimateunlocked)
        {
            int skill1manacost = 0;
            int skill2manacost = 20;
            int skill3manacost = 0;
            int skill4manacost = 30;

            if (skillIndex == 0)
            {
                if (CurrentMP >= skill1manacost)
                {
                    CurrentMP -= skill1manacost;
                    GainMP(10);
                    enemies[targetIndex].TakeDamage(Attackpower);
                    System.Console.WriteLine($"{enemies[targetIndex].Name} takes {Attackpower} damage from {Name}!");
                    this.Attackpower = this.BaseAttackPower;
                    return true;
                }
            }
            else if (skillIndex == 1)
            {
                if (CurrentMP >= skill2manacost)
                {
                    CurrentMP -= skill2manacost;
                    int enemyCount = enemies.Count;
                    if (enemyCount > 0)
                    {
                        int dividedDamage = 30 / enemyCount;
                        foreach (var e in enemies) e.TakeDamage(dividedDamage);
                        this.Attackpower = this.BaseAttackPower;
                    }
                    System.Console.WriteLine($"{Name} uses Chain Lightning, dealing 30 damage divided among all enemies!");
                    return true;

                }



            }

            else if (skillIndex == 2)
            {

                if (CurrentMP >= skill3manacost)
                {
                    CurrentMP -= skill3manacost;
                    allies[targetIndex].CurrentMP += 30;
                    if (allies[targetIndex].CurrentMP > allies[targetIndex].MaxMP)
                        allies[targetIndex].CurrentMP = allies[targetIndex].MaxMP;

                    return true;
                }
            }

            else if (skillIndex == 3 && isultimateunlocked)
            {
                if (CurrentMP >= skill4manacost)
                {

                    int totalDamage = CurrentMP;
                    foreach (var e in enemies) e.TakeDamage(totalDamage);
                    CurrentMP = 0;
                    this.Attackpower = this.BaseAttackPower;
                    System.Console.WriteLine($"{Name} uses Ultimate Sacrifice, dealing {totalDamage} damage to all enemies!");
                    return true;
                }
            }

            return false;
        }
    }







}