using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
/// <summary>
/// Author:  Tyler Amy, Lindsey Loveland
/// Date: 11/23/2020
/// </summary>
namespace TankWars
{
    public class World
    {
        public Dictionary<int, Wall> Walls;
        public Dictionary<int, Tank> Tanks;
        public Dictionary<int, Projectile> Projectiles;
        public Dictionary<int, Powerup> PowerUps;
        public int worldSize
            {
            get;  private set;
            }

        public World(int size)
        {
            Walls = new Dictionary<int, Wall>();
            worldSize = size;
            Tanks = new Dictionary<int, Tank>();
            Projectiles = new Dictionary<int, Projectile>();
            PowerUps = new Dictionary<int, Powerup>();
        }
       
    }
}

