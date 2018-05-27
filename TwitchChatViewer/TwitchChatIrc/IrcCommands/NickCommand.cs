using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchChatViewer.TwitchChatIrc {
    class NickCommand : IrcCommand {
        public string Nick { get; private set; }
        public NickCommand(string nick) {
            Nick = nick; ;
        }

        public override string ToString() {
            return $"NICK {Nick}";
        }
    }
}
