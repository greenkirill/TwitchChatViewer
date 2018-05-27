using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchChatViewer.TwitchChatIrc {
    public class TwitchBadge {

        public string Name { get; private set; }
        public string Version { get; private set; }

        public TwitchBadge(string str) {
            var slash_split = str.Split('/');
            Name = slash_split[0];
            Version = slash_split[1];
        }

        public TwitchBadge(string name, string version) {
            Name = name;
            Version = version;
        }

        public override string ToString() {
            return Name + '/' + Version;
        }
    }
}
