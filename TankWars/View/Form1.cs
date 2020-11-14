using NetworkUtil;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace View
{
    /**
     * REQUIREMENTS FOR THE VIEW:
     *      Allow a player to declare a name and choose a server by IP address or host name.
     *      
     *      Provide a way to display connection errors and to retry connecting.
     *      
     *      Draw the scene, including the tanks, projectiles, etc...The first 8 players(at least) should be drawn with a unique color or graphic 
     *      that identifies that player. Beyond 8 players, you can reuse existing graphics or colors. The name, health, and score of each player should be displayed by their tank.
     *      
     *      Full credit will include polish and attention to detail, such as changing the color of the HP bar as it gets low, or an artistic way of drawing the beam attack and explosions.
     *      
     *      Your GUI should be able to keep up with the server and draw frames as fast as the server sends new information about the world, at least 60 frames per second.
     *      
     *      Follow the defined communication sequence and protocol (see below).
     *      
     *      Register keyboard and mouse handlers that recognize basic user inputs and invoke appropriate controller methods to process the inputs.
     * **/

    //
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            if (nameBox.Text.Equals("") || serverAddressBox.Text.Equals(""))
            {
                MessageBox.Show("Must fill out both Player Name and Server Address", "Error!",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            //if server address contains a semicolon, we know its an ip 
            else if (serverAddressBox.Text.Contains(":"))
            {
                string ip = serverAddressBox.Text.Substring(0, serverAddressBox.Text.IndexOf(':'));
                int port = int.Parse(serverAddressBox.Text.Substring(serverAddressBox.Text.IndexOf(':') + 1));
                //next create socket connection
                //Socket connection = Networking.ConnectToServer.....
            }

            //if just host name, create socket connection with that
            else
            {
                //Socket connection = Networking.ConnectToServer.....
            }
        }


    }
}
