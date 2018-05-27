using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchChatViewer.TwitchChatIrc {
    public class TwitchEmoteMessage {

        public TwitchEmote Emote { get; private set;  }
        public EmoteRange[] Ranges { get; private set; }

        public TwitchEmoteMessage(TwitchEmote emote, EmoteRange[] ranges) {
            Emote = emote;
            Ranges = ranges;
        }

        public TwitchEmoteMessage(string str) {
            var colon_split = str.Split(':');
            Emote = new TwitchEmote(Convert.ToInt32(colon_split[0]));
            var comma_split = colon_split[1].Split(',');
            Ranges = new EmoteRange[comma_split.Length];
            for (int i = 0; i < comma_split.Length; i++) 
                Ranges[i] = new EmoteRange(comma_split[i], Emote);
        }

        public override string ToString() {
            return Emote.Id + ":" + string.Join(",", Ranges.Select(x => x.ToString()));
        }
    }
}
