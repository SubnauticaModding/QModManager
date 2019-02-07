using FMOD.Studio;
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

            private void Start()
            {
                Canvas canvas = gameObject.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                gameObject.AddComponent<RawImage>();
                GetVideo();
                DontDestroyOnLoad(this);
            }
            private void Update()
            {
                //RuntimeManager.GetBus("bus:/master").setMute(true);
                //UWE.Utils.alwaysLockCursor = true;
                //UWE.Utils.lockCursor = true;
                SceneManager.GetActiveScene().GetRootGameObjects().Do(go =>
                {
                    string[] bannedGOs =
                    {
                        "Audio",
                        //"WorldCursor",
                        "Default Notification Center",
                        "InputHandlerStack",
                        "SelectorCanvas",
                        "InputDummy",
                        "Clip Camera"
                    };
                    if (Array.IndexOf(bannedGOs, go.name) != -1) DestroyImmediate(go);
                });
            }

            private void GetVideo()
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
                text.text = $"Oopsie poopsie, your game is pirated!\nQModManager couldn't be initialized.\nTo fix this issue, turn on your internet connection,\nthen restart {(Patcher.game == Patcher.Game.Subnautica ? "Subnautica" : "Below Zero")}.";
                text.color = new Color(1, 0, 0);
                text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                text.fontStyle = FontStyle.BoldAndItalic;
                text.fontSize = 40;
            }

            private IEnumerator PlayVideo()
            {
                VideoPlayer videoPlayer = gameObject.GetComponent<VideoPlayer>() ?? gameObject.AddComponent<VideoPlayer>();
                AudioSource audioSource = gameObject.GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();

                videoPlayer.playOnAwake = false;
                audioSource.playOnAwake = false;
                audioSource.Pause();

                videoPlayer.source = VideoSource.Url;
                videoPlayer.url = videoURL;

                videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
                videoPlayer.EnableAudioTrack(0, true);
                videoPlayer.SetTargetAudioSource(0, audioSource);

                videoPlayer.Prepare();

                while (!videoPlayer.isPrepared)
                {
                    yield return null;
                }

                GetComponent<RawImage>().texture = videoPlayer.texture;

                videoPlayer.Play();
                audioSource.Play();

                if (Patcher.game == Patcher.Game.Subnautica) StartCoroutine(OpenSNLinks());
                else StartCoroutine(OpenBZLinks());

                while (videoPlayer.isPlaying)
                {
                    yield return null;
                }

                yield return StartCoroutine(PlayVideo());
            }

            private IEnumerator OpenSNLinks()
            {
                yield return new WaitForSeconds(10);
                Process.Start("https://store.steampowered.com/app/264710/Subnautica/");
                Process.Start("https://www.epicgames.com/store/en-US/product/subnautica/home");
                Process.Start("https://discordapp.com/store/skus/489926636943441932/subnautica");
                yield return new WaitForSeconds(10);
                Process.Start("https://www.youtube.com/watch?v=dQw4w9WgXcQ");
            }
            private IEnumerator OpenBZLinks()
            {
                yield return new WaitForSeconds(10);
                Process.Start("https://store.steampowered.com/app/848450/Subnautica_Below_Zero/");
                Process.Start("https://www.epicgames.com/store/en-US/product/subnautica-below-zero/home");
                Process.Start("https://discordapp.com/store/skus/535869836748783616/subnautica-below-zero");
                yield return new WaitForSeconds(10);
                Process.Start("https://www.youtube.com/watch?v=dQw4w9WgXcQ");
            }
        }

        internal static void PirateDetected()
        {
            GameObject obj = new GameObject("YOU ARE A PIRATE");
            obj.AddComponent<Pirate>();
        }

        internal static bool IsPirate(string folder)
        {
            return true;

            string steamDll = Path.Combine(folder, "steam_api64.dll");

            // Check for a modified steam dll
            if (File.Exists(steamDll))
            {
                FileInfo fileInfo = new FileInfo(steamDll);

                if (fileInfo.Length > 209000) return true;
            }

            // Check for ini files in the root
            FileInfo[] iniFiles = new DirectoryInfo(folder).GetFiles("*.ini");
            FileInfo[] cdxFiles = new DirectoryInfo(folder).GetFiles("*.cdx");
            int e = 0;

            if (File.Exists(Path.Combine(folder, "desktop.ini"))) e--;

            if (iniFiles.Length + cdxFiles.Length + e > 0) return true;

            return false;
        }

        internal static void Log()
        {
            UnityEngine.Debug.LogError("Do what you want cause a pirate is free, you are a pirate!\nYarr har fiddle dee dee\nBeing a pirate is alright to be\nDo what you want cause a pirate is free\nYou are a pirate!");
        }
    }
}
