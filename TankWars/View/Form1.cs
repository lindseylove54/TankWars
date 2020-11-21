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
            
            controller.UpdateArrived += OnFrame;
            controller.ConnectToView += connectFromController;
            controller.TankReceived += tankReceived;
            controller.connectID += playerIDReceived;


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
            nameText = new TextBox();
            nameText.Text = "player";
            nameText.Location = new Point(50, 5);
            nameText.Size = new Size(70, 15);
            this.Controls.Add(nameText);


            // Place and add the drawing panel
            drawingPanel = new DrawingPanel();
            drawingPanel.Location = new Point(0, menuSize);
            drawingPanel.Size = new Size(viewSize, viewSize);
            this.Controls.Add(drawingPanel);

            // Set up key and mouse handlers
            this.KeyUp += HandleKeyUp;
            this.KeyDown += HandleKeyDown;
            drawingPanel.MouseMove+= MouseMoveHandler;
            //drawingPanel.MouseUp += HandleMouseUp;
        }

        /// <summary>
        /// Handler for the controller's UpdateArrived eventw
        /// </summary>
        private void OnFrame()
        {
            //Invalidate this form and all its children
            //This will cause the form to redraw as soon as it can
            //method invoker
            MethodInvoker invalidator = new MethodInvoker(() => this.Invalidate(true));
            this.Invoke(invalidator);
        }
        /// <summary>
        /// Delegate for the controller to inform the view that the players tank has been received. 
        /// </summary>
        private void tankReceived()
        {
            drawingPanel.tankReceived();
        }
        private void connectFromController(World w)
        {
            drawingPanel.SetWorld(w);
        }
        private void playerIDReceived(int id)
        {
            drawingPanel.setPlayerID(id);
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
            startButton.Enabled = false;
            nameText.Enabled = false;
            serverText.Enabled = false;
        }

        private void MouseMoveHandler(object sender, MouseEventArgs e)
        {
            controller.HandleMouseMovement(e.Location);
        }
        private void HandleKeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.W)
            {
                controller.HandleMoveRequest("up");
            } 
            if(e.KeyCode == Keys.A)
            {
                controller.HandleMoveRequest("left");
            }
            if(e.KeyCode == Keys.S)
            {
                controller.HandleMoveRequest("down");
            }
            if(e.KeyCode == Keys.D)
            {
                controller.HandleMoveRequest("right");
            }
            e.Handled = true;
        }
        private void HandleKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    controller.HandleMoveRequest("none");
                    break;
                case Keys.A:
                    controller.HandleMoveRequest("none");
                    break;
                case Keys.S:
                    controller.HandleMoveRequest("none");
                    break;
                case Keys.D:
                    controller.HandleMoveRequest("none");
                    break;
            }
            

        }


        public class DrawingPanel : Panel
        {

            private World theWorld;
            Image walls;
            Image backGround;
            Image DarkTank;
            Image DarkTurret;
            Image RedStar;
            public bool receivedWorld;
            public bool receivedTank;
            private int playerID;
            private int wallX;
            private int wallY;
            public DrawingPanel()
            {
                walls = Image.FromFile("../../../Resources/Sprites/WallSprite.png"); 
                backGround = Image.FromFile("../../../Resources/Sprites/Background.png");
                DarkTank = Image.FromFile("../../../Resources/Sprites/DarkTank.png");
                DarkTurret = Image.FromFile("../../../Resources/Sprites/DarkTurret.png");
                RedStar = Image.FromFile("../../../Resources/Sprites/redStar.png");
                DoubleBuffered = true;
                
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
            
            private void tankDrawer(object o, PaintEventArgs e)
            {
                int tankWidth = 60;
                Tank t = o as Tank;
                Rectangle r = new Rectangle(-(tankWidth / 2 - 4), -(tankWidth / 2), tankWidth, tankWidth);
                e.Graphics.DrawImage(DarkTank, r);
                Rectangle turret = new Rectangle(-(tankWidth / 2), -(tankWidth / 2), tankWidth - 10 , tankWidth - 10);

                e.Graphics.DrawImage(DarkTurret, turret);
            }

            private void wallDrawer(object o, PaintEventArgs e)
            {
                int wallWidth = 50;
                Wall wall = o as Wall;
                Rectangle r = new Rectangle(-(wallWidth / 2), -(wallWidth / 2), wallWidth, wallWidth);
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                e.Graphics.DrawImage(walls, r);
            }
            private void powerDrawer(object o, PaintEventArgs e)
            {
                Powerup power = o as Powerup;
                Rectangle r = new Rectangle(-20,-20,20,20);

                e.Graphics.DrawImage(RedStar, r);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                if (receivedWorld == true && receivedTank == true)
                {

                    e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                    double playerX = theWorld.Tanks[playerID].Location.GetX();
                    double playerY = theWorld.Tanks[playerID].Location.GetY();

                    // calculate view/world size ratio
                    double ratio = (double)viewSize / (double)theWorld.worldSize;
                    int halfSizeScaled = (int)(theWorld.worldSize / 2.0 * ratio);

                    double inverseTranslateX = -WorldSpaceToImageSpace(theWorld.worldSize, playerX) + halfSizeScaled;
                    double inverseTranslateY = -WorldSpaceToImageSpace(theWorld.worldSize, playerY) + halfSizeScaled;

                    e.Graphics.TranslateTransform((float)inverseTranslateX, (float)inverseTranslateY);

                    //draw the background
                    e.Graphics.DrawImage(backGround, 0, 0, theWorld.worldSize, theWorld.worldSize);
                    lock(theWorld)
                    { 
                        foreach (Wall wall in theWorld.Walls.Values)
                    {
                        if (wall.P1.GetX() == wall.P2.GetX())
                        {
                            wallX = (int)wall.P1.GetX();
                            if (wall.P1.GetY() > wall.P2.GetY())
                            {
                                for (wallY = (int)wall.P2.GetY(); wallY <= wall.P1.GetY(); wallY += 50)
                                {
                                    DrawObjectWithTransform(e, wall, theWorld.worldSize, wallX, wallY, 0, wallDrawer);
                                }
                            }
                            else
                            {
                                for (wallY = (int)wall.P1.GetY(); wallY <= wall.P2.GetY(); wallY += 50)
                                {
                                    DrawObjectWithTransform(e, wall, theWorld.worldSize, wallX, wallY, 0, wallDrawer);
                                }
                            }
                        }
                        else
                        {
                            wallY = (int)wall.P1.GetY();
                            if (wall.P1.GetX() > wall.P2.GetX())
                            {
                                for (wallX = (int)wall.P2.GetX(); wallX <= wall.P1.GetX(); wallX += 50)
                                {
                                    DrawObjectWithTransform(e, wall, theWorld.worldSize, wallX, wallY, 0, wallDrawer);
                                }
                            }
                            else
                            {
                                for (wallX = (int)wall.P1.GetX(); wallX <= wall.P2.GetX(); wallX += 50)
                                {
                                    DrawObjectWithTransform(e, wall, theWorld.worldSize, wallX, wallY, 0, wallDrawer);
                                }
                            }
                        }
                    }
                    
                        foreach (Tank tank in theWorld.Tanks.Values)
                            //  if(tank hp == 0
                            DrawObjectWithTransform(e, tank, theWorld.worldSize, tank.Location.GetX(), tank.Location.GetY(), tank.Orientation.ToAngle(), tankDrawer);

                        List<Powerup> deadPower = new List<Powerup>();
                        foreach (Powerup power in theWorld.PowerUps.Values)
                        {
                            
                            if(power.Died == true)
                            {
                                deadPower.Add(power);
                            }
                            DrawObjectWithTransform(e, power, theWorld.worldSize, power.Location.GetX(), power.Location.GetY(), 0, powerDrawer);
                        }
                        foreach(Powerup power in deadPower)
                        {
                            theWorld.PowerUps.Remove(power.powerID);
                        }
                    }
                    base.OnPaint(e);
                }
                else
                {
                    return;
                }
            }

            public  void SetWorld(World w)
            {
                theWorld = w;
                receivedWorld = true;
            }
            public void tankReceived()
            {
                receivedTank = true;
            }
            public void setPlayerID(int id)
            {
                playerID = id;
            }
        }
    }
}