using System;
using NAI.Utils;
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

                using (var cameraWindow = new Window("Kamera"))
                using (var hsvWindow = new Window("HSV"))
                using (var thresholdingWindow = new Window("Progowanie obrazu"))
                using (var cameraImage = new Mat())
                using (var thresholdingImage = new Mat())
                using (var hsvImage = new Mat())
                {
                    cameraWindow.Move(0, 0);
                    hsvWindow.Move(videoCapture.FrameWidth, 0);
                    thresholdingWindow.Move(2 * videoCapture.FrameWidth, 0);
                    WinApiUtils.SetWindowPosition(0, videoCapture.FrameHeight, videoCapture.FrameWidth, videoCapture.FrameHeight);

                    while (true)
                    {
                        videoCapture.Read(cameraImage);
                        if (cameraImage.Empty())
                        {
                            Console.WriteLine("Kamera przestała odpowiadać.");
                            break;
                        }

                        Cv2.CvtColor(cameraImage, hsvImage, ColorConversionCodes.BGR2HSV);
                        Cv2.InRange(hsvImage, new Scalar(0, 50, 123), new Scalar(20, 255, 255), thresholdingImage);
                      
                        cameraWindow.ShowImage(cameraImage);
                        hsvWindow.ShowImage(hsvImage);
                        thresholdingWindow.ShowImage(thresholdingImage);

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
