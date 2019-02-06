using FMODUnity;
using Harmony;
using Oculus.Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
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
                Camera.main.gameObject.AddComponent<AudioListener>().enabled = true;
                GetVideo();
            }

            private void Update()
            {
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
                            Logger.Error("There was an error retrieving the video from YouTube!");
                            Debug.LogException(e.Error);
                            return;
                        }
                        ParseVideo(e.Result);
                    };

                    Logger.Debug("Getting the latest version...");
                    client.DownloadStringAsync(new Uri(VideoURLObtainer));
                }
            }
            private void ParseVideo(string result)
            {
                if (result == null)
                {
                    Logger.Error("There was an error retrieving the video from YouTube!");
                    return;
                }
                Dictionary<string, string>[] parsed;
                try
                {
                    parsed = JsonConvert.DeserializeObject<Dictionary<string, string>[]>(result);
                }
                catch
                {
                    Logger.Error("There was an error retrieving the video from YouTube!");
                    return;
                }
                if (parsed == null || parsed[0] == null)
                {
                    Logger.Error("There was an error retrieving the video from YouTube!");
                    return;
                }
                Dictionary<string, string> firstLink = parsed[0];
                if (!firstLink.TryGetValue("url", out string url))
                {
                    Logger.Error("There was an error retrieving the video from YouTube!");
                    return;
                }
                videoURL = url;
                Logger.Info($"Obtained video URL, playing video...");

                StartCoroutine(PlayVideo());
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
                //audioSource.Play();

                while (videoPlayer.isPlaying)
                {
                    yield return null;
                }

                yield return StartCoroutine(PlayVideo());
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

            if (iniFiles.Length + cdxFiles.Length > 0) return true;

            return false;
        }

        internal static void Log()
        {
            Debug.LogError("Do what you want cause a pirate is free, you are a pirate!\nYarr har fiddle dee dee\nBeing a pirate is alright to be\nDo what you want cause a pirate is free\nYou are a pirate!");
        }
    }
}
