﻿using Newtonsoft.Json;
using System;
using TankWars;

namespace TankWars
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Tank
    {
        [JsonProperty(PropertyName = "tank")]
        private int tankID;

        [JsonProperty(PropertyName = "loc")]
        private Vector2D location;

        [JsonProperty(PropertyName = "bdir")]
        private Vector2D orientation;

        [JsonProperty(PropertyName = "tdir")]
        private Vector2D aiming = new Vector2D();

        [JsonProperty(PropertyName = "name")]
        private string name;

        //[JsonProperty(PropertyName = "hp")]
        //private int hitPoints = Constants.MaxHP;

        [JsonProperty(PropertyName = "score")]
        private int score = 0;

        [JsonProperty(PropertyName = "died")]
        private bool died = false;

        [JsonProperty(PropertyName = "dc")]
        private bool disconnected = false;

        [JsonProperty(PropertyName = "join")]
        private bool joined = false;

        public Vector2D Location { get =>  location; set => location = value; }
        public int TankID { get => tankID; set => tankID = value; }
        public Vector2D Orientation { get => orientation; set => orientation = value; }
        public Tank( )
        {
        }
    }
}