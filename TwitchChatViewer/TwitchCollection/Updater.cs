using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Net;
using System.IO;

namespace TwitchChatViewer {
    class Updater {
        private static bool update_emotes = false;
        private static bool update_badges = false;
        private static bool update_bttv = false;

        protected string Client_id { get; private set; } = "9tre3zr325pj4zjkwfmfyq3mg1jjgc";
        protected string Channel { get; private set; }
        private bool TwitchBadges { get; set; } = false;
        protected bool Stop { get; set; } = false;

        private dynamic ChannelInfo { get; set; }

        public Updater(string clientId, string channel) {
            Channel = channel;
        }

        private bool DownloadImage(string uri, string fileName) {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            HttpWebResponse response;
            try {
                response = (HttpWebResponse)request.GetResponse();
            } catch (Exception) {
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
            } else
                return false;
        }


        public void InitChannelInfo() {
            var channelInfo_JsonStr = "";
            using (var webClient = new WebClient()) {
                webClient.Headers.Add("Client-ID", Client_id);
                channelInfo_JsonStr = webClient.DownloadString($"https://api.twitch.tv/kraken/channels/{Channel}");
            }
            ChannelInfo = JsonConvert.DeserializeObject(channelInfo_JsonStr);
        }

        public void DownloadBadges() {
            var downloadList = new List<Tuple<string, string>>();
            var globalBadges_JsonStr = "";
            using (var webClient = new WebClient()) {
                globalBadges_JsonStr = webClient.DownloadString($"https://badges.twitch.tv/v1/badges/global/display");
            }
            dynamic globalBadges_Json = JsonConvert.DeserializeObject(globalBadges_JsonStr);
            dynamic badgeSets = globalBadges_Json.badge_sets;
            foreach (var badge in badgeSets) {
                var name = badge.Name;
                var versions = badge.Value.versions;
                foreach (var version in versions) {
                    var url_1x = version.Value.image_url_1x.Value;
                    Directory.CreateDirectory("badges");
                    downloadList.Add(Tuple.Create(url_1x, "badges/" + name + "_" + version.Name + ".png"));
                }
            }

            var channelBadges_JsonStr = "";
            using (var webClient = new WebClient()) {
                channelBadges_JsonStr = webClient.DownloadString($"https://badges.twitch.tv/v1/badges/channels/{ChannelInfo._id.Value}/display");
            }
            dynamic channelBadges_Json = JsonConvert.DeserializeObject(channelBadges_JsonStr);
            dynamic badgeSetsC = channelBadges_Json.badge_sets;
            foreach (var badge in badgeSetsC) {
                var name = badge.Name;
                var versions = badge.Value.versions;
                foreach (var version in versions) {
                    var url_1x = version.Value.image_url_1x.Value;
                    Directory.CreateDirectory($"badges/{Channel}");
                    downloadList.Add(Tuple.Create(url_1x, $"badges/{Channel}/{name}_{version.Name}.png"));
                }
            }


            Parallel.ForEach(downloadList, a => {
                DownloadImage(a.Item1, a.Item2);
            });
        }

        void DownloadGlobalBadges() {
            var globalBadges_JsonStr = "";
            using (var webClient = new WebClient()) {
                globalBadges_JsonStr = webClient.DownloadString($"https://badges.twitch.tv/v1/badges/global/display");
            }
            dynamic globalBadges_Json = JsonConvert.DeserializeObject(globalBadges_JsonStr);
            dynamic badgeSets = globalBadges_Json.badge_sets;
            foreach (var badge in badgeSets) {
                var name = badge.Name;
                var versions = badge.Value.versions;
                foreach (var version in versions) {
                    var url_1x = version.Value.image_url_1x.Value;
                    Directory.CreateDirectory("badges");
                    DownloadImage(url_1x, "badges/" + name + "_" + version.Name + ".png");
                }
            }


        }

        void DownloadChannelBadges() {
            var channelBadges_JsonStr = "";
            using (var webClient = new WebClient()) {
                channelBadges_JsonStr = webClient.DownloadString($"https://badges.twitch.tv/v1/badges/channels/{ChannelInfo._id.Value}/display");
            }
            dynamic channelBadges_Json = JsonConvert.DeserializeObject(channelBadges_JsonStr);
            dynamic badgeSets = channelBadges_Json.badge_sets;
            foreach (var badge in badgeSets) {
                var name = badge.Name;
                var versions = badge.Value.versions;
                foreach (var version in versions) {
                    var url_1x = version.Value.image_url_1x.Value;
                    Directory.CreateDirectory($"badges/{Channel}");
                    DownloadImage(url_1x, $"badges/{Channel}/{name}_{version.Name}.png");
                }
            }
        }


        public void InitChanelInfo(string channel) {
            if (channel == "")
                return;
            using (var writer = new StreamWriter("info.txt")) {

                writer.Write(DownloadString($"https://api.twitch.tv/kraken/channels/{channel}"));
            }
        }



        private string DownloadString(string url) {
            using (var webClient = new WebClient()) {
                webClient.Headers.Add("Accept", "application/vnd.twitchtv.v5+json");
                webClient.Headers.Add("Client-ID", Client_id);
                try {
                    var q = webClient.DownloadData(url);
                    return System.Text.Encoding.Default.GetString(q);
                    return webClient.DownloadString(url);
                } catch (Exception e) {
                    return "";
                }
            }
        }

        private void GetBadgeToFile(string channel) {
            if (channel == "")
                return;
            using (var writer = new StreamWriter("badge.txt")) {
                writer.Write(DownloadString(channel));
            }
        }

        public List<(string, string, string)> DownloadBTTVEmotes(string channel = "") {
            var url = channel == "" ? "https://api.betterttv.net/2/emotes" : $"https://api.betterttv.net/2/channels/{channel}";
            var str = "";
            using (var webClient = new WebClient()) {
                str = webClient.DownloadString(url);
            }
            Directory.CreateDirectory($"emotes");
            var emts = new List<(string, string, string)>();
            dynamic ret = JsonConvert.DeserializeObject(str);
            if (ret.status != 200) return emts;
            dynamic emotes = ret.emotes;
            foreach (var emt in emotes) {
                emts.Add((emt.id, emt.code, emt.imageType));
            }
            Task.Run(() => {
                foreach (var emt in emotes) {
                    DownloadImage($"https://cdn.betterttv.net/emote/{emt.id}/1x",
                        $"emotes/{emt.id}.{emt.imageType}");
                }
            });
            return emts;
        }

        private string DownloadStringWOHeaders(string url) {
            using (var webClient = new WebClient()) {
                try {
                    var q = webClient.DownloadData(url);
                    return System.Text.Encoding.Default.GetString(q);
                    return webClient.DownloadString(url);
                } catch (Exception e) {
                    return "";
                }
            }
        }


    }
}
