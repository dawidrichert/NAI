using System.Configuration;
using NAI.Models;

namespace NAI.Utils
{
    public class AppSettings
    {
        private const string SectionName = "appSettings";

        private const string HueMinKey = "HueMin";
        private const string HueMaxKey = "HueMax";
        private const string SaturationMinKey = "SaturationMin";
        private const string SaturationMaxKey = "SaturationMax";
        private const string ValueMinKey = "ValueMin";
        private const string ValueMaxKey = "ValueMax";
        private const string BlurKey = "Blur";
        private const string ErodeKey = "Erode";
        private const string DilateKey = "Dilate";

        public static int HueMin
        {
            get => GetNumberSetting(HueMinKey) ?? 80;
            set => SetSetting(HueMinKey, value);
        }

        public static int HueMax
        {
            get => GetNumberSetting(HueMaxKey) ?? 110;
            set => SetSetting(HueMaxKey, value);
        }

        public static int SaturationMin
        {
            get => GetNumberSetting(SaturationMinKey) ?? 0;
            set => SetSetting(SaturationMinKey, value);
        }

        public static int SaturationMax
        {
            get => GetNumberSetting(SaturationMaxKey) ?? 255;
            set => SetSetting(SaturationMaxKey, value);
        }

        public static int ValueMin
        {
            get => GetNumberSetting(ValueMinKey) ?? 0;
            set => SetSetting(ValueMinKey, value);
        }

        public static int ValueMax
        {
            get => GetNumberSetting(ValueMaxKey) ?? 255;
            set => SetSetting(ValueMaxKey, value);
        }

        public static int Blur
        {
            get => GetNumberSetting(BlurKey) ?? 3;
            set => SetSetting(BlurKey, value);
        }

        public static int Erode
        {
            get => GetNumberSetting(ErodeKey) ?? 5;
            set => SetSetting(ErodeKey, value);
        }

        public static int Dilate
        {
            get => GetNumberSetting(DilateKey) ?? 5;
            set => SetSetting(DilateKey, value);
        }

        public static void LoadControlPanelData(ControlPanelData controlPanelData)
        {
            controlPanelData.HsvModel.Hue.Min = HueMin;
            controlPanelData.HsvModel.Hue.Max = HueMax;
            controlPanelData.HsvModel.Saturation.Min = SaturationMin;
            controlPanelData.HsvModel.Saturation.Max = SaturationMax;
            controlPanelData.HsvModel.Value.Min = ValueMin;
            controlPanelData.HsvModel.Value.Max = ValueMax;
            controlPanelData.Blur = Blur;
            controlPanelData.Erode = Erode;
            controlPanelData.Dilate = Dilate;
        }

        public static void SaveControlPanelData(ControlPanelData controlPanelData)
        {
            HueMin = controlPanelData.HsvModel.Hue.Min;
            HueMax = controlPanelData.HsvModel.Hue.Max;
            SaturationMin = controlPanelData.HsvModel.Saturation.Min;
            SaturationMax = controlPanelData.HsvModel.Saturation.Max;
            ValueMin = controlPanelData.HsvModel.Value.Min;
            ValueMax = controlPanelData.HsvModel.Value.Max;
            Blur = controlPanelData.Blur;
            Erode = controlPanelData.Erode;
            Dilate = controlPanelData.Dilate;
        }

        private static int? GetNumberSetting(string key)
        {
            var value = GetSetting(key);
            if (value != null)
            {
                return int.Parse(GetSetting(key));
            }

            return null;
        }

        private static string GetSetting(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        private static void SetSetting(string key, int? value)
        {
            SetSetting(key, value.ToString());
        }

        private static void SetSetting(string key, string value)
        {
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            configuration.AppSettings.Settings.Remove(key);
            configuration.AppSettings.Settings.Add(key, value);
            configuration.Save();
            ConfigurationManager.RefreshSection(SectionName);
        }
    }
}
