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
    class PrivmsgTextBlock_Alpha {
        public TextBlock Textblock { get; private set; }
        public bool EmoteLoaded { get; set; }

        public List<string> TextList { get; private set; }
        public List<int> IdList { get; private set; }
        public PrivmsgCommand Msg { get; private set; }

        private TwitchCollectionAlpha TC { get; set; }

        public PrivmsgTextBlock_Alpha(PrivmsgTextBlock_Alpha ptb, TwitchCollectionAlpha tc) {
            var idList = ptb.IdList.Select(x => -1).ToList();
            var textList = ptb.TextList;
            var msg = ptb.Msg;
            TC = tc;
            EmoteLoaded = true;
            Textblock = new TextBlock();
            TextList = textList;
            IdList = idList;
            Msg = msg;

            Textblock.TextWrapping = TextWrapping.Wrap;
            var name = GetRun(msg.Params.DisplayName + ": ");
            name.FontWeight = FontWeights.Bold;
            try {
                name.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(msg.Params.Color));
            }
            catch { }
            Textblock.Inlines.Add(name);
            for (int i = 0; i < idList.Count; i++) {
                var run = GetRun(textList[i]);
                var iuc = GetIUC(idList[i]);
                Textblock.Inlines.Add(run);
                if (iuc != null)
                    Textblock.Inlines.Add(iuc);
            }
            Textblock.Inlines.Add(GetRun(textList[textList.Count - 1]));
        }

        public PrivmsgTextBlock_Alpha(PrivmsgCommand msg, TwitchCollectionAlpha tc) {
            InitTwoLists(msg);
            EmoteLoaded = true;
            Textblock = new TextBlock();
            Msg = msg;
            TC = tc;
            Textblock.TextWrapping = TextWrapping.Wrap;
            var name = GetRun(msg.Params.DisplayName + ": ");
            name.FontWeight = FontWeights.Bold;
            try {
                name.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom(msg.Params.Color));
            }
            catch { }
            Textblock.Inlines.Add(name);
            for (int i = 0; i < IdList.Count; i++) {
                var run = GetRun(TextList[i]);
                var iuc = GetIUC(IdList[i]);
                Textblock.Inlines.Add(run);
                if (iuc != null)
                    Textblock.Inlines.Add(iuc);
            }
            Textblock.Inlines.Add(GetRun(TextList[TextList.Count - 1]));
        }
        


        private static (int, int) GetWidthHeight(double width, double height, int max) {
            return width > height ? (max, (int)(height * max / width)) : ((int)(width * max / height), max);
        }

        public InlineUIContainer GetIUC(int id) {
            try {
                Image imgMessage = new Image();
                BitmapImage bi = new BitmapImage(new Uri("emotes/" + id + ".png", UriKind.Relative));
                (imgMessage.Width, imgMessage.Height) = GetWidthHeight(bi.Width, bi.Height, 24);

                imgMessage.Source = bi;
                imgMessage.Margin = new Thickness(0, 0, 0, -5);
                return new InlineUIContainer(imgMessage);
            }
            catch (FileNotFoundException e) {
                Image imgMessage = new Image();
                BitmapImage bi = new BitmapImage(new Uri("emotes/blank.png", UriKind.Relative));
                (imgMessage.Width, imgMessage.Height) = GetWidthHeight(bi.Width, bi.Height, 24);

                imgMessage.Source = bi;
                EmoteLoaded = false;

                var thread = new Thread(TC.UpdateTwitchEmotesList) {
                    IsBackground = true
                };
                thread.Start();
                return new InlineUIContainer(imgMessage);
                //DownloadEmote(id);
            }
        }
        public static Run GetRunS(string text) {
            return new Run() { Text = text };
        }
        public Run GetRun(string text) {
            return new Run() { Text = text };
        }

        

        private void InitTwoLists(PrivmsgCommand cmd) {
            var msg = cmd.Text;
            var rangeList = new List<EmoteRange>();
            foreach (var emote in cmd.Params.Emotes) {
                rangeList.AddRange(emote.Ranges);
            }
            rangeList.Sort();

            var textList = new List<string>();
            var idList = new List<int>();
            if (rangeList.Count == 0) {
                textList.Add(cmd.Text);
            }
            else {
                textList.Add(cmd.Text.Substring(0, rangeList[0].From));
                for (int i = 0; i < rangeList.Count - 1; i++) {
                    idList.Add(rangeList[i].Emote.Id);
                    textList.Add(cmd.Text.Substring(rangeList[i].To + 1, rangeList[i + 1].From - rangeList[i].To - 1));
                }
                idList.Add(rangeList[rangeList.Count - 1].Emote.Id);
                textList.Add(cmd.Text.Substring(rangeList[rangeList.Count - 1].To + 1, cmd.Text.Length - rangeList[rangeList.Count - 1].To - 1));

            }

            TextList = textList;
            IdList = idList;
        }
    }
}
