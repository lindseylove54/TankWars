using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using TankWars;

namespace World
{
    [JsonObject(MemberSerialization.OptIn)]
    class Beam
    {

        [JsonProperty(PropertyName = "beam")]
        private int ID;

        [JsonProperty]
        private Vector2D org;

        [JsonProperty]
        private Vector2D dir;

        [JsonProperty]
        private int owner;


        public int wallID { get => ID; set => ID = value; }
        public Vector2D Origin { get => org; set => org = value; }
        public Vector2D Direction { get => dir; set => dir = value; }
        public int Owner { get => owner; set => owner = value; }
    }
}
