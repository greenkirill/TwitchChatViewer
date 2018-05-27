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

        private void Privmsg_action(PrivmsgCommand msg) {
            if (!twitchIrc.isRun)
                return;
            Parallel.ForEach(msg.Params.Emotes, a => {
                TwitchCollectionAlpha.DownloadEmoteV1(a);
            });
            var privmsg = new Privmsg(msg.Text);
            privmsg = msgBuilder_Collection.DoAllBuilders(privmsg, msg);
            Dispatcher.Invoke(() => {
                var tb = new PrivmsgTextBlock_Beta(privmsg, Channel, msg);
                MessageStackPanel.Children.Add(tb.Textblock);
                if (scrollViewerBindingSwitch)
                    MainScrollViewer.ScrollToBottom();
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


        private void Button_Click_1(object sender, RoutedEventArgs e) {
            foreach (var tb in tbList) {
                var last = tb.Textblock.Inlines.FirstInline;
                var next = tb.Textblock.Inlines.FirstInline;

                while ((next = last.NextInline) != null) {
                    if (next is InlineUIContainer) {
                        if ((((next as InlineUIContainer).Child as Image).Source as BitmapImage).UriSource.ToString() == "emotes/blank.png") {
                            var qq = 3;
                        }
                        tb.Textblock.Inlines.Remove(next);
                        tb.Textblock.Inlines.InsertAfter(last, tb.GetIUC(-1));
                    }
                    last = next;
                }


                //foreach (var item in tb.Textblock.Inlines) {
                //    if (item.GetType() == typeof(InlineUIContainer)) {
                //        tb.Textblock.Inlines.
                //        item = tb.GetIUC(-1);
                //    }
                //}
            }
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
            msgBuilder_Collection = new MsgBuilder_Collection();
            msgBuilder_Collection.BuilderList.Add(new MsgBuilder_TwitchEmotes());
            msgBuilder_Collection.BuilderList.Add(new MsgBuilder_TwitchBadges());
            var updater = new Updater("9tre3zr325pj4zjkwfmfyq3mg1jjgc", Channel);
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
