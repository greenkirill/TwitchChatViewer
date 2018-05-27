using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchChatViewer.TwitchChatIrc {
    public class EmoteRange: IComparable<EmoteRange> {
        public int From { get; private set; }
        
        public int To { get; private set; }

        public TwitchEmote Emote { get; private set; }

        public EmoteRange(string str, TwitchEmote emote) {
            var hyphen_split = str.Split('-');
            From = Convert.ToInt32(hyphen_split[0]);
            To = Convert.ToInt32(hyphen_split[1]);
            Emote = emote;

        }
        public EmoteRange(int from, int to) {
            From = from;
            To = to;
            Emote = new TwitchEmote(1);
        }

        public override string ToString() {
            return $"{From}-{To}";
        }

        public int CompareTo(EmoteRange obj) {
            return From - obj.From;
        }
    }
}
