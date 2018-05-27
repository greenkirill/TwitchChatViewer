using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchChatViewer.TwitchChatIrc;

namespace TwitchChatViewer {



    public abstract class InlineItem {
        public int Length { get; set; }
    }

    public class InlineText : InlineItem {
        public string Text { get; set; }

        public InlineText(string text) {
            Text = text;
            Length = text.Length;
        }
    }

    public class InlineEmote : InlineItem {
        public int Id { get; set; }

        public InlineEmote(int id, int length) {
            Id = id;
            Length = length;
        }
    }

    public class InlineBadge : InlineItem {
        public TwitchBadge Badge { get; set; }

        public InlineBadge(TwitchBadge badge, int length) {
            Badge = badge;
            Length = 0;
        }
    }
}
