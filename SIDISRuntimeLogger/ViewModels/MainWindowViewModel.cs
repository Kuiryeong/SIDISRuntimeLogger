using SIDISRuntimeLogger.Commands;
using SIDISRuntimeLogger.Models;
using SIDISRuntimeLogger.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SIDISRuntimeLogger.ViewModels
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private LogCaptureService _logCaptureService;


        private bool isCaptureStarted;
        public bool IsCaptureStarted
        {
            get
            {
                return isCaptureStarted;
            }
            set
            {
                isCaptureStarted = value;
                OnPropertyChanged(nameof(IsCaptureStarted));
            }
        }

        private string fileNameToSave = @"D:\" + DateTime.Now.ToString("yyMMdd_") + "SIDISRuntimeLog.txt";
        public string FileNameToSave
        {
            get
            {
                return fileNameToSave;
            }
            set
            {
                fileNameToSave = value;
                OnPropertyChanged(nameof(FileNameToSave));
            }
        }

        private bool isSaveToFileChecked;
        public bool IsSaveToFileChecked
        {
            get
            {
                return isSaveToFileChecked;
            }
            set
            {
                isSaveToFileChecked = value;
                OnPropertyChanged(nameof(IsSaveToFileChecked));
            }
        }

        private string unitName;
        public string UnitName
        {
            get
            {
                return unitName;
            }
            set
            {
                unitName = value;
                OnPropertyChanged(nameof(UnitName));
            }
        }

        private ObservableCollection<LogPacketModel> logPackageModels;
        public ObservableCollection<LogPacketModel> LogPackageModels
        {
            get { return logPackageModels; }
            set
            {
                if (logPackageModels != value) return;
                logPackageModels = value;

                
            }
        }

        public ICommand StartCaptureCommand { get; }
        public ICommand StopCaptureCommand { get; }
        public ICommand ExportCommand { get; }
        public ICommand ClearDataCommand { get; }

        public MainWindowViewModel()
        {
            logPackageModels = new ObservableCollection<LogPacketModel>();

            StartCaptureCommand = new Command(StartCapturing);
            StopCaptureCommand = new Command(StopCapturing);
            ExportCommand = new Command(Export);
            ClearDataCommand = new Command(ClearData);
        }

        private void StartCapturing()
        {
            if (IsCaptureStarted) return;

            IsCaptureStarted = true;

            _logCaptureService?.Dispose();
            _logCaptureService = new LogCaptureService();
            _logCaptureService.PacketReceived += PacketCaptured;

            _logCaptureService.StartCaptureAsync(this);
        }

        private void StopCapturing()
        {
            if (_logCaptureService != null)
            {
                _logCaptureService.StopCapture();
                _logCaptureService.PacketReceived -= PacketCaptured;
            }

        }

        private void Export()
        {
            try
            {
                if (!string.IsNullOrEmpty(FileNameToSave) && LogPackageModels?.Count > 1)
                {
                
                    using (StreamWriter streamWriter = new StreamWriter(FileNameToSave, true))
                    {
                        for (int i = LogPackageModels.Count - 1; i > 0; i--)
                        {
                            LogPacketModel model = LogPackageModels[i];
                            streamWriter.WriteLineAsync($"{model.ReceivedTime} {model.CSTName} {model.LogString}");
                        }
                    }
                }
                
            }
            catch (Exception ex) { System.Windows.Forms.MessageBox.Show(ex.ToString()); }
        }

        private void ClearData()
        {
            LogPackageModels?.Clear();
        }

        private void PacketCaptured(object sender, LogPacketModel model)
        {
            Application.Current.Dispatcher.Invoke(() => { LogPackageModels.Insert(0, model); });

            Application.Current.Dispatcher.Invoke(() => { UnitName = model.CSTName; });

            if (IsSaveToFileChecked && !string.IsNullOrEmpty(FileNameToSave))
            {
                try
                {
                    using (StreamWriter streamWriter = new StreamWriter(FileNameToSave, true))
                    {
                        streamWriter.WriteLine($"{model.ReceivedTime} {model.CSTName} {model.LogString}");
                    }
                }
                catch (Exception ex) { return; }
            }
        }
    }
}
