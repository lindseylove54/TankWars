using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TankWars
{
    public class World
    {
        public Dictionary<int, Wall> Walls;
        public Dictionary<int, Tank> Tanks;
        public Dictionary<int, Projectile> Projectiles;
        public Dictionary<int, Powerup> PowerUps;
        public Dictionary<int, Beam> Beams;
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
            Beams = new Dictionary<int, Beam>();
        }
       
    }
}

