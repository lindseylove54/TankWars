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
    /// <summary>
    /// Author:  Tyler Amy, Lindsey Loveland
    /// Date: 11/23/2020
    /// </summary>
    public partial class Form1 : Form
    {
        private GameController controller;
        //the connect button
        private Button startButton;
        private TextBox nameText;
        private TextBox serverText;
        private DrawingPanel drawingPanel;
        private Label nameLabel;
        private Label serverLabel;
        private const int viewSize = 900;
        private const int menuSize = 70;



        /// <summary>
        /// The view constructor for the GUI of TankWars
        /// </summary>
        /// <param name="ctrl"></param>
        public Form1(GameController ctrl)
        {
            InitializeComponent();
            controller = ctrl;

            controller.UpdateArrived += OnFrame;
            controller.ConnectToView += connectFromController;
            controller.TankReceived += tankReceived;
            controller.connectID += playerIDReceived;
            this.FormClosing += Form1_FormClosing;
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
            serverLabel.Location = new Point(125, 10);
            serverLabel.Size = new Size(40, 15);
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
            controller.shootBeam += drawingPanel.beamHandler;

            // Set up key and mouse handlers
            this.KeyUp += HandleKeyUp;
            this.KeyDown += HandleKeyDown;
            drawingPanel.MouseMove += MouseMoveHandler;
            drawingPanel.MouseDown += HandleMouseClick;
            drawingPanel.MouseUp += HandleMouseUp;
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
            //Notify the drawing panel that the players tank was received.
            drawingPanel.tankReceived();
        }
        /// <summary>
        /// This method passes the world from the controller to the drawing panel 
        /// </summary>
        /// <param name="w"></param>
        private void connectFromController(World w)
        {
            //notify the drawing panel that the world has been received, and pass the world created in the controller to the panel
            drawingPanel.SetWorld(w);
        }
        /// <summary>
        /// This method passes the player id to the drawing panel
        /// </summary>
        /// <param name="id"></param>
        private void playerIDReceived(int id)
        {
            //send the player ID that was received from the server, to the drawing panel
            drawingPanel.setPlayerID(id);
        }
        /// <summary>
        /// Method for handling when the form is closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //handle the form closing 
            controller.HandleFormClosing(e);
        }

        /// <summary>
        /// Method for handling when the connect button is clicked. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void connectButton_Click(object sender, EventArgs e)
        {
            if (nameText.Text.Equals("") || nameText.Text.Equals(""))
            {
                MessageBox.Show("Must fill out both Player Name and Server Address", "Error!",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            //save the name written in the text box
            controller.playerName = nameText.Text;
            //connect to server 
            controller.Connect(serverText.Text);
            startButton.Enabled = false;
            nameText.Enabled = false;
            serverText.Enabled = false;

        }

        /// <summary>
        /// Method used for informing the controller the location of the mouse. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseMoveHandler(object sender, MouseEventArgs e)
        {
            //let the controller know that the mouse moved locations, and send the mouses location 
            controller.HandleMouseMovement(e.Location);
            
        }
        /// <summary>
        /// Method for handling when a key is pressed down.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W)
            {
                controller.HandleMoveRequest("up");
            }
            if (e.KeyCode == Keys.A)
            {
                controller.HandleMoveRequest("left");
            }
            if (e.KeyCode == Keys.S)
            {
                controller.HandleMoveRequest("down");
            }
            if (e.KeyCode == Keys.D)
            {
                controller.HandleMoveRequest("right");
            }
            //make sure the windows sound doesn't happen
            e.Handled = true;
            e.SuppressKeyPress = true;
        }
        /// <summary>
        /// Method for handling when a key is released
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            //make sure the windows sounds don't happen
            e.Handled = true;
            e.SuppressKeyPress = true;
        }
        /// <summary>
        /// Method for handling when the mouse is pressed 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                controller.HandleMouseClick("left");
            }

            if (e.Button == MouseButtons.Right)
            {
                controller.HandleMouseClick("right");
            }



        }
        /// <summary>
        /// Method for handling when the mouse is released 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleMouseUp(object sender, MouseEventArgs e)
        {
            controller.HandleMouseClick("none");
        }

       
        /// <summary>
        /// The drawing panel class is responsible for drawing the TankWars game, it contains methods that center the players view and draws all the 
        /// objects in the world 
        /// </summary>
        public class DrawingPanel : Panel
        {

            private World theWorld;
            //all the .png files used in our game
            Image walls;
            Image backGround;
            Image DarkTank;
            Image DarkTurret;
            Image BlueTank;
            Image BlueTurret;
            Image GreenTank;
            Image GreenTurret;
            Image OrangeTank;
            Image OrangeTurret;
            Image LightGreenTank;
            Image LightGreenTurret;
            Image PurpleTank;
            Image PurpleTurret;
            Image RedTank;
            Image RedTurret;
            Image YellowTank;
            Image YellowTurret;
            Image RedStar;
            Image BlueProj;
            Image Explosion;

            
            public bool receivedWorld;
            public bool receivedTank;
            private int playerID;
            //wallX and wallY are used for wall drawing. 
            private int wallX;
            private int wallY;
            //frameCount is only used for drawing beams for a short period of time
            private int frameCount;
            Dictionary<int,Beam> beams;

            public DrawingPanel()
            {
                walls = Image.FromFile("../../../Resources/Sprites/WallSprite.png");
                backGround = Image.FromFile("../../../Resources/Sprites/Background.png");

                DarkTank = Image.FromFile("../../../Resources/Sprites/DarkTank.png");
                DarkTurret = Image.FromFile("../../../Resources/Sprites/DarkTurret.png");
                BlueTank = Image.FromFile("../../../Resources/Sprites/BlueTank.png");
                BlueTurret = Image.FromFile("../../../Resources/Sprites/BlueTurret.png");
                GreenTank = Image.FromFile("../../../Resources/Sprites/GreenTank.png");
                GreenTurret = Image.FromFile("../../../Resources/Sprites/GreenTurret.png");
                OrangeTank = Image.FromFile("../../../Resources/Sprites/OrangeTank.png");
                OrangeTurret = Image.FromFile("../../../Resources/Sprites/OrangeTurret.png");
                LightGreenTank = Image.FromFile("../../../Resources/Sprites/LightGreenTank.png");
                LightGreenTurret = Image.FromFile("../../../Resources/Sprites/LightGreenTurret.png");
                PurpleTank = Image.FromFile("../../../Resources/Sprites/PurpleTank.png");
                PurpleTurret = Image.FromFile("../../../Resources/Sprites/PurpleTurret.png");
                RedTank = Image.FromFile("../../../Resources/Sprites/RedTank.png");
                RedTurret = Image.FromFile("../../../Resources/Sprites/RedTurret.png");
                YellowTank = Image.FromFile("../../../Resources/Sprites/YellowTank.png");
                YellowTurret = Image.FromFile("../../../Resources/Sprites/YellowTurret.png");


                RedStar = Image.FromFile("../../../Resources/Sprites/redStar.png");
                RedStar = Image.FromFile("../../../Resources/Sprites/redStar.png");
                BlueProj = Image.FromFile("../../../Resources/Sprites/shot_blue.png");
                Explosion = Image.FromFile("../../../Resources/Sprites/explosion.png");
                beams = new Dictionary<int, Beam>();
                frameCount = 0;

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

            /// <summary>
            /// Delegate method for drawing all the tanks
            /// </summary>
            /// <param name="o"></param>
            /// <param name="e"></param>
            private void tankDrawer(object o, PaintEventArgs e)
            {
                int tankWidth = 60;
                Tank t = o as Tank;
                Rectangle r = new Rectangle(-(tankWidth / 2), -(tankWidth / 2), tankWidth, tankWidth);
                //if the tankID is even
                if(t.TankID % 2 == 0)
                {
                    if(t.TankID % 8 == 0)
                    {
                        e.Graphics.DrawImage(DarkTank, r);
                    }
                    else if(t.TankID % 6 == 0)
                    {
                        e.Graphics.DrawImage(BlueTank, r);
                    }
                    else if(t.TankID % 4 == 0)
                    {
                        e.Graphics.DrawImage(RedTank, r);

                    }
                    else
                    {
                        e.Graphics.DrawImage(GreenTank, r);
                    }
                }
                //if the tankID is odd
                else 
                {
                    if(t.TankID % 7 == 0)
                    {
                        e.Graphics.DrawImage(YellowTank, r);

                    }
                    else if(t.TankID % 5 == 0)
                    {
                        e.Graphics.DrawImage(OrangeTank, r);
                    }
                    else if(t.TankID % 3 == 0)
                    {
                        e.Graphics.DrawImage(LightGreenTank, r);
                    }
                    else
                    {
                        e.Graphics.DrawImage(PurpleTank, r);
                    }
                }

                


            }
            /// <summary>
            /// Delegate method for drawing all the walls
            /// </summary>
            /// <param name="o"></param>
            /// <param name="e"></param>
            private void wallDrawer(object o, PaintEventArgs e)
            {
                int wallWidth = 50;
                Wall wall = o as Wall;
                Rectangle r = new Rectangle(-(wallWidth / 2), -(wallWidth / 2), wallWidth, wallWidth);

                e.Graphics.DrawImage(walls, r);
            }
            /// <summary>
            /// Delegate method for drawing all the powerups 
            /// </summary>
            /// <param name="o"></param>
            /// <param name="e"></param>
            private void powerDrawer(object o, PaintEventArgs e)
            {
                Powerup power = o as Powerup;
                Rectangle r = new Rectangle(-20, -20, 20, 20);

                e.Graphics.DrawImage(RedStar, r);
            }
            /// <summary>
            /// Delegate method for drawing the projectile 
            /// </summary>
            /// <param name="o"></param>
            /// <param name="e"></param>
            private void ProjectileDrawer(object o, PaintEventArgs e)
            {
                //what color do we want the projectiles?

                int width = 30;
                int height = 30;

                e.Graphics.DrawImage(BlueProj, -width / 2, -height / 2, width, height);

            }
            /// <summary>
            /// Delegate method for drawing the turret 
            /// </summary>
            /// <param name="o"></param>
            /// <param name="e"></param>
            private void turretDrawer(object o, PaintEventArgs e)
            {
                Tank t = o as Tank;
                int tankWidth = 60;
                Rectangle r = new Rectangle(-(tankWidth / 2), -(tankWidth / 2), tankWidth, tankWidth);
                //if the tank ID is even
                if (t.TankID % 2 == 0)
                {
                    if (t.TankID % 8 == 0)
                    {
                        e.Graphics.DrawImage(DarkTurret, r);
                    }
                    else if (t.TankID % 6 == 0)
                    {
                        e.Graphics.DrawImage(BlueTurret, r);
                    }
                    else if (t.TankID % 4 == 0)
                    {
                        e.Graphics.DrawImage(RedTurret, r);

                    }
                    else
                    {
                        e.Graphics.DrawImage(GreenTurret, r);
                    }
                }
                //if the tankID is odd
                else
                {
                    if (t.TankID % 7 == 0)
                    {
                        e.Graphics.DrawImage(YellowTurret, r);

                    }
                    else if (t.TankID % 5 == 0)
                    {
                        e.Graphics.DrawImage(OrangeTurret, r);
                    }
                    else if (t.TankID % 3 == 0)
                    {
                        e.Graphics.DrawImage(LightGreenTurret, r);
                    }
                    else
                    {
                        e.Graphics.DrawImage(PurpleTurret, r);
                    }
                }
            }
            /// <summary>
            /// Delegate method for drawing an explosion when a player dies
            /// </summary>
            /// <param name="o"></param>
            /// <param name="e"></param>
            private void explosionDrawer(object o, PaintEventArgs e)
            {
                Rectangle r = new Rectangle(-25, -25, 50, 50);
                e.Graphics.DrawImage(Explosion, r);
            }
            /// <summary>
            /// Delegate method for drawing the health bar, player name, and player score under the tank. 
            /// </summary>
            /// <param name="o"></param>
            /// <param name="e"></param>
            private void underTankDrawer(object o, PaintEventArgs e)
            {
                Tank t = o as Tank;
                int rectWidth = 90;
                int rectHeight = 15;
                string drawString = t.PlayerName + ":" + " " + t.PlayerScore;
                SolidBrush greenBrush = new SolidBrush(Color.DarkGreen);
                SolidBrush yellowBrush = new SolidBrush(Color.Yellow);
                SolidBrush redBrush = new SolidBrush(Color.Red);
                Pen pen = new Pen(Color.Black, 2);

                Font font = new Font("Arial", 12);
                SolidBrush brush = new SolidBrush(Color.Black);

                e.Graphics.DrawRectangle(pen, -50, -60, rectWidth, rectHeight);
                e.Graphics.DrawString(drawString, font, brush, -50, -40);
                //change how much the health bar is filled, and the color associated with how much health the tank has left
                switch (t.Health)
                {
                    case 3:
                        e.Graphics.FillRectangle(greenBrush, -50, -60, rectWidth, rectHeight);
                        break;
                    case 2:
                        e.Graphics.FillRectangle(yellowBrush, -50, -60, rectWidth * 2 / 3, rectHeight);
                        break;
                    case 1:
                        e.Graphics.FillRectangle(redBrush, -50, -60, rectWidth * 1 / 3, rectHeight);
                        break;
                }

            }
            /// <summary>
            /// Method for drawing beams
            /// </summary>
            /// <param name="o"></param>
            /// <param name="e"></param>
            private void beamDrawer(object o, PaintEventArgs e)
            {
                Beam beam = o as Beam;
                Pen pen = new Pen(Color.Red, 4);
                Point origin = new Point(0, 0);
                Point dest = new Point(0, -theWorld.worldSize * 2);
                
                e.Graphics.DrawLine(pen, origin, dest);
            }
            /// <summary>
            /// Method for drawing all the worlds objects.
            /// </summary>
            /// <param name="e"></param>
            protected override void OnPaint(PaintEventArgs e)
            {
                //make sure that the world and tank have been received before drawing anything
                if (receivedWorld == true && receivedTank == true)
                {

                    e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    
                    lock (theWorld)
                    {
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
                            //draw the walls
                            foreach (Wall wall in theWorld.Walls.Values)
                            {
                            //if both the 'x's are equal, draw the walls from the lower y, to the higher y
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
                                //if both the 'y's are equal, draw the walls from the lower x, to the higher x
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
                            //this list is for keeping track of a tank that has disconnected
                            List<Tank> disconnectedTank = new List<Tank>();
                            foreach (Tank tank in theWorld.Tanks.Values)
                            {
                                    //if the tank hasn't disconnected but it's health is gone, draw the explosion
                                if (tank.Health == 0 && tank.Disconnected == false)
                                {
                                    DrawObjectWithTransform(e, tank, theWorld.worldSize, tank.Location.GetX(), tank.Location.GetY(), 0, explosionDrawer);
                                    continue;
                                }
                                //if the tank is disconnected, add it to the list of disconnected tanks
                                else if (tank.Disconnected == true) { disconnectedTank.Add(tank); continue; }
                                //draw the tank
                                DrawObjectWithTransform(e, tank, theWorld.worldSize, tank.Location.GetX(), tank.Location.GetY(), tank.Orientation.ToAngle(), tankDrawer);
                                //draw the turret
                                DrawObjectWithTransform(e, tank, theWorld.worldSize, tank.Location.GetX(), tank.Location.GetY(), tank.Aiming.ToAngle(), turretDrawer);
                                //draw the health bar, playerName, and score under the tank
                                DrawObjectWithTransform(e, tank, theWorld.worldSize, tank.Location.GetX(), tank.Location.GetY() + 100, 0, underTankDrawer);

                            }
                            //remove disconnected tanks from the world
                            foreach (Tank deadTank in disconnectedTank)
                            {
                                theWorld.Tanks.Remove(deadTank.TankID);
                            }


                            //a list to keep track of the consumed powerups
                            List<Powerup> deadPower = new List<Powerup>();
                            foreach (Powerup power in theWorld.PowerUps.Values)
                            {
                                //add the powerup to the list of consumed powerups 
                                if (power.Died == true)
                                {
                                    deadPower.Add(power);
                                }
                                //draw the powerup
                                DrawObjectWithTransform(e, power, theWorld.worldSize, power.Location.GetX(), power.Location.GetY(), 0, powerDrawer);
                            }
                            //remove consumed powerups
                            foreach (Powerup power in deadPower)
                            {
                                theWorld.PowerUps.Remove(power.powerID);
                            }
                            //list to keep track of the projectiles that have been shot and consumed 
                            List<Projectile> deadProjectile = new List<Projectile>();

                            foreach (Projectile proj in theWorld.Projectiles.Values)
                            {
                                //if the projectile hits a wall, or another player, add it to the list of projectiles to remove 
                                if (proj.Died == true)
                                {
                                    deadProjectile.Add(proj);
                                }
                                //draw the projectile
                                DrawObjectWithTransform(e, proj, theWorld.worldSize, proj.Location.GetX(), proj.Location.GetY(), proj.Direction.ToAngle(), ProjectileDrawer);
                            }
                            //remove the consumed projectiles from the world 
                            foreach (Projectile proj in deadProjectile)
                            {
                                theWorld.Projectiles.Remove(proj.ProjID);
                            }
                            //list to add dead beams...aka beams that should no longer be in the world after fired
                            List<Beam> deadBeams = new List<Beam>();
                            foreach (Beam beam in beams.Values)
                            {
                            frameCount++;
                            //draw the beam for 20 frames
                            if (frameCount >= 20)
                            {
                                //reset the frame count and add the beam to the "dead beam 
                                frameCount = 0;
                                deadBeams.Add(beam);
                                continue;
                            }
                            //draw the beam
                            DrawObjectWithTransform(e, beam, theWorld.worldSize, beam.Origin.GetX(), beam.Origin.GetY(), beam.Direction.ToAngle(), beamDrawer);
                            }
                            //remove the dead beams
                            foreach(Beam b in deadBeams)
                            {
                            beams.Remove(b.BeamID);
                            }
                        base.OnPaint(e);
                    }
                }
                //this is called if the tank and world haven't both been received yet
                else
                {
                    return;
                }
            }


            /// <summary>
            /// set the drawing panel world equal to the world from the controller.
            /// </summary>
            /// <param name="w">world from the controller</param>
            public void SetWorld(World w)
            {
                theWorld = w;
                receivedWorld = true;
            }
            /// <summary>
            /// when this method is called it will set receivedTank to true, letting us know we can start drawing
            /// </summary>
            public void tankReceived()
            {
                receivedTank = true;
            }
            /// <summary>
            /// set the playerID in the drawing panel to the passed in id the server gave in the controller
            /// </summary>
            /// <param name="id"></param>
            public void setPlayerID(int id)
            {
                playerID = id;
            }
            /// <summary>
            /// if a beam is read from the server, add it to the list of beams to draw in this panel 
            /// </summary>
            /// <param name="b"></param>
            public void beamHandler(Beam b)
            {
                beams.Add(b.BeamID, b);
            }
           
        }
    }
}