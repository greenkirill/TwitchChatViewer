using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchChatViewer.TwitchChatIrc {
    class JoinCommand : IrcCommand {
        public string Chanel { get; private set; }
        public JoinCommand(string chanel) {
            Chanel = chanel;
        }

        public override string ToString() {
            return $"JOIN #{Chanel}";
        }
    }
}
