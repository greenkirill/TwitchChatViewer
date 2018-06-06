using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TwitchChatViewer.TwitchChatIrc;

namespace TwitchChatViewer {
    class MsgBuilder_BTVEmotes : MsgBuilder {
        private Regex Reg { get; set; }
        private Dictionary<string, (string, string)> DicE { get; set; }

        public MsgBuilder_BTVEmotes(List<(string, string, string)> emts) {
            var reg = new StringBuilder();
            reg.Append("(");
            var dct = new Dictionary<string, (string, string)>();
            foreach (var emt in emts) {
                reg.Append(emt.Item2.Replace("(", @"\(").Replace(")", @"\)"));
                reg.Append("|");
                dct.Add(emt.Item2, (emt.Item1, emt.Item3));
            }
            reg.Length--;
            reg.Append(")");
            if (reg.ToString() == ")") reg = new StringBuilder();
            Reg = new Regex(reg.ToString());
            DicE = dct;
        }

        public override Privmsg EditMsg(Privmsg msg, PrivmsgCommand pc) {
            var item = msg.Msg.First;
            do {
                if (item.Value is InlineText) {
                    var mch = Reg.Matches((item.Value as InlineText).Text);
                    if (mch.Count > 0) {
                        InsertEmoteIntoText(DicE[mch[0].Value].Item1, mch[0].Index,
                            mch[0].Value.Length, DicE[mch[0].Value].Item2, item, msg.Msg);
                    }
                }
            } while ((item = item.Next) != null);
            return msg;
        }

        private void InsertEmoteIntoText(string id, int from, int length, string ext, LinkedListNode<InlineItem> it, LinkedList<InlineItem> ll) {
            if (from >= it.Value.Length || (from + length) > it.Value.Length) return;
            var before = (it.Value as InlineText).Text.Substring(0, from);
            var curent = new InlineBTTVEmote(id, length, ext);
            var after = new InlineText((it.Value as InlineText).Text.Substring(from + length, it.Value.Length - from - length));
            (it.Value as InlineText).Text = before;
            it.Value.Length = before.Length;
            ll.AddAfter(it, curent);
            ll.AddAfter(it.Next, after);
            if (after.Length == 0)
                ll.Remove(after);
            if (before.Length == 0)
                ll.Remove(it);
        }
    }
}
