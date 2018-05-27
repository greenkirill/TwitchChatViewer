using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchChatViewer.TwitchChatIrc {
    class PingCommand : IrcCommand {

        public string Str { get; private set; }
       
        public PingCommand(string str) {
            var space_split = str.Split(' ');
            if (space_split[0] == "PING") {
                Str = space_split[1];
            }
            else {
                Str = space_split[0];
            }
        }
    }
}
