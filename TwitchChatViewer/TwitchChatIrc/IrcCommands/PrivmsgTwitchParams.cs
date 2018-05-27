using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchChatViewer.TwitchChatIrc {
    public class PrivmsgTwitchParams {

        public TwitchBadge[] Badges { get; private set; }
        public TwitchEmoteMessage[] Emotes { get; private set; }
        public string Color { get; private set; }
        public string DisplayName { get; private set; }


        public string Id { get; private set; }
        public string TmiSentTs { get; private set; }
        public int RoomId { get; private set; }
        public int UserId { get; private set; }
        public string[] UserType { get; private set; }

        public bool Mod { get; private set; }
        public bool Subscriber { get; private set; }
        public bool Turbo { get; private set; }
        

        public PrivmsgTwitchParams(string str) {
            Badges = new TwitchBadge[0];
            Emotes = new TwitchEmoteMessage[0];
            if (str[0] == '@') 
                str = str.Remove(0, 1);
            var semicolon_split = str.Split(';');
            foreach (var param in semicolon_split) {
                var eq_split = param.Split('=');
                switch (eq_split[0]) {
                    case "badges":
                        if (eq_split[1] == "")
                            break;
                        var comma_split = eq_split[1].Split(',');
                        Badges = new TwitchBadge[comma_split.Length];
                        for (int i = 0; i < comma_split.Length; i++)
                            Badges[i] = new TwitchBadge(comma_split[i]);
                        break;
                    case "color":
                        Color = eq_split[1];
                        break;
                    case "display-name":
                        DisplayName = eq_split[1];
                        break;
                    case "emotes":
                        if (eq_split[1] == "")
                            break;
                        var slash_split = eq_split[1].Split('/');
                        Emotes = new TwitchEmoteMessage[slash_split.Length];
                        for (int i = 0; i < slash_split.Length; i++) 
                            Emotes[i] = new TwitchEmoteMessage(slash_split[i]);
                        break;
                    case "id":
                        Id = eq_split[1];
                        break;
                    case "mod":
                        Mod = (eq_split[1] == "1");
                        break;
                    case "room-id":
                        RoomId = Convert.ToInt32(eq_split[1]);
                        break;
                    case "subscriber":
                        Subscriber = (eq_split[1] == "1");
                        break;
                    case "tmi-sent-ts":
                        TmiSentTs = eq_split[1];
                        break;
                    case "turbo":
                        Turbo = (eq_split[1] == "1");
                        break;
                    case "user-id":
                        UserId = Convert.ToInt32(eq_split[1]);
                        break;
                    case "user-type":
                        UserType = eq_split[1].Split('/');
                        break;
                }
            }
        }
        
    }
}
