﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatrixProjection {

    class Program {

        static void Main(string[] args) {

            Draw draw = new Draw(120, 50, false);

            Vector v1 = new Vector(0, 0, 0);
            Vector v2 = new Vector(5, 5, 0); 
            Vector v3 = new Vector(5, -5, 0);
            Vector v4 = new Vector(-5, -5, 0);
            Vector v5 = new Vector(-5, 5, 0);

            Matrix3D m1 = new Matrix3D() {

                Matrix = new float[3, 3] {
                    {1, 0, 0},
                    {0, 1, 0},
                    {0, 0, 1}
                }
            };

            //Console.WriteLine(v1);
            //Console.WriteLine();
            //Console.WriteLine(m1);

            draw.DrawPoint(v1);
            draw.DrawPoint(v2);
            draw.DrawPoint(v3);
            draw.DrawPoint(v4);
            draw.DrawPoint(v5);

            Console.ReadKey();
        }
    }
}
