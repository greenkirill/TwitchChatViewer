using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TwitchChatViewer.JSON;
using System.IO;
using System.Windows;
using System.Text.RegularExpressions;
using System.Diagnostics;
using TwitchChatViewer.TwitchChatIrc;
using System.Windows.Documents;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Threading;

namespace TwitchChatViewer.Collection {


    public class TwitchCollectionAlpha {

        private string BaseName = "EmoteList.db3";

        private Process twitchEmotesProcess;

        public string Chanel { get; private set; }
        public bool BTTV { get; private set; }



        private Action<string> privmsg_callback;

        public Action after_update { get; set; }

        private bool update = false;

        public bool TwitchEmotesLoad { get; private set; } = false;
        public bool TwitchBadgesLoad { get; private set; } = false;
        public bool TwitchBitsLoad { get; private set; } = false;
        public bool BTTVEmotesLoad { get; private set; } = false;

        protected string client_id = "9tre3zr325pj4zjkwfmfyq3mg1jjgc";

        public TwitchCollectionAlpha(string chanel, bool bttv) {
            Chanel = chanel;
            BTTV = bttv;
        }

        public void Update() {
            UpdateTwitchEmotesList();
        }


        public void UpdateTwitchEmotesList() {
            if (update)
                return;
            update = true;
            twitchEmotesProcess = new Process();
            twitchEmotesProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            twitchEmotesProcess.StartInfo.FileName = "emoticons.exe";
            twitchEmotesProcess.StartInfo.Arguments = " -e";
            twitchEmotesProcess.Start();
            twitchEmotesProcess.WaitForExit();
            after_update.Invoke();
            update = false;
        }


        public static void UpdateWithWait() {
            //if (update)
            //    return;
            //update = true;
            //var twitchEmotesProcess = new Process();
            //twitchEmotesProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            //twitchEmotesProcess.StartInfo.FileName = "emoticons.exe";
            //twitchEmotesProcess.Start();
            //twitchEmotesProcess.WaitForExit();
            //after_update_callback();
            //update = false;

        }

        public static bool DownloadEmoteN(int id, int N = 10) {
            if (File.Exists($"emotes/{id}.png"))
                return true;
            var url = $"http://static-cdn.jtvnw.net/emoticons/v1/{id}/1.0";
            for (int i = 0; i < N; i++) {
                if (DownloadEmote(url, "emotes/" + id + ".png"))
                    return true;
            }
            return false;
        }

        public static bool DownloadEmoteV1(int id) {
            if (File.Exists($"emotes/{id}.png"))
                return true;
            var url = $"http://static-cdn.jtvnw.net/emoticons/v1/{id}/1.0";
            return DownloadEmote(url, "emotes/" + id + ".png");
        }

        public static bool DownloadEmoteV1(TwitchEmoteMessage emote) {
            return DownloadEmoteV1(emote.Emote.Id);
        }


        private static bool DownloadEmote(string uri, string fileName) {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            HttpWebResponse response;
            try {
                response = (HttpWebResponse)request.GetResponse();
            } catch (Exception e) {
                SLog.Log("ERROR_DownloadEmote", uri, fileName);
                SLog.Log("ERROR_DownloadEmote", e.ToString());
                return false;
            }

            if ((response.StatusCode == HttpStatusCode.OK ||
                response.StatusCode == HttpStatusCode.Moved ||
                response.StatusCode == HttpStatusCode.Redirect) &&
                response.ContentType.StartsWith("image", StringComparison.OrdinalIgnoreCase)) {
                var flag = 0;
                while (flag < 100) {
                    try {
                        using (Stream inputStream = response.GetResponseStream())
                        using (Stream outputStream = File.OpenWrite(fileName)) {
                            byte[] buffer = new byte[4096];
                            int bytesRead;
                            do {
                                bytesRead = inputStream.Read(buffer, 0, buffer.Length);
                                outputStream.Write(buffer, 0, bytesRead);
                            } while (bytesRead != 0);
                        }
                        return true;
                    } catch { flag++; }
                }
                return false;
            } else {
                SLog.Log("DEBUG_DownloadEmote", uri, response.StatusCode.ToString(), response.ToString(), response.ContentType.ToString());
                return false;
            }
        }
    }
}
