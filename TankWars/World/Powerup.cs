using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using TankWars;

namespace TankWars
{
    [JsonObject(MemberSerialization.OptIn)]
    class Powerup
    {

        [JsonProperty(PropertyName = "power")]
        private int ID;

        [JsonProperty]
        private Vector2D loc;

        [JsonProperty]
        private bool died;



        public int powerID { get => ID; set => ID = value; }
        public Vector2D Location { get => loc; set => loc = value; }
        public bool Died { get => died; set => died = value; }



    }
}