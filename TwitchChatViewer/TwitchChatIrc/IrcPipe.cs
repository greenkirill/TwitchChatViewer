using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TwitchChatViewer.TwitchChatIrc {
    class IrcPipe : IDisposable {

        private NetworkStream stream;
        private StreamReader reader;
        private StreamWriter writer;

        public IrcPipe(NetworkStream stream) {
            this.stream = stream;
            reader = new StreamReader(stream);
            writer = new StreamWriter(stream) { NewLine = "\r\n", AutoFlush = true };
        }

        public void WriteLine(string str) {
            writer.WriteLine(str);
        }

        public IEnumerable<string> ReadLines() {
            while (true) {
                string line = reader.ReadLine();
                if (line != null) {
                    yield return line;
                }
            }
        }

        public void Send(params IrcCommand[] commands) {
            for (int i = 0; i < commands.Length; i++) {
                SLog.Log("DEBUG_IrcSend", commands[i].ToString());
                writer.WriteLine(commands[i].ToString());
            }
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing) {

                }

                stream.Dispose();
                reader.Dispose();
                writer.Dispose();

                disposedValue = true;
            }
        }

        ~IrcPipe() {
            Dispose(false);
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion


    }
}
