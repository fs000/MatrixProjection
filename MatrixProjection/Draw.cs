﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatrixProjection {

    public class Draw {

        private const float xOffset = 2.0f;

        private int consoleX, consoleY;

        public Draw(int x, int y, bool cursor) {

            consoleX = x;
            consoleY = y;

            Console.WindowWidth = consoleX;
            Console.WindowHeight = consoleY;

            Console.BufferWidth = consoleX;
            Console.BufferHeight = consoleY;

            Console.CursorVisible = cursor;
        }

        public void DrawPoint(Vector v) {

            Console.SetCursorPosition((int)((v.X * xOffset + consoleX / 2.0f)), -(int)(v.Y - consoleY / 2.0f));
            Console.Write('·');
        }

        private void DrawLine(Vector v1, Vector v2) {

            //float step = 10.0f;

            // 1. Subtract 'v2' by 'v1'
            // 2. Divide new Vector by 'step'
            // 3. Add newer, smaller Vector with 'v1' 'step' times; DrawPoint() after every addition
        }

        private bool OutOfBounds() {

            return false;
        }
    }
}
