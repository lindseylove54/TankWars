﻿using System;
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

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            controller.HandleFormClosing(e);
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
            drawingPanel.setPlayerName(nameText.Text);
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
            e.Handled = true;
            e.SuppressKeyPress = true;
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
            e.Handled = true;
            e.SuppressKeyPress = true;
        }

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
        private void HandleMouseUp(object sender, MouseEventArgs e)
        {
            controller.HandleMouseClick("none");
        }

       

        public class DrawingPanel : Panel
        {

            private World theWorld;
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
            private int wallX;
            private int wallY;
            private string playerName;
            private int frameCount;
            private Dictionary<int,Beam> beams;
            private Random rnumber;
            private int turretColorNumber;
            private int colorNumber;
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
                rnumber = new Random();
                colorNumber = rnumber.Next(1, 9);
                turretColorNumber = colorNumber;
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
                Rectangle r = new Rectangle(-(tankWidth / 2), -(tankWidth / 2), tankWidth, tankWidth);
                if (colorNumber == 1) { e.Graphics.DrawImage(DarkTank, r);turretColorNumber = 1; }
                    if (colorNumber == 2) { e.Graphics.DrawImage(BlueTank, r); turretColorNumber = 2; }
                        if (colorNumber == 3) { e.Graphics.DrawImage(GreenTank, r); turretColorNumber = 3; }
                            if (colorNumber == 4) { e.Graphics.DrawImage(OrangeTank, r); turretColorNumber = 4; }
                                if (colorNumber == 5) { e.Graphics.DrawImage(LightGreenTank, r); turretColorNumber = 5; }
                                    if (colorNumber == 6) { e.Graphics.DrawImage(PurpleTank, r); turretColorNumber = 6; }
                                        if (colorNumber == 7) { e.Graphics.DrawImage(RedTank, r); turretColorNumber = 7; }
                                             if (colorNumber == 8){ e.Graphics.DrawImage(YellowTank, r); turretColorNumber = 8; }
                  
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
                Rectangle r = new Rectangle(-20, -20, 20, 20);

                e.Graphics.DrawImage(RedStar, r);
            }

            private void ProjectileDrawer(object o, PaintEventArgs e)
            {
                //what color do we want the projectiles?

                int width = 30;
                int height = 30;

                e.Graphics.DrawImage(BlueProj, -width / 2, -height / 2, width, height);

            }
            private void turretDrawer(object o, PaintEventArgs e)
            {
                int tankWidth = 60;
                Rectangle r = new Rectangle(-(tankWidth / 2), -(tankWidth / 2), tankWidth, tankWidth);
                if (turretColorNumber == 1) e.Graphics.DrawImage(DarkTurret, r);
                if (turretColorNumber == 2) e.Graphics.DrawImage(BlueTurret, r);
                if (turretColorNumber == 3) e.Graphics.DrawImage(GreenTurret, r);
                if (turretColorNumber == 4) e.Graphics.DrawImage(OrangeTurret, r);
                if (turretColorNumber == 5) e.Graphics.DrawImage(LightGreenTurret, r);
                if (turretColorNumber == 6) e.Graphics.DrawImage(PurpleTurret, r);
                if (turretColorNumber == 7) e.Graphics.DrawImage(RedTurret, r);
                if (turretColorNumber == 8) e.Graphics.DrawImage(YellowTurret, r);
            }
            private void explosionDrawer(object o, PaintEventArgs e)
            {
                Rectangle r = new Rectangle(-25, -25, 50, 50);
                e.Graphics.DrawImage(Explosion, r);
            }
            private void underTankDrawer(object o, PaintEventArgs e)
            {
                Tank t = o as Tank;
                int rectWidth = 90;
                int rectHeight = 15;
                string drawString = playerName + ":" + " " + t.PlayerScore;
                SolidBrush greenBrush = new SolidBrush(Color.LightGreen);
                SolidBrush yellowBrush = new SolidBrush(Color.Yellow);
                SolidBrush redBrush = new SolidBrush(Color.Red);
                Pen pen = new Pen(Color.Black, 2);

                Font font = new Font("Arial", 12);
                SolidBrush brush = new SolidBrush(Color.Black);

                e.Graphics.DrawRectangle(pen, -50, -60, rectWidth, rectHeight);
                e.Graphics.DrawString(drawString, font, brush, -50, -40);
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
            private void beamDrawer(object o, PaintEventArgs e)
            {
                Beam beam = o as Beam;
                Pen pen = new Pen(Color.Red, 4);
                Point origin = new Point(0, 0);
                Point dest = new Point(0, -theWorld.worldSize * 2);
                
                e.Graphics.DrawLine(pen, origin, dest);
            }
            protected override void OnPaint(PaintEventArgs e)
            {
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

                            List<Tank> disconnectedTank = new List<Tank>();
                            foreach (Tank tank in theWorld.Tanks.Values)
                            {
                                if (tank.Health == 0 && tank.Disconnected == false)
                                {
                                    DrawObjectWithTransform(e, tank, theWorld.worldSize, tank.Location.GetX(), tank.Location.GetY(), 0, explosionDrawer);
                                    continue;
                                }
                                else if (tank.Disconnected == true) { disconnectedTank.Add(tank); continue; }
                                DrawObjectWithTransform(e, tank, theWorld.worldSize, tank.Location.GetX(), tank.Location.GetY(), tank.Orientation.ToAngle(), tankDrawer);
                                DrawObjectWithTransform(e, tank, theWorld.worldSize, tank.Location.GetX(), tank.Location.GetY(), tank.Aiming.ToAngle(), turretDrawer);
                                DrawObjectWithTransform(e, tank, theWorld.worldSize, tank.Location.GetX(), tank.Location.GetY() + 100, 0, underTankDrawer);

                            }
                            foreach (Tank deadTank in disconnectedTank)
                            {
                                theWorld.Tanks.Remove(deadTank.TankID);
                            }



                            List<Powerup> deadPower = new List<Powerup>();
                            foreach (Powerup power in theWorld.PowerUps.Values)
                            {

                                if (power.Died == true)
                                {
                                    deadPower.Add(power);
                                }
                                DrawObjectWithTransform(e, power, theWorld.worldSize, power.Location.GetX(), power.Location.GetY(), 0, powerDrawer);
                            }
                            foreach (Powerup power in deadPower)
                            {
                                theWorld.PowerUps.Remove(power.powerID);
                            }
                            List<Projectile> deadProjectile = new List<Projectile>();
                            foreach (Projectile proj in theWorld.Projectiles.Values)
                            {
                                if (proj.Died == true)
                                {
                                    deadProjectile.Add(proj);
                                }
                                DrawObjectWithTransform(e, proj, theWorld.worldSize, proj.Location.GetX(), proj.Location.GetY(), proj.Direction.ToAngle(), ProjectileDrawer);
                            }
                            foreach (Projectile proj in deadProjectile)
                            {
                                theWorld.Projectiles.Remove(proj.ProjID);
                            }
                            List<Beam> deadBeams = new List<Beam>();
                            foreach (Beam beam in beams.Values)
                            {
                            frameCount++;
                            if (frameCount >= 20)
                            {
                                frameCount = 0;
                                deadBeams.Add(beam);
                                continue;
                            }
                            DrawObjectWithTransform(e, beam, theWorld.worldSize, beam.Origin.GetX(), beam.Origin.GetY(), beam.Direction.ToAngle(), beamDrawer);
                            }
                            foreach(Beam b in deadBeams)
                            {
                            beams.Remove(b.BeamID);
                            }
                        base.OnPaint(e);
                    }
                }
                else
                {
                    return;
                }
            }



            public void SetWorld(World w)
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
            public void beamHandler(Beam b)
            {
                beams.Add(b.BeamID, b);
            }
            public void setPlayerName(string name)
            {
                playerName = name;
            }
        }
    }
}