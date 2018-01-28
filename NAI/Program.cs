using System;
using OpenCvSharp;

namespace NAI
{
    class Program
    {
        public const int FrameRate = 15;
        public const int SpaceKey = 32;

        public static void Main()
        {
            var videoCapture = new VideoCapture(CaptureDevice.Any);

            if (!videoCapture.IsOpened())
            {
                Console.WriteLine("Nie można zainicjować kamery.");
            }
            else
            {
                Console.WriteLine("Zainicjowano kamerę.");

                using (var window = new Window("Kamera"))
                using (var image = new Mat())
                {
                    while (true)
                    {
                        videoCapture.Read(image);
                        if (image.Empty())
                        {
                            Console.WriteLine("Kamera przestała odpowiadać.");
                            break;
                        }

                        window.ShowImage(image);

                        if (Cv2.WaitKey(FrameRate) == SpaceKey)
                        {

                        }
                    }
                }

            }

            Console.ReadKey();
        }
    }
}
