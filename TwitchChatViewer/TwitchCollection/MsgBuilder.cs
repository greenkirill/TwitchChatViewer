using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchChatViewer.TwitchChatIrc;

namespace TwitchChatViewer {
    public abstract class MsgBuilder {
        virtual public Privmsg EditMsg(Privmsg msg, PrivmsgCommand pc) { return msg; }
    }

    public class MsgBuilder_Collection {
        public List<MsgBuilder> BuilderList { get; set; } = new List<MsgBuilder>();


        public Privmsg DoAllBuilders(Privmsg privmsg, PrivmsgCommand pc) {
            for (var i = 0; i < BuilderList.Count; i++)
                privmsg = BuilderList[i].EditMsg(privmsg, pc);
            return privmsg;
        }
    }

}
