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
    public class Wall
    {

        [JsonProperty(PropertyName = "wall")]
        private int ID;

        [JsonProperty]
        private Vector2D p1;

        [JsonProperty]
        private Vector2D p2;


        public int wallID { get => ID; set => ID = value; }
        public Vector2D P1 { get => p1; set => p1 = value; }
        public Vector2D P2 { get => p2; set => p2 = value; }

        
    }
}