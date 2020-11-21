using NetworkUtil;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;

namespace TankWars
{
    public class GameController
    {

       private World theWorld;
       

        public delegate void ServerUpdateHandler();
        public delegate void ConnectWorldToView(World w);
        public delegate void TankReceivedFromServer();
        public delegate void ConnectPlayerIDToView(int id);

        public event ServerUpdateHandler UpdateArrived;
        public event ConnectWorldToView ConnectToView;
        public event TankReceivedFromServer TankReceived;
        public event ConnectPlayerIDToView connectID;
        

        ControlCommand command;
        bool playerIDReceived;
        bool WorldSizeReceived;
        public string playerName
        {
            get; set;
        }

        private int worldSize;
        private int playerID;
        
        public GameController()
        {
            command = new ControlCommand();
        }


        public void Connect(string address)
        {
            Networking.ConnectToServer(firstContact, address, 11000);
        }

        private void firstContact(SocketState state)
        {
            
            state.OnNetworkAction = ReceiveStartup;
            Networking.Send(state.TheSocket, playerName + "\n");
            Networking.GetData(state);
        }

        private void ReceiveStartup(SocketState state)
        {
            //extract data
           if(ProcessStartupData(state))
            {
                //create the world once the worldSize has been received. 
                theWorld = new World(worldSize);
                
                ConnectToView(theWorld);
                connectID(playerID);
                state.OnNetworkAction = ReceiveWorld;
            }

            Networking.GetData(state);
        }

        /// <summary>
        /// final step of handshake that runs on repeat.  Processess JSON and informs the view to redraw everything
        /// </summary>
        /// <param name="state"></param>
        private void ReceiveWorld(SocketState state)
        {
            string totalData = state.GetData();
            string[] parts = Regex.Split(totalData, @"(?<=[\n])");

            lock (theWorld)
            {
                //receive frame
                foreach (string s in parts)
                {
                    if (s == "") continue;

                    if (!s.EndsWith("\n")) continue;

                    JObject obj = JObject.Parse(s);

                    JToken token = obj["wall"];

                    if (token != null)
                    {
                        Wall wall = JsonConvert.DeserializeObject<Wall>(s);
                        theWorld.Walls.Add(wall.wallID, wall);
                        state.RemoveData(0, s.Length);

                        continue;
                    }
                    token = obj["tank"];

                    if (token != null)
                    {
                        Tank tank = JsonConvert.DeserializeObject<Tank>(s);

                        theWorld.Tanks[tank.TankID] = tank;
                        TankReceived();
                        state.RemoveData(0, s.Length);
                        continue;
                    }

                    token = obj["proj"];
                    if (token != null)
                    {
                        Projectile proj = JsonConvert.DeserializeObject<Projectile>(s);
                        theWorld.Projectiles[proj.ProjID] = proj;
                        state.RemoveData(0, s.Length);
                        continue;
                    }
                    token = obj["beam"];
                    if (token != null)
                    {
                        Beam beam = JsonConvert.DeserializeObject<Beam>(s);
                        theWorld.Beams[beam.beamID] = beam;
                        state.RemoveData(0, s.Length);
                        continue;
                    }
                    token = obj["power"];
                    if (token != null)
                    {
                        Powerup power = JsonConvert.DeserializeObject<Powerup>(s);
                        theWorld.PowerUps[power.powerID] = power;
                        state.RemoveData(0, s.Length);
                        continue;
                    }
                }

            }
            //draw frame
            //send control command data
            string message = JsonConvert.SerializeObject(command);
            Networking.Send(state.TheSocket, message + "\n");
                UpdateArrived();
           Networking.GetData(state);
            //send game controller message

            ;
        }

        /// <summary>
        /// This method will return true if both the world size, and player ID were successfully received from the server. False otherwise
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        private bool ProcessStartupData(SocketState state)
        {
            string totalData = state.GetData();
            string[] parts = Regex.Split(totalData, @"(?<=[\n])");
            if (parts.Length < 2) return false;

            if(int.TryParse(parts[0], out int result))
            {
                playerID = result;
                playerIDReceived = true;
                state.RemoveData(0, parts[0].Length );
            }

            if(int.TryParse(parts[1], out int size))
            {
                worldSize = size;
                WorldSizeReceived = true;
                state.RemoveData(0, parts[1].Length );
            }

            return WorldSizeReceived && playerIDReceived;
           
        }



        public void HandleMoveRequest(string movement)
        {
            command.Moving = movement;
        }

        public void HandleMouseMovement(Point p)
        {
            float x = p.X;
            float y = p.Y;
            Vector2D vector = new Vector2D(x, y);
            vector.Normalize();
            command.Tdir = vector;
        }
    }
}
