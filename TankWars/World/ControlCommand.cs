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
    public class ControlCommand
    {


        [JsonProperty(PropertyName = "moving")]
        private string moving;

        [JsonProperty(PropertyName = "fire")]
        private string fire;

        [JsonProperty(PropertyName = "tdir")]
        private Vector2D tdir;


        public string Moving { get => moving; set => moving = value; }
        public string Fire { get => fire; set => fire = value; }
        public Vector2D Tdir { get => tdir; set => tdir = value; }

        public ControlCommand()
        {
            moving = "none";
            fire = "none";
            tdir = new Vector2D(0, 0);
        }

    }
}
