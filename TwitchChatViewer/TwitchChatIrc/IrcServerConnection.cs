using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TwitchChatViewer.TwitchChatIrc {
    class IrcServerConnection : IDisposable {

        public TcpClient Irc;


        public string Host { get; private set; }
        public int Port { get; private set; }


        public IrcServerConnection(string host, int port) {
            Host = host;
            Port = port;
        }

        public IrcServerConnection Open() {
            Irc = new TcpClient(Host, Port);
            return this;
        }
        
        public IrcPipe OpenPipe() {
            return new IrcPipe(Irc.GetStream());
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    
                }

                Irc.Dispose();

                disposedValue = true;
            }
        }
        
        ~IrcServerConnection() {
            Dispose(false);
        }
        
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

    }
}
