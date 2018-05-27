using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchChatViewer.TwitchChatIrc;

namespace TwitchChatViewer {
    public class Privmsg {

        public LinkedList<InlineItem> Badges { get; set; }
        public LinkedList<InlineItem> Msg { get; set; }
        

        public Privmsg(string msg) {
            Badges = new LinkedList<InlineItem>();
            Msg = new LinkedList<InlineItem>();
            Msg.AddFirst(new InlineText(msg));
        }


        public bool InsertImageIntoText(int id, int from, int to, string url) {
            //var offset = 0;
            //var item = Msg.First;
            //do {
            //    if (offset + item.Value.Length > from) {
            //        if (offset + item.Value.Length < to || item.Value is InlineImage)
            //            return false;
            //        var before = (item.Value as InlineText).Text.Substring(0, from - offset);
            //        var after = (item.Value as InlineText).Text.Substring(to - offset + 1, item.Value.Length - to + offset - 1);
            //        if (before.Length == 0) {
            //            item.Value = new InlineImage(url, to - from + 1);
            //            if (after.Length != 0) {
            //                Msg.AddAfter(item, new InlineText(after));
            //            }
            //        }
            //        else {
            //            (item.Value as InlineText).Text = before;
            //            (item.Value as InlineText).Length = before.Length;
            //            Msg.AddAfter(item, new InlineImage(url, to - from + 1));
            //            if (after.Length != 0) {
            //                Msg.AddAfter(item.Next, new InlineText(after));
            //            }
            //        }
            //        return true;
            //    }
            //    offset += item.Value.Length;
            //} while ((item = item.Next) != null);
            return true;
        }

        

        public override string ToString() {
            var str = "";
            var item = Msg.First;
            do {
                if (item.Value is InlineText) {
                    str += (item.Value as InlineText).Text;
                }
                else {
                    str += "IMAGE";
                }
            } while ((item = item.Next) != null);
            return str;
        }
    }
}
