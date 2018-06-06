using TwitchChatViewer.TwitchChatIrc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TwitchChatViewer.Collection;

namespace TwitchChatViewer {
    class PrivmsgTextBlock_Beta {
        public TextBlock Textblock { get; private set; } = new TextBlock();

        public bool EmoteLoaded { get; set; } = false;

        public PrivmsgCommand Msg { get; private set; }

        public string Channel { get; private set; } = "";
        public PrivmsgTextBlock_Beta() { }

        public Privmsg PrivMsg { get; private set; } = new Privmsg("");

        public List<(int, Inline, Inline)> Blanks { get; private set; } = new List<(int, Inline, Inline)>();

        public PrivmsgTextBlock_Beta(Privmsg msg, string channel, PrivmsgCommand pc) {
            Channel = channel;
            Textblock = new TextBlock();
            Textblock.TextWrapping = TextWrapping.Wrap;
            // BADGES
            var badge = msg.Badges.First;

            while (badge != null) {
                var badge_value = badge.Value as InlineBadge;
                var path = "";
                if (File.Exists($"badges/{Channel}/{badge_value.Badge.Name}_{badge_value.Badge.Version}.png"))
                    path = $"badges/{Channel}/{badge_value.Badge.Name}_{badge_value.Badge.Version}.png";
                else
                    path = $"badges/{badge_value.Badge.Name}_{badge_value.Badge.Version}.png";
                var BadgeIUC = GetIUC(path, max: 18);
                Textblock.Inlines.Add(BadgeIUC.Item1);
                badge = badge.Next;
            }
            // NICKNAME
            var name = GetRun(" " + pc.Params.DisplayName + ": ");
            name.FontWeight = FontWeights.Bold;
            try {
                name.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(pc.Params.Color));
            } catch { }
            Textblock.Inlines.Add(name);

            // MESSAGE
            var item = msg.Msg.First;
            var prev = Textblock.Inlines.LastInline;
            while (item != null) {
                if (item.Value is InlineText) {
                    var item_value = item.Value as InlineText;
                    var ret = GetRun(item_value.Text);
                    Textblock.Inlines.Add(ret);
                    prev = Textblock.Inlines.LastInline;
                } else if (item.Value is InlineEmote) {
                    var item_value = item.Value as InlineEmote;
                    var ret = GetIUC($"emotes/{item_value.Id}.png");
                    Textblock.Inlines.Add(ret.Item1);
                    if (!ret.Item2) {
                        Blanks.Add((item_value.Id, prev, Textblock.Inlines.LastInline));
                    }
                    prev = Textblock.Inlines.LastInline;
                } else if (item.Value is InlineBTTVEmote) {
                    var item_value = item.Value as InlineBTTVEmote;
                    var ret = GetIUC($"emotes/{item_value.Id}.{item_value.Ext}");
                    Textblock.Inlines.Add(ret.Item1);
                    //if (!ret.Item2) {
                    //    Blanks.Add((item_value.Id, prev, Textblock.Inlines.LastInline));
                    //}
                    prev = Textblock.Inlines.LastInline;
                } else {
                    var item_value = item.Value;
                    var ret = GetRun(item_value.ToString());
                    Textblock.Inlines.Add(ret);
                    prev = Textblock.Inlines.LastInline;
                }
                item = item.Next;
            }

            Textblock.Margin = new Thickness(0, 0, 0, 5);
        }

        private static (int, int) GetWidthHeight(double width, double height, int max) {
            return width > height ? (max, (int)(height * max / width)) : ((int)(width * max / height), max);
        }


        public (InlineUIContainer, bool) GetIUC(string path, string blank_path = "emotes/blank.png", int max = 24) {
            try {
                Image imgMessage = new Image();
                BitmapImage bi = new BitmapImage(new Uri(path, UriKind.Relative));
                (imgMessage.Width, imgMessage.Height) = GetWidthHeight(bi.Width, bi.Height, max);
                imgMessage.Source = bi;
                imgMessage.Margin = new Thickness(0, 0, 0, -5);
                
                return (new InlineUIContainer(imgMessage), true);
            } catch (FileNotFoundException e) {
                SLog.Log("DEBUG_Blank", path);
                Image imgMessage = new Image();
                BitmapImage bi = new BitmapImage(new Uri(blank_path, UriKind.Relative));
                (imgMessage.Width, imgMessage.Height) = GetWidthHeight(bi.Width, bi.Height, max);

                imgMessage.Source = bi;
                EmoteLoaded = false;
                return (new InlineUIContainer(imgMessage), false);
            }
        }

        public Run GetRun(string text) {
            return new Run() { Text = text };
        }

    }
}
