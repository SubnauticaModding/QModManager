using FMODUnity;
using Harmony;
using Oculus.Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

namespace QModManager
{
    internal static class PirateCheck
    {
        private class Pirate : MonoBehaviour
        {
            private string videoURL;
            private const string VideoURLObtainer = "https://you-link.herokuapp.com/?url=https://www.youtube.com/watch?v=i8ju_10NkGY";

            private static readonly HashSet<string> BannedGameObjectNames = new HashSet<string>()
            {
                "Audio",
                "WorldCursor",
                "Default Notification Center",
                "InputHandlerStack",
                "SelectorCanvas",
                "Clip Camera"
            };

            private void Start()
            {
                Canvas canvas = gameObject.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                gameObject.AddComponent<RawImage>();

                FindObjectsOfType<AudioListener>().Do(l => DestroyImmediate(l));
                gameObject.AddComponent<AudioListener>().enabled = true;

                GetVideo();
                //DontDestroyOnLoad(this);
            }

            private void Update()
            {
                RuntimeManager.GetBus("bus:/master").setMute(true);
                UWE.Utils.alwaysLockCursor = true;
                UWE.Utils.lockCursor = true;
                foreach (GameObject go in SceneManager.GetActiveScene().GetRootGameObjects())
                {
                    if (BannedGameObjectNames.Contains(go.name)) DestroyImmediate(go);
                }
            }

            private void GetVideo()
            {
                if (!CheckConnection())
                {
                    ShowText();
                    return;
                }
                try
                {
                    ServicePointManager.ServerCertificateValidationCallback = VersionCheck.CustomRemoteCertificateValidationCallback;

                    using (WebClient client = new WebClient())
                    {
                        client.DownloadStringCompleted += (sender, e) =>
                        {
                            if (e.Error != null)
                            {
                                UnityEngine.Debug.LogException(e.Error);
                                ShowText();
                                return;
                            }
                            if (!ParseVideo(e.Result)) ShowText();
                        };

                        client.DownloadStringAsync(new Uri(VideoURLObtainer));
                    }
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogException(e);
                    ShowText();
                }
            }
            private bool ParseVideo(string result)
            {
                if (result == null)
                {
                    return false;
                }
                Dictionary<string, string>[] parsed;
                try
                {
                    parsed = JsonConvert.DeserializeObject<Dictionary<string, string>[]>(result);
                }
                catch
                {
                    return false;
                }
                if (parsed == null || parsed[0] == null)
                {
                    return false;
                }
                Dictionary<string, string> firstLink = parsed[0];
                if (!firstLink.TryGetValue("url", out string url))
                {
                    return false;
                }
                videoURL = url;

                StartCoroutine(PlayVideo());

                return true;
            }
            private void ShowText()
            {
                DestroyImmediate(gameObject.GetComponent<RawImage>());
                Text text = gameObject.AddComponent<Text>();
                text.text = $"An error has occured!\nQModManager couldn't be initialized.\nPlease turn on your internet connection,\nthen restart {(Patcher.game == Patcher.Game.Subnautica ? "Subnautica" : "Below Zero")}.";
                text.color = new Color(1, 0, 0);
                text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                text.fontStyle = FontStyle.BoldAndItalic;
                text.fontSize = 40;
            }

            private IEnumerator PlayVideo()
            {
                VideoPlayer videoPlayer = gameObject.GetComponent<VideoPlayer>() ?? gameObject.AddComponent<VideoPlayer>();
                AudioSource audioSource = gameObject.GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();

                videoPlayer.enabled = true;
                audioSource.enabled = true;

                videoPlayer.playOnAwake = false;
                audioSource.playOnAwake = false;

                videoPlayer.errorReceived += (VideoPlayer source, string message) => UnityEngine.Debug.LogError(message);

                videoPlayer.source = VideoSource.Url;
                videoPlayer.url = videoURL;

                videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
                videoPlayer.controlledAudioTrackCount = 1;
                videoPlayer.EnableAudioTrack(0, true);
                videoPlayer.SetTargetAudioSource(0, audioSource);

                videoPlayer.Prepare();

                while (!videoPlayer.isPrepared)
                {
                    yield return null;
                }

                GetComponent<RawImage>().texture = videoPlayer.texture;

                videoPlayer.Play();

                yield return new WaitForSeconds(15);
                if (Patcher.game == Patcher.Game.Subnautica)
                {
                    Process.Start("https://store.steampowered.com/app/264710/Subnautica/");
                    Process.Start("https://www.epicgames.com/store/en-US/product/subnautica/home");
                    Process.Start("https://discordapp.com/store/skus/489926636943441932/subnautica");
                }
                else
                {
                    Process.Start("https://store.steampowered.com/app/848450/Subnautica_Below_Zero/");
                    Process.Start("https://www.epicgames.com/store/en-US/product/subnautica-below-zero/home");
                    Process.Start("https://discordapp.com/store/skus/535869836748783616/subnautica-below-zero");
                }

                while (videoPlayer.isPlaying)
                {
                    yield return null;
                }

                Process.Start("https://www.youtube.com/watch?v=dQw4w9WgXcQ");                

                yield return StartCoroutine(PlayVideo());
            }

            private static bool CheckConnection(string hostedURL = "http://www.google.com")
            {
                try
                {
                    string HtmlText = GetHtmlFromUri(hostedURL);
                    if (string.IsNullOrEmpty(HtmlText))
                        return false;
                    else
                        return true;
                }
                catch
                {
                    return false;
                }
            }
            private static string GetHtmlFromUri(string resource)
            {
                string html = string.Empty;
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(resource);
                try
                {
                    using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse())
                    {
                        bool isSuccess = resp.StatusCode >= HttpStatusCode.OK && resp.StatusCode < HttpStatusCode.Ambiguous;
                        if (isSuccess)
                        {
                            using (StreamReader reader = new StreamReader(resp.GetResponseStream()))
                            {
                                char[] cs = new char[80];
                                reader.Read(cs, 0, cs.Length);
                                foreach (char ch in cs)
                                {
                                    html += ch;
                                }
                            }
                        }
                    }
                }
                catch
                {
                    return null;
                }
                return html;
            }
        }

        internal static void PirateDetected()
        {
            Hooks.Update += Log;
            GameObject obj = new GameObject("YOU ARE A PIRATE");
            obj.AddComponent<Pirate>();
        }

        internal static bool IsPirate(string folder)
        {
            string steamDll = Path.Combine(folder, "steam_api64.dll");

            // Check for a modified steam dll
            if (File.Exists(steamDll))
            {
                FileInfo fileInfo = new FileInfo(steamDll);

                if (fileInfo.Length > 220000) return true;
            }

            // Check for cracked files in the folder
            bool steamapiINI = File.Exists(Path.Combine(folder, "steam_api64.ini"));
            bool cdxFiles = new DirectoryInfo(folder).GetFiles("*.cdx").Length > 0;

            if (steamapiINI || cdxFiles) return true;

            return false;
        }

        internal static void Log()
        {
            UnityEngine.Debug.LogError("Do what you want cause a pirate is free, you are a pirate!\nYarr har fiddle dee dee\nBeing a pirate is alright to be\nDo what you want cause a pirate is free\nYou are a pirate!");
        }
    }
}
