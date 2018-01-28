using System;
using NAI.Models;
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
                using (var controlPanel = new Window("Panel sterowania"))
                using (var cameraImage = new Mat())
                using (var thresholdingImage = new Mat())
                using (var hsvImage = new Mat())
                {
                    var hsvModel = new HsvModel
                    {
                        Hue = new ModelItem
                        {
                            Min = 80,
                            Max = 110
                        },
                        Saturation = new ModelItem
                        {
                            Min = 0,
                            Max = 255
                        },
                        Value = new ModelItem
                        {
                            Min = 0,
                            Max = 255
                        }
                    };

                    cameraWindow.Move(0, 0);
                    hsvWindow.Move(videoCapture.FrameWidth, 0);
                    thresholdingWindow.Move(2 * videoCapture.FrameWidth, 0);
                    controlPanel.Move(0, videoCapture.FrameHeight + 32);
                    controlPanel.Resize(videoCapture.FrameWidth, videoCapture.FrameHeight);
                    WinApiUtils.SetWindowPosition(videoCapture.FrameWidth, videoCapture.FrameHeight + 32, videoCapture.FrameWidth + 16, videoCapture.FrameHeight + 40);
                    
                    controlPanel.CreateTrackbar("Min (H)", hsvModel.Hue.Min, 179, pos => hsvModel.Hue.Min = pos);
                    controlPanel.CreateTrackbar("Max (H)", hsvModel.Hue.Max, 179, pos => hsvModel.Hue.Max = pos);
                    controlPanel.CreateTrackbar("Min (S)", hsvModel.Saturation.Min, 255, pos => hsvModel.Saturation.Min = pos);
                    controlPanel.CreateTrackbar("Max (S)", hsvModel.Saturation.Max, 255, pos => hsvModel.Saturation.Max = pos);
                    controlPanel.CreateTrackbar("Min (V)", hsvModel.Value.Min, 255, pos => hsvModel.Value.Min = pos);
                    controlPanel.CreateTrackbar("Max (V)", hsvModel.Value.Max, 255, pos => hsvModel.Value.Max = pos);

                    while (true)
                    {
                        videoCapture.Read(cameraImage);
                        if (cameraImage.Empty())
                        {
                            Console.WriteLine("Kamera przestała odpowiadać.");
                            break;
                        }

                        Cv2.CvtColor(cameraImage, hsvImage, ColorConversionCodes.BGR2HSV);
                        Cv2.InRange(hsvImage, new Scalar(hsvModel.Hue.Min, hsvModel.Saturation.Min, hsvModel.Value.Min), new Scalar(hsvModel.Hue.Max, hsvModel.Saturation.Max, hsvModel.Value.Max), thresholdingImage);
                      
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
