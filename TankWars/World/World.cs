using System;
using System.Collections.Generic;
using System.Text;

namespace World
{
    class World
    {
        public Dictionary<int, Tank> tanks;
        public Dictionary<int, Projectile> projectiles;
        public Dictionary<int, Wall> walls;
        public Dictionary<int, Powerup> powerUps;
        //do we add beams?

        public World()
        {
            tanks = new Dictionary<int, Tank>();
            projectiles = new Dictionary<int, Projectile>();
            walls = new Dictionary<int, Wall>();
            powerUps = new Dictionary<int, Powerup>();
        }

    }
}
