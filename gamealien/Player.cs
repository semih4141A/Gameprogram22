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

        public override void ExecuteSkill(int skillIndex, List<Player> allies, List<Enemy> enemies, int targetIndex)
        {
            if (skillIndex == 0)
            {
                if (CurrentMP >= 0)
                {
                    CurrentMP -= 0;
                    enemies[targetIndex].TakeDamage(Attackpower);
                }
            }
            else if (skillIndex == 1)
            {
                CurrentMP -= 0;
                enemies[targetIndex].TakeDamage(Attackpower);

            }
        }
    }

    public class Renato : Player
    {
        public Renato(string name, int hp, int mp, int atk, Vector2 pos) :
         base(name, hp, mp, atk, pos)
        {
        }

        public override void ExecuteSkill(int skillIndex, List<Player> allies, List<Enemy> enemies, int targetIndex)
        {
            if (skillIndex == 0)
            {
                if (CurrentMP >= 0)
                {
                    CurrentMP -= 0;
                    enemies[targetIndex].TakeDamage(Attackpower);
                }
            }
            else if (skillIndex == 1)
            {
                CurrentMP -= 0;
                enemies[targetIndex].TakeDamage(Attackpower);

            }
        }
    }


    public class Yaser : Player
    {
        public Yaser(string name, int hp, int mp, int atk, Vector2 pos) :
         base(name, hp, mp, atk, pos)
        {
        }

        public override void ExecuteSkill(int skillIndex, List<Player> allies, List<Enemy> enemies, int targetIndex)
        {
            if (skillIndex == 0)
            {
                if (CurrentMP >= 0)
                {
                    CurrentMP -= 0;
                    enemies[targetIndex].TakeDamage(Attackpower);
                }
            }
            else if (skillIndex == 1)
            {

                CurrentMP -= 0;
                enemies[targetIndex].TakeDamage(Attackpower);

            }
        }
    }





}