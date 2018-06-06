using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TwitchChatViewer.Collection;
using TwitchChatViewer.TwitchChatIrc;

namespace TwitchChatViewer {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private TwitchCollectionAlpha tc;
        private TwitchIrc twitchIrc;
        private MsgBuilder_Collection msgBuilder_Collection;
        private const int N = 50;
        private PrivmsgTextBlock_Beta[] messages = new PrivmsgTextBlock_Beta[N];
        private List<PrivmsgTextBlock_Beta> blanks = new List<PrivmsgTextBlock_Beta>();
        private int iter = 0;
        private string Channel = "c_a_k_e";
        public MainWindow() {
            InitializeComponent();
        }
        private bool scrollViewerBindingSwitch = true;

        private List<PrivmsgTextBlock_Alpha> tbList = new List<PrivmsgTextBlock_Alpha>();

        private void Button_Click(object sender, RoutedEventArgs e) {

            if (scrollViewerBindingSwitch)
                MainScrollViewer.ScrollToBottom();
        }

        private void ScrollbarSwitch_Click(object sender, RoutedEventArgs e) {
            scrollViewerBindingSwitch = !scrollViewerBindingSwitch;
        }

        private void BlankImprove(PrivmsgTextBlock_Beta pm) {
            foreach (var bl in pm.Blanks) {
                TwitchCollectionAlpha.DownloadEmoteN(bl.Item1);
            }
            Dispatcher.Invoke(() => {
                foreach (var bl in pm.Blanks) {
                    pm.Textblock.Inlines.Remove(bl.Item3);
                    var ret = pm.GetIUC($"emotes/{bl.Item1}.png");
                    pm.Textblock.Inlines.InsertAfter(bl.Item2, ret.Item1);
                }
            });
        }


        private void Privmsg_action(PrivmsgCommand msg) {
            if (!twitchIrc.isRun)
                return;
            Task.Run(() => {
                Task.Run(() => {
                    Parallel.ForEach(msg.Params.Emotes, a => {
                        TwitchCollectionAlpha.DownloadEmoteV1(a);
                    });
                });
                Thread.Sleep(50);
                var privmsg = new Privmsg(msg.Text);
                privmsg = msgBuilder_Collection.DoAllBuilders(privmsg, msg);
                Dispatcher.Invoke(() => {
                    var tb = new PrivmsgTextBlock_Beta(privmsg, Channel, msg);
                    try {
                        MessageStackPanel.Children.Remove(messages[iter].Textblock);
                    } catch { }
                    messages[iter] = tb;
                    MessageStackPanel.Children.Add(messages[iter].Textblock);
                    var iiter = iter;
                    if (!messages[iter].EmoteLoaded)
                        Task.Run(() => { BlankImprove(messages[iiter]); });
                    iter = (iter + 1) % N;
                    if (scrollViewerBindingSwitch)
                        MainScrollViewer.ScrollToBottom();
                });
            });
        }

        private void Callback2(string msg) {
            Dispatcher.Invoke(() => {
                var webBrowser = new WebBrowser();
                webBrowser.NavigateToString(msg);
                MessageStackPanel.Children.Add(webBrowser);
                if (scrollViewerBindingSwitch)
                    MainScrollViewer.ScrollToBottom();
            });
        }
        
        private void Button_Click_2(object sender, RoutedEventArgs e) {

            MessageStackPanel.Children.Clear();
            Channel = nickTB.Text.ToLower();

            InputsSwitch(false);
            MainScrollViewer.Margin = new Thickness(10, 10, 10, 10);
            twitchIrc = new TwitchIrc("prosto_bot", "oauth:2e42s7qtj8la153fx2vgzh7rtjajyh", Channel);
            twitchIrc.on_privmsg += Privmsg_action;
            var irc_thread = new Thread(twitchIrc.Start) {
                IsBackground = true
            };
            irc_thread.Start();
            var updater = new Updater("9tre3zr325pj4zjkwfmfyq3mg1jjgc", Channel);
            var bttvem1 = updater.DownloadBTTVEmotes();
            var bttvem2 = updater.DownloadBTTVEmotes(Channel);
            msgBuilder_Collection = new MsgBuilder_Collection();
            msgBuilder_Collection.BuilderList.Add(new MsgBuilder_TwitchEmotes());
            msgBuilder_Collection.BuilderList.Add(new MsgBuilder_TwitchBadges());
            msgBuilder_Collection.BuilderList.Add(new MsgBuilder_BTVEmotes(bttvem1));
            if (bttvem2.Count > 0)
                msgBuilder_Collection.BuilderList.Add(new MsgBuilder_BTVEmotes(bttvem2));
            Directory.CreateDirectory("emotes");
            var upTask = Task.Factory.StartNew(() => {
                updater.InitChannelInfo();
                updater.DownloadBadges();
            });
        }

        private void InputsSwitch(bool enable) {
            if (enable) {
                subutton.IsEnabled = true;
                subutton.Foreground = Brushes.Black;
                nickTB.IsReadOnly = false;
                nickTB.Foreground = Brushes.Black;
            } else {
                subutton.IsEnabled = false;
                subutton.Foreground = Brushes.Gray;
                nickTB.IsReadOnly = true;
                nickTB.Foreground = Brushes.Gray;
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e) {
            twitchIrc.Stop();
            MessageStackPanel.Children.Clear();
            InputsSwitch(true);
        }
    }

}
