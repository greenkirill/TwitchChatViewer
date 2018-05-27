using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchChatViewer.TwitchChatIrc;
using System.IO;
using System.Net;
using System.Threading;
using TwitchChatViewer.TwitchChatIrc;

namespace TwitchChatViewer {
    class MsgBuilder_TwitchBadges :MsgBuilder {

        private bool correct = true;

        public string Path { get; set; } = "emotes";

        private string BaseName = "EmoteList.db3";

        public override Privmsg EditMsg(Privmsg msg, PrivmsgCommand pc) {
            foreach (var badge in pc.Params.Badges) {
                msg.Badges.AddLast(new InlineBadge(badge, 0));
            }
            return msg;
        }
    }
}
