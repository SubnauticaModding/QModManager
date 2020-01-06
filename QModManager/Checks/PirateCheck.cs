/*using FMODUnity;
using Oculus.Newtonsoft.Json;
using QModManager.Utility;
using System;
using System.Collections;
using System.Diagnostics;
using System.Net;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;*/
using System.Collections.Generic;
using System.IO;
using Logger = QModManager.Utility.Logger;

namespace QModManager.Checks
{
    internal static class PirateCheck
    {
        /*internal class Pirate : MonoBehaviour
        {
            internal static string videoURL;
            internal const string VideoURLObtainer = "https://you-link.herokuapp.com/?url=https://www.youtube.com/watch?v=i8ju_10NkGY";

            internal static readonly HashSet<string> BannedGameObjectNames = new HashSet<string>()
            {
                "Audio",
                "WorldCursor",
                "Default Notification Center",
                "InputHandlerStack",
                "SelectorCanvas",
                "Clip Camera"
            };

            internal void Start()
            {
                Canvas canvas = gameObject.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                gameObject.AddComponent<RawImage>();

                gameObject.AddComponent<AudioListener>().enabled = true;

                GetVideo();
            }

            internal void Update()
            {
                RuntimeManager.MuteAllEvents(true);
                UWE.Utils.alwaysLockCursor = true;
                UWE.Utils.lockCursor = true;
                foreach (GameObject go in SceneManager.GetActiveScene().GetRootGameObjects())
                {
                    if (BannedGameObjectNames.Contains(go.name)) DestroyImmediate(go);
                }
            }

            internal void GetVideo()
            {
                if (!NetworkUtilities.CheckConnection())
                {
                    ShowText();
                    return;
                }
                try
                {
                    ServicePointManager.ServerCertificateValidationCallback = NetworkUtilities.CustomSCVC;

                    using (WebClient client = new WebClient())
                    {
                        client.DownloadStringCompleted += (sender, e) =>
                        {
                            if (e.Error != null)
                            {
                                UnityEngine.Logger.Exception(e.Error);
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
                    UnityEngine.Logger.Exception(e);
                    ShowText();
                }
            }
            internal bool ParseVideo(string result)
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

            internal IEnumerator PlayVideo()
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
            internal void ShowText()
            {
                DestroyImmediate(gameObject.GetComponent<RawImage>());
                Text text = gameObject.AddComponent<Text>();
                text.text = $"An error has occured!\nQModManager couldn't be initialized.\nPlease go and actually purchase {(Patcher.game == Patcher.Game.Subnautica ? "Subnautica" : "Below Zero")}.\nPiracy is bad and hurts the game developer.";
                text.color = new Color(1, 0, 0);
                text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                text.fontStyle = FontStyle.BoldAndItalic;
                text.fontSize = 40;
            }
        }*/

        internal static void PirateDetected()
        {
            Logger.Warn("Ahoy, matey! Ye be a pirate!");
            /*Hooks.Update += Log;
            GameObject obj = new GameObject("YOU ARE A PIRATE");
            obj.AddComponent<Pirate>();*/
        }

        internal static readonly HashSet<string> CrackedFiles = new HashSet<string>()
        {
            "steam_api64.cdx",
            "steam_api64.ini",
            "steam_emu.ini",
            "valve.ini",
            "Subnautica_Data/Plugins/steam_api64.cdx",
            "Subnautica_Data/Plugins/steam_api64.ini",
            "Subnautica_Data/Plugins/steam_emu.ini",
        };

        internal static void IsPirate(string folder)
        {
            string steamDll = Path.Combine(folder, "steam_api64.dll");
            if (File.Exists(steamDll))
            {
                FileInfo fileInfo = new FileInfo(steamDll);

                if (fileInfo.Length > 220000)
                {
                    PirateDetected();
                    return;
                }
            }

            foreach (string file in CrackedFiles)
            {
                if (File.Exists(Path.Combine(folder, file)))
                {
                    PirateDetected();
                    return;
                }
            }
        }

        /*internal static void Log()
        {
            UnityEngine.Debug.LogError("");
        }*/
    }
}
