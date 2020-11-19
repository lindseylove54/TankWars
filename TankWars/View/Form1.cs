using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tankwars;

namespace TankWars
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
        private GameController controller;
        private World theWorld;

        private Button startButton;
        private TextBox nameText;
        private TextBox serverText;
        private DrawingPanel drawingPanel;
        private Label nameLabel;
        private Label serverLabel;
        private const int viewSize = 900;
        private const int menuSize = 70;




        public Form1(GameController ctrl)
        {
            InitializeComponent();
            controller = ctrl;
            theWorld = new World();
            controller.UpdateArrived += OnFrame;

            ClientSize = new Size(viewSize, viewSize + menuSize);


            // Place and add the button
            startButton = new Button();
            startButton.Location = new Point(250, 5);
            startButton.Size = new Size(70, 20);
            startButton.Text = "Connect";
            startButton.Click += connectButton_Click;
            this.Controls.Add(startButton);

            // Place and add the name label
            nameLabel = new Label();
            nameLabel.Text = "Name:";
            nameLabel.Location = new Point(5, 10);
            nameLabel.Size = new Size(40, 15);
            this.Controls.Add(nameLabel);

            //Place and add the server label
            serverLabel = new Label();
            serverLabel.Text = "Server:";
            serverLabel.Location = new Point(125,10);
            serverLabel.Size = new Size(40,15);
            this.Controls.Add(serverLabel);


            // Place and add the server textbox
            serverText = new TextBox();
            serverText.Text = "localhost";
            serverText.Location = new Point(170, 5);
            serverText.Size = new Size(70, 15);
            this.Controls.Add(serverText);

            //Place and add the name textbox
            serverText = new TextBox();
            serverText.Text = "player";
            serverText.Location = new Point(50, 5);
            serverText.Size = new Size(70, 15);
            this.Controls.Add(serverText);


            // Place and add the drawing panel
            drawingPanel = new DrawingPanel(theWorld);
            drawingPanel.Location = new Point(0, menuSize);
            drawingPanel.Size = new Size(viewSize, viewSize);
            this.Controls.Add(drawingPanel);

            // Set up key and mouse handlers
            //this.KeyDown += HandleKeyDown;
            //this.KeyUp += HandleKeyUp;
            //drawingPanel.MouseDown += HandleMouseDown;
            //drawingPanel.MouseUp += HandleMouseUp;
        }

        /// <summary>
        /// Handler for the controller's UpdateArrived event
        /// </summary>
        private void OnFrame()
        {
            //Invalidate this form and all its children
            //This will cause the form to redraw as soon as it can
            this.Invalidate(true);
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            if (nameText.Text.Equals("") || nameText.Text.Equals(""))
            {
                MessageBox.Show("Must fill out both Player Name and Server Address", "Error!",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            controller.playerName = nameText.Text;
            controller.Connect(serverText.Text);
        }





        public class DrawingPanel : Panel
        {

            private World theWorld;

            public DrawingPanel(World w)
            {
                DoubleBuffered = true;
                theWorld = w;
            }

            // A delegate for DrawObjectWithTransform
            // Methods matching this delegate can draw whatever they want using e  
            public delegate void ObjectDrawer(object o, PaintEventArgs e);

            /// <summary>
            /// Helper method for DrawObjectWithTransform
            /// </summary>
            /// <param name="size">The world (and image) size</param>
            /// <param name="w">The worldspace coordinate</param>
            /// <returns></returns>
            private static int WorldSpaceToImageSpace(int size, double w)
            {
                return (int)w + size / 2;
            }

            /// <summary>
            /// This method performs a translation and rotation to drawn an object in the world.
            /// </summary>
            /// <param name="e">PaintEventArgs to access the graphics (for drawing)</param>
            /// <param name="o">The object to draw</param>
            /// <param name="worldSize">The size of one edge of the world (assuming the world is square)</param>
            /// <param name="worldX">The X coordinate of the object in world space</param>
            /// <param name="worldY">The Y coordinate of the object in world space</param>
            /// <param name="angle">The orientation of the objec, measured in degrees clockwise from "up"</param>
            /// <param name="drawer">The drawer delegate. After the transformation is applied, the delegate is invoked to draw whatever it wants</param>
            private void DrawObjectWithTransform(PaintEventArgs e, object o, int worldSize, double worldX, double worldY, double angle, ObjectDrawer drawer)
            {
                // "push" the current transform
                System.Drawing.Drawing2D.Matrix oldMatrix = e.Graphics.Transform.Clone();

                int x = WorldSpaceToImageSpace(worldSize, worldX);
                int y = WorldSpaceToImageSpace(worldSize, worldY);
                e.Graphics.TranslateTransform(x, y);
                e.Graphics.RotateTransform((float)angle);
                drawer(o, e);

                // "pop" the transform
                e.Graphics.Transform = oldMatrix;
            }
            private void worldDrawer(object o, PaintEventArgs e)
            {
                World world = o as World;

                Rectangle r = new Rectangle(-(world.worldSize / 2), (world.worldSize / 2), -world.worldSize, world.worldSize);
                Bitmap image = new Bitmap(@"C:\Users\tyler\Desktop\TankWars\Images\Background.png");

                e.Graphics.DrawImage(image, r);

            }
        }
    }
}