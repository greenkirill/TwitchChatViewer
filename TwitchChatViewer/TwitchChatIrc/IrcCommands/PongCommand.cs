using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchChatViewer.TwitchChatIrc {
    class PongCommand : IrcCommand {
        public string Text { get; private set; }
        public PongCommand(string str) {
            Text = str;
        }
        public PongCommand(PingCommand ping) {
            Text = ping.Str;
        }

        public override string ToString() {
            return "PONG " + Text;
        }

    }
}
