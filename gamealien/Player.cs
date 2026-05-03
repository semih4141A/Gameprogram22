using System.Collections.Generic;
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

    public class Leroy : Player
    {
        public Leroy(string name, int hp, int mp, int atk, Vector2 pos) :
         base(name, hp, mp, atk, pos)
        {
        }

        public override bool ExecuteSkill(int skillIndex, List<Player> allies, List<Enemy> enemies, int targetIndex)
        {
            int skill1manacost = 0;
            int skill2manacost = 20;
            if (skillIndex == 0)
            {
                if (CurrentMP >= skill1manacost)
                {
                    CurrentMP -= skill1manacost;
                    enemies[targetIndex].TakeDamage(Attackpower);
                    return true;
                }
            }
            else if (skillIndex == 1)
            {
                if (CurrentMP >= skill2manacost)
                {
                    CurrentMP -= skill2manacost;
                    enemies[targetIndex].TakeDamage(Attackpower);
                    return true;
                }
            }
            return false;
        }
    }

    public class Renato : Player
    {
        public Renato(string name, int hp, int mp, int atk, Vector2 pos) :
         base(name, hp, mp, atk, pos)
        {
        }

        public override bool ExecuteSkill(int skillIndex, List<Player> allies, List<Enemy> enemies, int targetIndex)
        {
            int skill1manacost = 10;
            int skill2manacost = 20;
            if (skillIndex == 0)
            {
                if (CurrentMP >= skill1manacost)
                {
                    CurrentMP -= skill1manacost;
                    enemies[targetIndex].TakeDamage(Attackpower);
                    return true;
                }
            }
            else if (skillIndex == 1)
            {
                if (CurrentMP >= skill2manacost)
                {
                    CurrentMP -= skill2manacost;
                    enemies[targetIndex].TakeDamage(Attackpower);
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

        public override bool ExecuteSkill(int skillIndex, List<Player> allies, List<Enemy> enemies, int targetIndex)
        {
            int skill1manacost = 10;
            int skill2manacost = 20;

            if (skillIndex == 0)
            {
                if (CurrentMP >= skill1manacost)
                {
                    CurrentMP -= skill1manacost;
                    enemies[targetIndex].TakeDamage(Attackpower);
                    return true;
                }
            }
            else if (skillIndex == 1)
            {

                CurrentMP -= skill2manacost;
                enemies[targetIndex].TakeDamage(Attackpower);
                return true;

            }
            return false;
        }
    }





}