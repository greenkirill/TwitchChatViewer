using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchChatViewer.TwitchChatIrc {
    class CapabilityCommand : IrcCommand {

        public bool Req { get; private set; }
        public string Text { get; private set; }

        public CapabilityCommand(bool req, string text) {
            Req = req;
            Text = text;
        }
        public CapabilityCommand(string text) {
            Req = true;
            Text = text;
        }

        public override string ToString() {
            return $"CAP {(Req ? "REQ " : "" )}:{Text}"; 
        }
    }
}
