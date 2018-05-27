using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchChatViewer.JSON {
    public class EmoteJSON {
        public int id { get; set; }
        public string regex { get; set; }
        public EmoteImageJSON[] images { get; set; }
    }
}
