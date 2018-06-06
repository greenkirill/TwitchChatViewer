using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchChatViewer.TwitchChatIrc;

namespace TwitchChatViewer {



    public abstract class InlineItem {
        public int Length { get; set; }

        public override string ToString() => base.ToString();
    }

    public class InlineText : InlineItem {
        public string Text { get; set; }

        public InlineText(string text) {
            Text = text;
            Length = text.Length;
        }

        public override string ToString() {
            return Text;
        }
    }

    public class InlineEmote : InlineItem {
        public int Id { get; set; }

        public InlineEmote(int id, int length) {
            Id = id;
            Length = length;
        }

        public override string ToString() {
            return $"{{TE_{Id}}}";
        }
    }

    public class InlineBTTVEmote : InlineItem {
        public string Id { get; set; }
        public string Ext { get; private set; }

        public InlineBTTVEmote(string id, int length, string ext = "png") {
            Id = id;
            Length = length;
            Ext = ext;
        }

        public override string ToString() {
            return $"{{BTTVE_{Id}}}";
        }
    }

    public class InlineBadge : InlineItem {
        public TwitchBadge Badge { get; set; }

        public InlineBadge(TwitchBadge badge, int length) {
            Badge = badge;
            Length = 0;
        }

        public override string ToString() {
            return $"{{TB_{Badge}}}";
        }
    }
}
