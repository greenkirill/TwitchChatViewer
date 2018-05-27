using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchChatViewer.TwitchChatIrc {
    class UnknownCommand : IrcCommand {
        public string Text { get; private set; }

        public UnknownCommand(string text) {
            Text = text;
        }
    }
}
