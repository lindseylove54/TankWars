using NetworkUtil;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

/// <summary>
/// Author:  Tyler Amy, Lindsey Loveland
/// Date: 11/23/2020
/// </summary>
namespace TankWars 
{
    /// <summary>
    /// The GameController handles receiving data from the server, and informing the view the changed.  It is also responsible for sending
    /// JSON commands to the server. 
    /// </summary>
    public class GameController
    {
        //the world that the entire solution will be using 
        private World theWorld;
        //Used for closing the form when needed. 
        SocketState theServer = null;
        //List of delegate methods 
        public delegate void ServerUpdateHandler();
        public delegate void ConnectWorldToView(World w);
        public delegate void TankReceivedFromServer();
        public delegate void ConnectPlayerIDToView(int id);
        public delegate void BeamFire(Beam b);

        //List of events 
        public event ServerUpdateHandler UpdateArrived;
        public event ConnectWorldToView ConnectToView;
        public event TankReceivedFromServer TankReceived;
        public event ConnectPlayerIDToView connectID;
        public event BeamFire shootBeam;

        //ControlCommand object for sending JSON objects to the server. 
        ControlCommand command;
        //bools that must be true in order to start drawing. 
        bool playerIDReceived;
        bool WorldSizeReceived;
        public string playerName
        {
            get;  set;
        }
        //these two variables are initialized during the first step of the handshake. 
        private int worldSize;
        private int playerID;

        /// <summary>
        /// GameController constructor
        /// </summary>
        public GameController()
        {
            command = new ControlCommand();
        }

        /// <summary>
        /// Called when the connect button is clicked.  Establishes a connection to the server. 
        /// </summary>
        /// <param name="address"></param>
        public void Connect(string address)
        {
            Networking.ConnectToServer(firstContact, address, 11000);
        }

        /// <summary>
        /// First stage of the handshake between client and server.  
        /// </summary>
        /// <param name="state"></param>
        private void firstContact(SocketState state)
        {

            state.OnNetworkAction = ReceiveStartup;
            //send the server the player name 
            Networking.Send(state.TheSocket, playerName + "\n");
            Networking.GetData(state);
        }
        /// <summary>
        /// Second phase of the handshake between server and client
        /// </summary>
        /// <param name="state"></param>
        private void ReceiveStartup(SocketState state)
        {
            //extract data
            if (ProcessStartupData(state))
            {
                //create the world once the worldSize has been received. 
                theWorld = new World(worldSize);
                //send the world to the view 
                ConnectToView(theWorld);
                //send the playerID to the view
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


        /// <summary>
        /// this method is used for handling when the tank moves, sending the appropriate JSON command to the server. 
        /// </summary>
        /// <param name="movement"></param>
        public void HandleMoveRequest(string movement)
        {
            command.Moving = movement;
        }
        /// <summary>
        /// Send the appropriate JSON for the Tdir of the turret to the server. 
        /// </summary>
        /// <param name="p"></param>
        public void HandleMouseMovement(Point p)
        {
            
                //-450 to both x and y coordinates, to center the mouse and tank
                float x = p.X - 450 ;
                float y = p.Y - 450;
                Vector2D vector = new Vector2D(x, y);
                vector.Normalize();
                command.Tdir = vector;
            
        }
        /// <summary>
        /// This method is used for handling when the mouse is clicked, and sending the appropriate JSON command to the server
        /// </summary>
        /// <param name="mouseClick"></param>
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

        /// <summary>
        /// This method is invoked when the form closes.
        /// </summary>
        /// <param name="e"></param>
        public void HandleFormClosing(FormClosingEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to exit?", "Close Application", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                lock (theWorld)
                {
                    Environment.Exit(0);
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