﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Threading;
using Emgu.CV.UI;
using System.Drawing;
using System.Diagnostics;
using static Football.ColourPalet;

namespace Football
{
    public class Ball
    {

        public struct BallPosition
        {
            public static int X { get; set; }
            public static int Y { get; set; }
            public static bool goingRight { get; set; }
        }


        //objects
        public GoalsChecker Gcheck { get; set; }

        //variables   
        public int Index { get; set; }
        public List<int> xCoordList = new List<int>();
        public Image<Bgr, byte> ImgOriginal { get; set; }
        public Image<Gray, byte> ImgFiltered { get; set; }
        public Image<Gray, byte> ImgGates { get; set; }
        //public Colour[] Colour { get; set; }
        public string PositionComment;
        public string at, bt;
        delegate int Distance(int A, int B);
        delegate void Print(List<int> list);
        Gates _gates = new Gates();

        public ColourPalet colourPalet = new ColourPalet();
        public ChooseColour chooseColour = new ChooseColour();


        private CircleF[] GetCircles(Image<Gray, byte> imgGray)
        {
            Gray grayCircle = new Gray(12);
            Gray cannyThreshold = new Gray(26);
            double lAccumResolution = 2.0;
            double minDistanceBtwCircles = 1.0;
            int minRadius = 0;
            int maxRadius = 10;
            return imgGray.HoughCircles(grayCircle, cannyThreshold, lAccumResolution, minDistanceBtwCircles, minRadius, maxRadius)[0];
        }

        public void BallPositionDraw(Image<Bgr, byte> imgCircles)
        {
            try
            {
                foreach (CircleF circle in GetCircles(ImgFiltered))
                {
                    BallPosition.X = (int)circle.Center.X;
                    Gcheck.StartStopwatch(BallPosition.X, ImgOriginal.Width);
                    Gcheck.Direction(BallPosition.X, Index, xCoordList); Index++;
                    imgCircles.Draw(circle, new Bgr(Color.Red), 3);
                }


                if (Index >= 5)
                {
                    xCoordList = xCoordList.Skip(Index - 4).ToList();
                    Index = 4;
                }


                int AGATES = _gates.FindAGates(ImgGates); // O <--
                int BGATES = _gates.FindBGates(ImgGates); // --> O

                
                int ABdistance = _gates.dist(AGATES, BGATES, ImgFiltered);

                Print print = delegate (List<int> list)   // delegate with generics
                {
                    IEnumerator<int> enumerator = list.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        int item = enumerator.Current;
                        Debug.WriteLine(item);
                    }
                    Debug.WriteLine(AGATES + "   <--->   " + BGATES + "   dist = " + ABdistance + " ballpos: " + BallPosition.X);
                };
                //print(xCoordList);



                PositionComment = getBallStatus(ABdistance, AGATES);
            }
            catch (Exception)
            {
                return;
            }
        }

        /*
        private int DistanceBetweenGates(int AGates, int BGates)
        {
            int result = BGates - AGates;
            if (result <= 0)
                return ImgOriginal.Width * 5 / 6;
            else
                return result;
        }

        private int FindAGates()
        {
            int i, j, red = 0, green = 0, blue = 0, counter = 0, X = 0;
            Color clr;
            int width = ImgGates.Width * 1 / 4;                      // 1/4  | 3/4
            int sheight = ImgGates.Height * 2 / 5;                   // 2/5  | 2/5
            int height = ImgGates.Height * 4 / 5;                    // 4/5  | 4/5
            Bitmap bmp = new Bitmap(ImgGates.Bitmap);

            for (i = 0; i < width; ++i)
            {
                for (j = sheight; j < height; j++)
                {
                    clr = bmp.GetPixel(i, j);
                    red = clr.R;
                    green = clr.G;
                    blue = clr.B;

                    if (red >= 245 && green >= 245 && blue >= 245)
                        counter++;
                    else
                        counter = 0;

                    if (counter >= 5)
                    {
                        X = i - 1;
                        return X;
                    }
                }
            }
            return X;
        }

        private int FindBGates()
        {
            int i, j, red = 0, green = 0, blue = 0, counter = 0, X = 0;
            Color clr;
            int width = ImgGates.Width * 3 / 4;
            int sheight = ImgGates.Height * 2 / 5;
            int height = ImgGates.Height * 4 / 5;
            Bitmap bmp = new Bitmap(ImgGates.Bitmap);

            for (i = width; i < ImgGates.Width; ++i)
            {
                for (j = sheight; j < height; j++)
                {
                    clr = bmp.GetPixel(i, j);
                    red = clr.R;
                    green = clr.G;
                    blue = clr.B;

                    if (red >= 245 && green >= 245 && blue >= 245)
                        counter++;
                    else
                        counter = 0;

                    if (counter >= 5)
                    {
                        X = i - 1;
                        return X;
                    }
                }
            }
            return X;
        }*/

        private string getBallStatus(int dist, int diff)
        {
            if (BallPosition.X <= (dist * 4 / 20 + diff))
            {
                return at + " Team Defenders";
            }
            else if (BallPosition.X >= (dist * 4 / 20 + diff) && BallPosition.X < (dist * 7 / 20 + diff))
            {
                return bt + " Team Attackers";
            }
            else if (BallPosition.X >= (dist * 7 / 20 + diff) && BallPosition.X < (dist * 10 / 20 + diff))
            {
                return at + " Team Middle 5";
            }
            else if (BallPosition.X >= (dist * 10 / 20 + diff) && BallPosition.X < (dist * 13 / 20 + diff))
            {
                return bt + " Team Middle 5";
            }
            else if (BallPosition.X >= (dist * 13 / 20 + diff) && BallPosition.X < (dist * 16 / 20 + diff))
            {
                return at + " Team Attackers";
            }
            else if (BallPosition.X >= (dist * 16 / 20 + diff) && BallPosition.X < (dist + diff))
            {
                return bt + " Team Defenders";
            }
            else
            {
                return "Unknown";
            }
        }

        private void Display(List<int> list)
        {
            IEnumerator<int> enumerator = list.GetEnumerator();
            while (enumerator.MoveNext())
            {
                int item = enumerator.Current;
                Debug.WriteLine(item);
            }
        }
    }
}