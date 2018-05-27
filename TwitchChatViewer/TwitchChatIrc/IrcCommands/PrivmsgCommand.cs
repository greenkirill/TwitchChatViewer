using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchChatViewer.TwitchChatIrc {
    public class PrivmsgCommand : IrcCommand {

        public PrivmsgTwitchParams Params { get; private set; }
        public string Chanel { get; private set; }
        public string Text { get; private set; }

        public PrivmsgCommand(string message) {
            var space_split = message.Split(' ');
            var colon_split = message.Split(':');
            Params = new PrivmsgTwitchParams(space_split[0]);
            Chanel = space_split[3].Remove(0, 1);
            From = space_split[1].Remove(0, 1);
            var start = space_split[0].Length + space_split[1].Length + space_split[2].Length + space_split[3].Length + 5;
            Text = message.Substring(start);
        }

        public override string ToString() {
            return $"{Params} :{From} PRIVMSG #{Chanel} :{Text}";
        }
    }
}
