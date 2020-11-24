using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
/// <summary>
/// Author:  Tyler Amy, Lindsey Loveland
/// Date: 11/23/2020
/// </summary>
namespace TankWars
{
    [JsonObject(MemberSerialization.OptIn)]
   public class Projectile
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

        public int ProjID { get => ID; set => ID = value; }
        public Vector2D Location { get { return new Vector2D(loc); } set => loc = value; }
        public Vector2D Direction { get { return new Vector2D(dir); } set => dir = value; }
        public bool Died { get => died; set => died = value; }
        public int Owner { get => owner; set => owner = value; }

    }
}