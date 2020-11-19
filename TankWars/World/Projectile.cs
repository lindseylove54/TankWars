using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using TankWars;

namespace Tankwars
{
    [JsonObject(MemberSerialization.OptIn)]
    class Projectile
    {

        [JsonProperty(PropertyName = "proj")]
        private int ID;

        [JsonProperty]
        private Vector2D loc;

        [JsonProperty]
        private Vector2D dir;

        [JsonProperty]
        private bool died;

        [JsonProperty]
        private int owner;

        public int Proj { get => ID; set => ID = value; }
        public Vector2D Location { get { return new Vector2D(loc); } set => loc = value; }
        public Vector2D Direction { get { return new Vector2D(dir); } set => dir = value; }
        public bool Died { get => died; set => died = value; }
        public int Owner { get => owner; set => owner = value; }

    }
}