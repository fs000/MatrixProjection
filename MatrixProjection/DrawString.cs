﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MatrixProjection {

    public class DrawString {

        // Ratio between charaters' width and height (in pixels)
        private const float X_OFFSET = 1.0f;

        private readonly int width, height;

        private readonly int totalPixels;

        private readonly StringBuilder frame;

        private ShadeChar currChar = ShadeChar.Full;

        public DrawString() {

            width = Console.WindowWidth;
            height = Console.WindowHeight - 1;

            totalPixels = width * height;

            frame = new StringBuilder(totalPixels);
        }

        public void NewFrame() {

            frame.Clear();

            for (int i = 0; i < totalPixels; i++) {

                frame.Append(' ');
            }
        }

        public void PlotPoint(Vector v) { // '\u25A0' -> OLD BLOCK | '\u2588' -> Full Block | '\u2593' -> Dark Shade | '\u2592' -> Medium Shade | '\u2591' -> Light Shade

            if (!OutOfBounds(v)) {

                int index = (int)v.X + (int)(-v.Y * width);

                frame[index] = (char)currChar;
            }
        }

        // Draws Mesh
        public void PlotMesh(Triangle[] projected) {

            for (int i = 0; i < projected.Length; i++) {

                if (BackfaceCulled(projected[i])) continue;

                for (int j = 0; j < projected[i].VertexCount; j++) {

                    projected[i].Vertices[j] = ConvertToScreen(projected[i].Vertices[j]);
                }

                for (int j = 0; j < projected[i].VertexCount; j++) {

                    // Fills every possible spot between two given points to form a line
                    PlotLine(projected[i].Vertices[j], projected[i].Vertices[(j + 1) % projected[i].VertexCount]);
                }
            }
        }

        // Draws Solid
        public void PlotFaces(Triangle[] projected) {

            for (int i = 0; i < projected.Length; i++) {

                if (BackfaceCulled(projected[i])) continue;

                for (int j = 0; j < projected[i].VertexCount; j++) {

                    projected[i].Vertices[j] = ConvertToScreen(projected[i].Vertices[j]);
                }

                bool flat = false;
                float prevY = -1;
                Triangle sorted = SortVertices(projected[i]);

                for (int j = 0; j < projected[i].VertexCount; j++) {

                    // Verify if triangle is flat
                    if ((int)projected[i].Vertices[j].Y == (int)projected[i].Vertices[(j + 1) % projected[i].VertexCount].Y) {

                        flat = true;

                        if (prevY > projected[i].Vertices[j].Y) {

                            FillBottomFlatTriangle(sorted.Vertices[0], sorted.Vertices[1], sorted.Vertices[2]);
                            break;

                        } else {

                            FillTopFlatTriangle(sorted.Vertices[0], sorted.Vertices[1], sorted.Vertices[2]);
                            break;
                        }

                    } else {

                        prevY = projected[i].Vertices[j].Y;
                    }
                }

                // if not, divide triangle into two flat, smaller triangles
                if (!flat) {

                    Vector v4 = new Vector(sorted.Vertices[0].X + (sorted.Vertices[1].Y - sorted.Vertices[0].Y) / 
                                          (sorted.Vertices[2].Y - sorted.Vertices[0].Y) * 
                                          (sorted.Vertices[2].X - sorted.Vertices[0].X), 
                                           sorted.Vertices[1].Y);

                    FillTopFlatTriangle(sorted.Vertices[0], sorted.Vertices[1], v4);
                    FillBottomFlatTriangle(sorted.Vertices[1], v4, sorted.Vertices[2]);
                }
            }
        }

        // Draws Solid with Shading - WIP
        public void PlotShadedFaces(Triangle[] projected, Light light) {

            //Vector lightDir = (light.Position - projected[i].Vertices[j]).Normalized;
            //float intensity = Math.Max(0.0f, Vector.DotProduct(projected[i].Normal, lightDir));

            //Shade(intensity);
        }

        private void FillTopFlatTriangle(Vector v1, Vector v2, Vector v3) {

            float invslope1 = (v2.X - v1.X) / (v2.Y - v1.Y);
            float invslope2 = (v3.X - v1.X) / (v3.Y - v1.Y);

            float curx1 = v1.X + 0.5f; // +0.5f adds small offset to not draw
            float curx2 = v1.X + 0.5f; // extra pixel on first top/bottom edge

            for (int scanlineY = (int)v1.Y; scanlineY <= v2.Y; scanlineY++) {

                PlotLine(new Vector(curx1, scanlineY), new Vector(curx2, scanlineY));
                curx1 += invslope1;
                curx2 += invslope2;
            }
        }

        private void FillBottomFlatTriangle(Vector v1, Vector v2, Vector v3) {

            float invslope1 = (v3.X - v1.X) / (v3.Y - v1.Y);
            float invslope2 = (v3.X - v2.X) / (v3.Y - v2.Y);

            float curx1 = v3.X + 0.5f; // +0.5f adds small offset to not draw
            float curx2 = v3.X + 0.5f; // extra pixel on first top/bottom edge

            for (int scanlineY = (int)v3.Y; scanlineY >= v1.Y; scanlineY--) {

                PlotLine(new Vector(curx1, scanlineY), new Vector(curx2, scanlineY));
                curx1 -= invslope1;
                curx2 -= invslope2;
            }
        }

        private void Shade(float lightIntensity) {

            if (lightIntensity == 0.0f) {

                currChar = ShadeChar.Null;

            } else if (lightIntensity < 0.25f) {

                currChar = ShadeChar.Low;

            } else if (lightIntensity < 0.5f) {

                currChar = ShadeChar.Medium;

            } else if (lightIntensity < 0.75f) {

                currChar = ShadeChar.High;

            } else {

                currChar = ShadeChar.Full;
            }
        }

        public void DrawFrame() {

            Console.SetCursorPosition(0, 0);
            Console.Write(frame);
        }

        private bool OutOfBounds(Vector v) {

            if (v.X >= width || v.X < 0 ||
               -v.Y >= height || -v.Y < 0) {

                return true;
            }

            return false;
        }

        public void AddText(Vector windowCoord, string text) {

            int index = (int)(windowCoord.X + (windowCoord.Y * width));

            for (int i = 0; i < text.Length; i++) {

                frame[index + i] = text[i];
            }
        }

        public void AddText(Vector windowCoord, char character) {

            int index = (int)(windowCoord.X + (windowCoord.Y * width));

            frame[index] = character;
        }

        // Does not reverse 'Y' value (actual console Y)
        // Reversed 'Y' value is only used at the time of drawing and out of bounds verification
        private Vector ConvertToScreen(Vector v) {

            return new Vector((int)((v.X * X_OFFSET) + (width / 2.0f)), (int)(v.Y - (height / 2.0f)));
        }

        private bool BackfaceCulled(Triangle polygon) {

            // Shoelace formula: https://en.wikipedia.org/wiki/Shoelace_formula#Statement
            // Division by 2 is not necessary, since all we care about is if the value is positive/negative

            float sum = 0.0f;

            for (int i = 0; i < polygon.VertexCount; i++) {

                sum += (polygon.Vertices[i].X * polygon.Vertices[(i + 1) % polygon.VertexCount].Y) - (polygon.Vertices[i].Y * polygon.Vertices[(i + 1) % polygon.VertexCount].X);
            }

            return sum >= 0;
        }

        // Insertion Sort Algorithm
        // https://en.wikipedia.org/wiki/Insertion_sort
        private Triangle SortVertices(Triangle polygon) {

            Triangle polygonCopy = polygon;

            for (int i = 1; i < polygonCopy.VertexCount; i++) {

                Vector chosen = polygonCopy.Vertices[i];
                int j = i - 1;

                while (j >= 0 && polygonCopy.Vertices[j].Y > chosen.Y) {

                    polygonCopy.Vertices[j + 1] = polygonCopy.Vertices[j];
                    j--;
                }

                polygonCopy.Vertices[j + 1] = chosen;
            }

            return polygonCopy;
        }

        #region Bresenham's Line Algorithm

        /* Wiki w/ pseudocode -> https://en.wikipedia.org/wiki/Bresenham%27s_line_algorithm#All_cases
         * 
         * Note: Better results with 'int' values than with 'float' (less jittery) */

        public void PlotLine(Vector from, Vector to) {

            if (Math.Abs((int)to.Y - (int)from.Y) < Math.Abs((int)to.X - (int)from.X)) {

                if ((int)from.X > (int)to.X) {

                    PlotLineLow(to, from);

                } else {

                    PlotLineLow(from, to);
                }

            } else {

                if ((int)from.Y > (int)to.Y) {

                    PlotLineHigh(to, from);

                } else {

                    PlotLineHigh(from, to);
                }
            }
        }

        private void PlotLineLow(Vector from, Vector to) {

            int dx = (int)to.X - (int)from.X;
            int dy = (int)to.Y - (int)from.Y;

            int yi = 1;

            if (dy < 0) {

                yi = -1;
                dy = -dy;
            }

            int D = (2 * dy) - dx;
            int y = (int)from.Y;

            for (int i = (int)from.X; i <= (int)to.X; i++) {

                PlotPoint(new Vector(i, y));

                if (D > 0) {

                    y += yi;
                    D += 2 * (dy - dx);

                } else {

                    D += 2 * dy;
                }
            }
        }

        private void PlotLineHigh(Vector from, Vector to) {

            int dx = (int)to.X - (int)from.X;
            int dy = (int)to.Y - (int)from.Y;

            int xi = 1;

            if (dx < 0) {

                xi = -1;
                dx = -dx;
            }

            int D = (2 * dx) - dy;
            int x = (int)from.X;

            for (int i = (int)from.Y; i <= (int)to.Y; i++) {

                PlotPoint(new Vector(x, i));

                if (D > 0) {

                    x += xi;
                    D += 2 * (dx - dy);

                } else {

                    D += 2 * dx;
                }
            }
        }

        #endregion
    }
}
