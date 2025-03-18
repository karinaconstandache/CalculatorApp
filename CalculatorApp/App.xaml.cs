using System;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace CalculatorApp
{
    public partial class App : Application
    {
        public static AppSettings Settings { get; private set; }
        private readonly string settingsFilePath = "appsettings.json";

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            LoadSettings();

            if (Settings.LastMode == "Programmer")
            {
                var programmerWindow = new ProgrammerWindow();
                programmerWindow.Show();
            }
            else
            {
                var mainWindow = new MainWindow();
                mainWindow.Show();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            SaveSettings();
            base.OnExit(e);
        }

        private void LoadSettings()
        {
            if (File.Exists(settingsFilePath))
            {
                try
                {
                    string json = File.ReadAllText(settingsFilePath);
                    Settings = JsonSerializer.Deserialize<AppSettings>(json);
                }
                catch
                {
                    Settings = GetDefaultSettings();
                }
            }
            else
            {
                Settings = GetDefaultSettings();
            }
        }

        private void SaveSettings()
        {
            string json = JsonSerializer.Serialize(Settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(settingsFilePath, json);
        }

        private AppSettings GetDefaultSettings()
        {
            return new AppSettings
            {
                IsDigitGroupingEnabled = true,  
                LastMode = "Standard",           
                LastBase = 10                   
            };
        }
    }
}

