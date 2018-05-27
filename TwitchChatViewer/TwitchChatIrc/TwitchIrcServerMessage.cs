using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchChatViewer.TwitchChatIrc {
    public class TwitchIrcServerMessage {

        public string BaseString { get; private set; }

        public IrcCommand Command { get; private set; }

        public TwitchIrcServerMessage(string str) {
            BaseString = str;
            var space_split = str.Split(' ');
            if (space_split[0] == "PING") {
                Command = new PingCommand(str);
                return;
            } 
            var spaceSplitWithoutAt = space_split.Where(x => x[0] != '@').ToArray();
            var command = spaceSplitWithoutAt[1];
            switch (command) {
                case "MODE":
                    Command = new ModeCommand(str);
                    break;
                case "PRIVMSG":
                    Command = new PrivmsgCommand(str);
                    break;
                default:
                    Command = new UnknownCommand(str);
                    break;
            }
            //Console.WriteLine(spaceSplitWithoutAt[1]);
        }

        public override string ToString() {
            return Command.ToString();
        }
    }
}
