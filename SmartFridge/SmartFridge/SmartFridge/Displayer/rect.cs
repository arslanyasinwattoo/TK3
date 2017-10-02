using System;
using Microsoft.SPOT;

namespace SmartFridge.Displayer
{
    class rect
    {
        public int x;
        public int y;
        public int width;
        public int height;

        public rect(int x, int y, int width, int height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }


        public bool hitTest(int xPos, int yPos)
        {
            if (xPos >= x && yPos >= y && x + width >= xPos && y + height >= yPos)
                return true;
            return false;
        }
    }
}
