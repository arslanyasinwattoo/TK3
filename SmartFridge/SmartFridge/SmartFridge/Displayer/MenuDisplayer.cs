using System;
using Microsoft.SPOT;
using Gadgeteer.Modules.GHIElectronics;
using Microsoft.SPOT.Presentation.Media;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;

namespace SmartFridge.Displayer
{
    public class MenuDisplayer
    {
        private Gadgeteer.Modules.GHIElectronics.Joystick joystick;
        private Joystick.Position joystickPosition = new Joystick.Position();
        private Joystick.Position currentPosition = new Joystick.Position();
        private DisplayT35 display;
        private uint displayHeight, displayWidth = 0;
        private Bitmap cursor;
        private Bitmap homeScreen;        
        private const int SLD_CLR_OFFSET = 12;
        private const int BLK_RATE_OFFSET = 16;
        private const int HIT_TGT_CNT = 28;
        private const int MODE_OFFSET = 20;

        public event EventHandler OnButtonClicked;

        public Joystick.Position CurrentPosition
        {
            get { return currentPosition; }
        }

        public MenuDisplayer(Gadgeteer.Modules.GHIElectronics.Joystick joystick, DisplayT35 display)
        {
            this.joystick = joystick;
            this.joystick.JoystickReleased += joystick_JoystickReleased;
            this.display = display;
            
            displayWidth = (uint)display.Width - 7;
            displayHeight = (uint)display.Height - 7;

            cursor = new Bitmap(15, 15);
            cursor.Clear();
            cursor.MakeTransparent(Color.Black);
            cursor.DrawLine(Color.White, 1, 7, 0, 7, 14);
            cursor.DrawLine(Color.White, 1, 0, 7, 15, 7);        

            
        }

        void joystick_JoystickReleased(Joystick sender, Joystick.ButtonState state)
        {
            homeScreen.DrawRectangle(GT.Color.Green, 3, 70, 30, 200, 50, 3, 3, Color.Black, 0, 0, Color.Black, 0, 0, 0xff);            
            //homeScreen.DrawRectangle(GT.Color.Red, 3, 70, 100, 200, 50, 3, 3, Color.Black, 0, 0, Color.Black, 0, 0, 0xff);

            if (this.CurrentPosition.X >= 70 && this.CurrentPosition.X <= 270)
            {
                if (this.CurrentPosition.Y >= 30 && this.CurrentPosition.Y <= 80)
                {
                    if (OnButtonClicked != null)
                        OnButtonClicked("IN", null);
                }
                else if (this.CurrentPosition.Y >= 100 && this.CurrentPosition.Y <= 150)
                {
                    if (OnButtonClicked != null)
                        OnButtonClicked("OUT", null);
                }
                else if (this.CurrentPosition.Y >= 170 && this.CurrentPosition.Y <= 220)
                {
                    if (OnButtonClicked != null)
                        OnButtonClicked("CANCEL", null);
                }
            }

        }

        public void Start()
        {            
            currentPosition.X = displayWidth / 2.0;
            currentPosition.Y = displayHeight / 2.0;
            //MoveCursor();

            joystick.Calibrate();

            //this.joystick.JoystickReleased += joystick_JoystickReleased;
            //this.joystick.JoystickPressed += joystick_JoystickPressed;

            DrawHomeScreen();

            Gadgeteer.Timer timer = new Gadgeteer.Timer(100);
            timer.Tick += timer_Tick;
            timer.Start();
        }

        void timer_Tick(Gadgeteer.Timer timer)
        {
            double realX = 0, realY = 0;
            Joystick.Position newJoystickPosition = joystick.GetPosition();
            double newX = joystickPosition.X;// - .5 + currentPosition.X - currentPosition.X;
            double newY = joystickPosition.Y;// -.5 + currentPosition.Y - currentPosition.Y;
            joystickPosition = newJoystickPosition;

            // did we actually move...
            if (System.Math.Abs(newX) >= 0.03)
            {
                realX = newX;
                //display.SimpleGraphics.Clear();

            }
            if (System.Math.Abs(newY) >= 0.03)
            {
                realY = newY;
                //display.SimpleGraphics.Clear();

            }
            if (realX == 0.0 && realY == 0.0) return;

            if (realX + currentPosition.X >= displayWidth) currentPosition.X = 0;
            if (realX + currentPosition.X <= 0) currentPosition.X = displayWidth;
            if (realY + currentPosition.Y >= displayHeight) currentPosition.Y = 0;
            if (realY + currentPosition.Y <= 0) currentPosition.Y = displayHeight;

            Debug.Print(realX + " " + realY);
            currentPosition.X += realX * 5;//for increasing cursor speed
            currentPosition.Y += realY * 5;//for increasing cursoe speed

            DrawCursor();
        }

        private void DrawCursor()
        {
            display.SimpleGraphics.DisplayImage(homeScreen, 0, 0);
            display.SimpleGraphics.DisplayImage(cursor, (int)currentPosition.X, (int)currentPosition.Y);
        }

        private void DrawHomeScreen()
        {
            homeScreen = new Bitmap((int)display.Width, (int)display.Height);
            homeScreen.Clear();

            homeScreen.DrawRectangle(GT.Color.Green, 3, 70, 30, 200, 50, 3, 3, Color.Black, 0, 0, Color.Black, 0, 0, 0xff);
            homeScreen.DrawTextInRect("IN", 100, 45, 130, 120, Bitmap.DT_AlignmentCenter, GT.Color.White, Resources.GetFont(Resources.FontResources.NinaB));

            homeScreen.DrawRectangle(GT.Color.Red, 3, 70, 100, 200, 50, 3, 3, Color.Black, 0, 0, Color.Black, 0, 0, 0xff);
            homeScreen.DrawTextInRect("OUT", 100, 115, 130, 120, Bitmap.DT_AlignmentCenter, GT.Color.White, Resources.GetFont(Resources.FontResources.NinaB));

            homeScreen.DrawRectangle(GT.Color.White, 3, 70, 170, 200, 50, 3, 3, Color.Black, 0, 0, Color.Black, 0, 0, 0xff);
            homeScreen.DrawTextInRect("CANCEL", 100, 185, 130, 120, Bitmap.DT_AlignmentCenter, GT.Color.White, Resources.GetFont(Resources.FontResources.NinaB));

            display.SimpleGraphics.DisplayImage(homeScreen, 0, 0);
            cursor = new Bitmap(15, 15);
            cursor.Clear();
            cursor.MakeTransparent(Color.Black);
            cursor.DrawLine(Color.White, 1, 7, 0, 7, 14);
            cursor.DrawLine(Color.White, 1, 0, 7, 15, 7);
        }
                       
    }
}
