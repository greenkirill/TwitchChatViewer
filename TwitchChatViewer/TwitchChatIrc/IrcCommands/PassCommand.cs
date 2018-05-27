using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchChatViewer.TwitchChatIrc {
    class PassCommand : IrcCommand {
        public string Pass { get; private set; }
        public PassCommand(string pass) {
            Pass = pass;
        }

        public override string ToString() {
            return $"PASS {Pass}";
        }
    }
}
