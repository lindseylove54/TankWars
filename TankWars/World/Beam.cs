using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TankWars
{
    [JsonObject(MemberSerialization.OptIn)]
   public class Beam
    {


        [JsonProperty(PropertyName = "beam")]
        private int ID;

        [JsonProperty(PropertyName = "org")]
        private Vector2D org;

        [JsonProperty(PropertyName = "dir")]
        private Vector2D dir;

        [JsonProperty(PropertyName = "owner")]
        private bool owner;

        public int BeamID { get => ID; set => ID = value; }
        public bool Owner { get => owner; set => owner = value; }
        public Vector2D Origin { get => org; set => org = value; }
        public Vector2D Direction { get => dir; set => dir = value; }

    }
}
