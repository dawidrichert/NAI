using System;
using NAI.Models;
using NAI.Utils;
using OpenCvSharp;

namespace NAI
{
    internal class Program
    {
        public const int FrameRate = 15;
        public const int SpaceKey = 32;
        public const int SKey = 115;
        public static bool ControlMode;

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
                using (var controlPanelWindow = new Window("Panel sterowania"))
                using (var handWindow = new Window("Wykrywanie dłoni"))
                using (var cameraImage = new Mat())
                using (var thresholdingImage = new Mat())
                using (var hsvImage = new Mat())
                {
                    var controlPanelData = new ControlPanelData
                    {
                        HsvModel = new HsvModel
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
                        },
                        Blur = 3,
                        Erode = 5,
                        Dilate = 5
                    };

                    AppSettings.LoadControlPanelData(controlPanelData);
                    Console.WriteLine("Wczytano ustawienia.");

                    cameraWindow.Move(0, 0);
                    hsvWindow.Move(videoCapture.FrameWidth, 0);
                    thresholdingWindow.Move(2 * videoCapture.FrameWidth, 0);
                    controlPanelWindow.Move(0, videoCapture.FrameHeight + 32);
                    controlPanelWindow.Resize(videoCapture.FrameWidth, videoCapture.FrameHeight);
                    WinApiUtils.SetWindowPosition(videoCapture.FrameWidth, videoCapture.FrameHeight + 32, videoCapture.FrameWidth + 16, videoCapture.FrameHeight + 40);
                    handWindow.Move(2 * videoCapture.FrameWidth, videoCapture.FrameHeight + 32);

                    CreateControlPanelWindow(controlPanelWindow, controlPanelData);

                    while (true)
                    {
                        videoCapture.Read(cameraImage);
                        if (cameraImage.Empty())
                        {
                            Console.WriteLine("Kamera przestała odpowiadać.");
                            break;
                        }

                        Cv2.CvtColor(cameraImage, hsvImage, ColorConversionCodes.BGR2HSV);
                        Cv2.Blur(hsvImage, hsvImage, new Size(controlPanelData.Blur, controlPanelData.Blur));
                        Cv2.Flip(hsvImage, hsvImage, FlipMode.Y);

                        ThresholdImage(hsvImage, thresholdingImage, controlPanelData);
                        RemoveSmallObjectsFromTheForeground(thresholdingImage, controlPanelData);

                        cameraWindow.ShowImage(cameraImage);
                        hsvWindow.ShowImage(hsvImage);
                        thresholdingWindow.ShowImage(thresholdingImage);

                        HandDetector.Run(handWindow, thresholdingImage);
                        
                        switch (Cv2.WaitKey(FrameRate))
                        {
                            case SpaceKey:
                                ControlMode = !ControlMode;
                                break;
                            case SKey:
                                AppSettings.SaveControlPanelData(controlPanelData);
                                Console.WriteLine("Zapisano ustawienia.");
                                break;
                        }
                    }
                }
            }

            Console.ReadKey();
        }

        private static void RemoveSmallObjectsFromTheForeground(Mat thresholdingImage, ControlPanelData controlPanelData)
        {
            Cv2.Erode(thresholdingImage, thresholdingImage, Cv2.GetStructuringElement(MorphShapes.Ellipse, new Size(controlPanelData.Erode, controlPanelData.Erode)));
            Cv2.Dilate(thresholdingImage, thresholdingImage, Cv2.GetStructuringElement(MorphShapes.Ellipse, new Size(controlPanelData.Dilate, controlPanelData.Dilate)));
        }

        private static void ThresholdImage(Mat hsvImage, Mat thresholdingImage, ControlPanelData controlPanelData)
        {
            Cv2.InRange(hsvImage,
                new Scalar(controlPanelData.HsvModel.Hue.Min, controlPanelData.HsvModel.Saturation.Min, controlPanelData.HsvModel.Value.Min),
                new Scalar(controlPanelData.HsvModel.Hue.Max, controlPanelData.HsvModel.Saturation.Max, controlPanelData.HsvModel.Value.Max),
                thresholdingImage);
        }

        private static void CreateControlPanelWindow(Window window, ControlPanelData controlPanelData)
        {
            var hsvModel = controlPanelData.HsvModel;

            window.CreateTrackbar("Min (H)", hsvModel.Hue.Min, 179, pos => hsvModel.Hue.Min = pos);
            window.CreateTrackbar("Max (H)", hsvModel.Hue.Max, 179, pos => hsvModel.Hue.Max = pos);
            window.CreateTrackbar("Min (S)", hsvModel.Saturation.Min, 255, pos => hsvModel.Saturation.Min = pos);
            window.CreateTrackbar("Max (S)", hsvModel.Saturation.Max, 255, pos => hsvModel.Saturation.Max = pos);
            window.CreateTrackbar("Min (V)", hsvModel.Value.Min, 255, pos => hsvModel.Value.Min = pos);
            window.CreateTrackbar("Max (V)", hsvModel.Value.Max, 255, pos => hsvModel.Value.Max = pos);
            window.CreateTrackbar("Rozmycie", controlPanelData.Blur, 15, pos => controlPanelData.Blur = pos + 1);
            window.CreateTrackbar("Erozja", controlPanelData.Erode, 10, pos => controlPanelData.Erode = pos + 1);
            window.CreateTrackbar("Dylacja", controlPanelData.Dilate, 10, pos => controlPanelData.Dilate = pos + 1);
        }
    }
}
