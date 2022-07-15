using SIDISRuntimeLogger.Models;
using SIDISRuntimeLogger.ViewModels;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace SIDISRuntimeLogger.Services
{
    internal class LogCaptureService : IDisposable
    {
        private UdpClient _udpClient;

        private CancellationTokenSource _cancellSource;

        public event EventHandler<LogPacketModel> PacketReceived;
        
        public LogCaptureService()
        {
            try
            {
                _udpClient?.Dispose();
                _udpClient = new UdpClient(new IPEndPoint(IPAddress.Any, 1000));
            }
            catch (SocketException ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public async void StartCaptureAsync(MainWindowViewModel mainWindowViewModel)
        {
            _cancellSource = new CancellationTokenSource();

            try
            {
                while (!_cancellSource.Token.IsCancellationRequested)
                {
                    var udpReceiveResult = await _udpClient.ReceiveAsync(_cancellSource.Token);
                    byte[] rcvBytes = udpReceiveResult.Buffer;

                    OnPacketReceived(new LogPacketModel()
                    {
                        ReceivedBytes = rcvBytes,
                        RecievedTicks = DateTime.Now.Ticks
                    });
                }
            }
            catch (Exception ex) { }
            finally
            {
                _udpClient.Close();
                _udpClient.Dispose();

                mainWindowViewModel.IsCaptureStarted = false;
            }
        }

        public void Dispose()
        {
            _udpClient?.Dispose();
        }

        public void StopCapture()
        {
            _cancellSource?.Cancel();
        }

        public void OnPacketReceived(LogPacketModel packet)
        {
            this.PacketReceived?.Invoke(this, packet);
        }
    }
}
