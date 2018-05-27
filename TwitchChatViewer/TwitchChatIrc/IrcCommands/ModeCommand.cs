using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchChatViewer.TwitchChatIrc {
    class ModeCommand : IrcCommand {

        public string Chanel { get; private set; }
        public string Nick { get; private set; }
        public bool PM { get; private set; }
        public ModeCommand(string str) {
            var space_split = str.Split(' ');
            From = space_split[0].Remove(0, 1);
            Chanel = space_split[2].Remove(0, 1);
            PM = (space_split[3][0] == '+');
            Nick = space_split[4];
        }

        public ModeCommand(string from, string chanel, bool pm, string nick) {
            From = from;
            Chanel = chanel;
            PM = pm;
            Nick = nick;
        }

        public override string ToString() {
            return string.Join(" ", From, "MODE", "#" + Chanel, (PM ? "+" : "-") + "o", Nick);
        }


    }
}
