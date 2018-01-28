using OpenCvSharp;

namespace NAI.Utils
{
    public class HandDetector
    {
        public static void Run(Window window, Mat thresholdingImage)
        {
            var numberOfFingers = 0;

            Mat drawing = Mat.Zeros(thresholdingImage.Size(), MatType.CV_8UC3);
            Cv2.FindContours(thresholdingImage, out var contours, out var _, RetrievalModes.Tree, ContourApproximationModes.ApproxSimple);
            if (contours.Length > 0)
            {
                var contoursPoly = new Point[contours.Length][];
                for (var i = 0; i < contoursPoly.Length; i++)
                {
                    contoursPoly[i] = new Point[0];
                }
                var center = new Point2f[contours.Length];
                var radius = new float[contours.Length];

                for (var i = 0; i < contours.Length; i++)
                {
                    if (Cv2.ContourArea(contours[i]) >= 5000)
                    {
                        contoursPoly[i] = Cv2.ApproxPolyDP(contours[i], 3, true);
                        Cv2.MinEnclosingCircle(contoursPoly[i], out center[i], out radius[i]);
                        var tempContour = contours[i];

                        var hulls = new Point[1][];
                        var hullsI = new int[1][];
                        hulls[0] = Cv2.ConvexHull(tempContour);
                        hullsI[0] = Cv2.ConvexHullIndices(tempContour);

                        Cv2.DrawContours(drawing, hulls, -1, Scalar.Gold, 2);
                        if (hullsI[0].Length > 0)
                        {
                            var defects = Cv2.ConvexityDefects(tempContour, hullsI[0]);
                            if (defects.Length > 0)
                            {
                                if (radius[i] > 50)
                                {
                                    Cv2.DrawContours(drawing, contoursPoly, i, Scalar.Red);
                                    Cv2.Circle(drawing, center[i], (int)radius[i], Scalar.White, 2);
                                    Cv2.Circle(drawing, center[i], 5, Scalar.Red, 2);

                                    if (Program.ControlMode)
                                    {
                                        // Converting center x/y position into mouse x/y position
                                        WinApiUtils.SetCursorPos((int) (1366 / 400 * (center[i].X - 100)), (int) (800 / 200 * (center[i].Y - 200)));
                                    }
                                }

                                for (var j = 1; j < defects.Length; j++)
                                {
                                    var startIdx = defects[j][0];
                                    var ptStart = tempContour[startIdx];
                                    var farIdx = defects[j][2];
                                    var ptFar = tempContour[farIdx];

                                    if (Dist(ptStart, ptFar) > 1000 && ptStart.Y < center[i].Y && radius[i] >= 70)
                                    {
                                        Cv2.Circle(drawing, ptStart, 10, Scalar.Yellow, 2);
                                        Cv2.Line(drawing, ptStart, ptFar, Scalar.Pink, 2);

                                        numberOfFingers++;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            drawing.PutText($"Wykryte palce: {numberOfFingers}, Sterowanie: {(Program.ControlMode ? "TAK" : "NIE")}", new Point(10, 50), HersheyFonts.HersheyPlain, 2, new Scalar(255, 255, 255));
            window.ShowImage(drawing);
        }
    
        private static double Dist(Point x, Point y)
        {
            return (x.X - y.X) * (x.X - y.X) + (x.Y - y.Y) * (x.Y - y.Y);
        }
    }
}
