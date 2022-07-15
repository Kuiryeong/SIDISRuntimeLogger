using System;
using System.Text;

namespace SIDISRuntimeLogger.Models
{
    internal class LogPacketModel
    {
        public long RecievedTicks { get; set; }

        public byte[] ReceivedBytes { get; set; }

        public string FullMessage { get => Encoding.UTF8.GetString(ReceivedBytes); }

        public string LogString { get => FullMessage.Remove(0, 35); }

        public string WriteTime { get => new DateTime(RecievedTicks).ToString("yyyy-MM-dd HH:mm:ss.fff"); }

        public string ReceivedTime { get => FullMessage.Substring(12, 22); }

        public string CSTName { get => FullMessage.Substring(0, 12); }
    }
}
