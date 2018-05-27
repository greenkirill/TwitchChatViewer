using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TwitchChatViewer.TwitchChatIrc {
    public class TwitchIrc {
        public string Nick { get; private set; }
        public string Chanel { get; private set; }
        private string Pass { get; set; }

        private const string Host = "irc.chat.twitch.tv";
        private const int Port = 6667;

        private volatile bool retry = true;
        private object gate = new object();

        public bool isRun { get { return retry; } }

        public Action<PrivmsgCommand> on_privmsg { get; set; }
        
        public TwitchIrc(string nick, string pass, string chanel) {
            Nick = nick;
            Pass = pass;
            Chanel = chanel;
            on_privmsg = x => { };
        }
        

        public void AddPrivmsgCallback(Action<PrivmsgCommand> f) {
            on_privmsg += f;
        }

        private IrcServerConnection CreateConnect() {
            return new IrcServerConnection(Host, Port);
        }

        public void Stop() {
            lock (gate) {
                retry = false;
            }
        }

        public void Start() {
            lock (gate) {
                retry = true;
            }
            do {
                try {
                    using (var connect = CreateConnect().Open())
                    using (var pipe = connect.OpenPipe()) {
                        SpecificSet(pipe);
                        LogIn(pipe);
                        foreach (var line in pipe.ReadLines().Select(x => new TwitchIrcServerMessage(x))) {
                            if (line.Command is PrivmsgCommand) {
                                on_privmsg.Invoke(line.Command as PrivmsgCommand);
                            }
                            if (!retry)
                                break;
                        }
                    }
                }
                catch (Exception e) {
                    Console.WriteLine(e.ToString());
                }
            } while (retry);
        }

        private void SpecificSet(IrcPipe pipe, bool membership, bool commands, bool tags) {
            if (membership)
                pipe.Send(new CapabilityCommand("twitch.tv/membership"));
            if (commands)
                pipe.Send(new CapabilityCommand("twitch.tv/commands"));
            if (tags)
                pipe.Send(new CapabilityCommand("twitch.tv/tags"));
        }

        private void SpecificSet(IrcPipe pipe) {
            pipe.Send(
                new CapabilityCommand("twitch.tv/membership"),
                new CapabilityCommand("twitch.tv/commands"),
                new CapabilityCommand("twitch.tv/tags"));
        }


        private void LogIn(IrcPipe pipe) {
            pipe.Send(
                new PassCommand(Pass),
                new NickCommand(Nick),
                new JoinCommand(Chanel)
                );
        }
    }
}
