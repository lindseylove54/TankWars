using NetworkUtil;
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
            string id = parts[0];



            //second string is world size
            string worldSize = parts[1];




            //foreach wall... do something. 
            foreach (string p in parts)
            {
              


                startupData.Add(p);

                //  remove it from the SocketState's growable buffer... check if this is needed
                state.RemoveData(0, p.Length);
            }
            //inform the view
        }






    }
}
