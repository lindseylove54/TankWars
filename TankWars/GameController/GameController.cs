using NetworkUtil;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace TankWars
{
    public class GameController
    {

        private World theWorld;

        SocketState theServer = null;
        public delegate void ServerUpdateHandler();
        public delegate void ConnectWorldToView(World w);
        public delegate void TankReceivedFromServer();
        public delegate void ConnectPlayerIDToView(int id);
        public delegate void BeamFire(Beam b);

        public event ServerUpdateHandler UpdateArrived;
        public event ConnectWorldToView ConnectToView;
        public event TankReceivedFromServer TankReceived;
        public event ConnectPlayerIDToView connectID;
        public event BeamFire shootBeam;

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
            if (ProcessStartupData(state))
            {
                //create the world once the worldSize has been received. 
                theWorld = new World(worldSize);

                ConnectToView(theWorld);
                connectID(playerID);
                state.OnNetworkAction = ReceiveWorld;
                theServer = state;
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
                        shootBeam(beam);
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
            //send control command data
            string message = JsonConvert.SerializeObject(command);
            Networking.Send(state.TheSocket, message + "\n");
            //draw frame
            UpdateArrived();
            Networking.GetData(state);

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

            if (int.TryParse(parts[0], out int result))
            {
                playerID = result;
                playerIDReceived = true;
                state.RemoveData(0, parts[0].Length);
            }

            if (int.TryParse(parts[1], out int size))
            {
                worldSize = size;
                WorldSizeReceived = true;
                state.RemoveData(0, parts[1].Length);
            }

            return WorldSizeReceived && playerIDReceived;

        }



        public void HandleMoveRequest(string movement)
        {
            command.Moving = movement;
        }

        public void HandleMouseMovement(Point p)
        {
            
            
                float x = p.X - 450 ;
                float y = p.Y - 450;
                Vector2D vector = new Vector2D(x, y);
                vector.Normalize();
                command.Tdir = vector;
            
        }

        public void HandleMouseClick(string mouseClick)
        {

            if (mouseClick.Equals("left"))
            {

                command.Fire = "main";

            }
            else if (mouseClick.Equals("right"))
            {
                command.Fire = "alt";

            }
            else
            {
                command.Fire = "none";
            }
        }

        public void HandleFormClosing(FormClosingEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to exit?", "Close Application", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                lock (theWorld)
                {
                    Application.Exit();
                    theServer.TheSocket.Close(0);
                }

            }
            else
            {
                e.Cancel = true;
                return;
            }
        }

    }
}