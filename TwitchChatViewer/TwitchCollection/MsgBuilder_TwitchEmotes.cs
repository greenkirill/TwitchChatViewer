using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchChatViewer.TwitchChatIrc;
using System.IO;
using System.Net;
using System.Threading;

namespace TwitchChatViewer {
    class MsgBuilder_TwitchEmotes : MsgBuilder {

        private bool correct = true;

        public string Path { get; set; } = "emotes";

        public override Privmsg EditMsg(Privmsg msg, PrivmsgCommand pc) {
            foreach (var emote in pc.Params.Emotes) {
                foreach (var range in emote.Ranges) {
                    msg = InsertEmoteIntoText(emote.Emote.Id, range.From, range.To, msg);
                }
            }
            return msg;
        }

        public Privmsg InsertEmoteIntoText(int id, int from, int to, Privmsg msg) {
            var offset = 0;
            var item = msg.Msg.First;
            do {
                if (offset + item.Value.Length > from) {
                    if (offset + item.Value.Length < to || item.Value is InlineEmote) {
                        correct = false;
                        return msg;
                    }
                    var before = (item.Value as InlineText).Text.Substring(0, from - offset);
                    var after = (item.Value as InlineText).Text.Substring(to - offset + 1, item.Value.Length - to + offset - 1);
                    if (before.Length == 0) {
                        item.Value = new InlineEmote(id, to - from + 1);
                        if (after.Length != 0) {
                            msg.Msg.AddAfter(item, new InlineText(after));
                        }
                    }
                    else {
                        (item.Value as InlineText).Text = before;
                        (item.Value as InlineText).Length = before.Length;
                        msg.Msg.AddAfter(item, new InlineEmote(id, to - from + 1));
                        if (after.Length != 0) {
                            msg.Msg.AddAfter(item.Next, new InlineText(after));
                        }
                    }
                    return msg;
                }
                offset += item.Value.Length;
            } while ((item = item.Next) != null);
            return msg;
        }
        
    }
}
