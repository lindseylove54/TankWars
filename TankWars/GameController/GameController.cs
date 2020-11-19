using NetworkUtil;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace GameController
{
    public class GameController
    {

        public string playerName
        {
            get; set;
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
            ProcessStartupData(state);

            state.OnNetworkAction = ReceiveWorld;
            Networking.GetData(state);
        }

        private void ReceiveWorld(SocketState state)
        {
            throw new NotImplementedException();
        }

        public void ProcessStartupData(SocketState state)
        {
            string totalData = state.GetData();
            string[] parts = Regex.Split(totalData, @"(?<=[\n])");
            List<string> startupData = new List<string>();
            
            //first string is player ID
            //string id = parts[0];



            //second string is world size
            //string worldSize = parts[1];

            //handle any parsing errors of id and worldSize with tryParse
            int.TryParse(parts[0].Substring(0, parts[0].Length - 1), out int playerID);
            int.TryParse(parts[1].Substring(0, parts[1].Length - 1), out int worldSize);

           

            //***SHOULD THIS BE IN RECEIVE WORLD ???? ***\\
            //foreach wall... do something. 
            foreach (string p in parts)
            {
                //  remove it from the SocketState's growable buffer... check if this is needed
                state.RemoveData(0, p.Length);
                //default checks including the \n check
                if (p.Length == 0)
                {
                    continue;
                }
                if (p[p.Length - 1] != '\n')
                {
                    break;
                }

                //***is this correct? 2-4 substring for the type?
                string type = p.Substring(2, 4);

                //according to the examples on the assignment page, the type of object always comes first...
                //process accordingly
                if (type.Equals("tank"))
                {
                    //create tank, json deserialize it, then add it to the world 
                }
                else if (type.Equals("wall"))
                {
                    //create wall, json deserialize it, then add it to the world 
                }
                else if (type.Equals("proj"))
                {
                    //create projectile, json deserialize it, then add it to the world 
                } 
                else if (type.Equals("power"))
                {
                    //create powerup, json deserialize it, then add it to the world 
                }
                else if (type.Equals("beam"))
                {
                    //create beam, json deserialize it, then add it to the world 
                }

                //  remove it from the SocketState's growable buffer... check if this is needed
                state.RemoveData(0, p.Length);
            }
            //inform the view
            //call some sort of redraw function
        }






    }
}