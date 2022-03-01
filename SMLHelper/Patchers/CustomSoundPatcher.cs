using FMOD;
using FMODUnity;
using HarmonyLib;
using System;
using System.Collections.Generic;
using FMOD.Studio;
using SMLHelper.V2.Utility;
using UnityEngine;

namespace SMLHelper.V2.Patchers
{
    internal class CustomSoundPatcher
    {
        internal static SelfCheckingDictionary<string, Sound> CustomSounds = new SelfCheckingDictionary<string, Sound>("CustomSounds");
        internal static SelfCheckingDictionary<string, SoundChannel> CustomSoundChannels = new SelfCheckingDictionary<string, SoundChannel>("CustomSoundChannels");
        private static readonly Dictionary<string, Channel> PlayedChannels = new Dictionary<string, Channel>();
        
        internal static void Patch(Harmony harmony)
        {
            harmony.PatchAll(typeof(CustomSoundPatcher));
            Logger.Debug("CustomSoundPatcher is done.");
        }

#if  SUBNAUTICA
        
        [HarmonyPatch(typeof(FMODUWE), nameof(FMODUWE.PlayOneShotImpl))]
        [HarmonyPrefix]
        public static bool FMODUWE_PlayOneShotImpl_Prefix(string eventPath, Vector3 position, float volume)
        {
            if (string.IsNullOrEmpty(eventPath) || !CustomSounds.TryGetValue(eventPath, out Sound soundEvent)) return true;

            if (!CustomSoundChannels.TryGetValue(eventPath, out SoundChannel soundChannel))
                soundChannel = SoundChannel.Master;

            AudioUtils.PlaySound(soundEvent, soundChannel);
            return false;
        }
        
        [HarmonyPatch(typeof(SoundQueue), nameof(SoundQueue.Play))]
        [HarmonyPrefix]
        public static bool SoundQueue_Play_Prefix(SoundQueue __instance, string sound, string subtitles)
        {
            if (string.IsNullOrEmpty(sound) || !CustomSounds.TryGetValue(sound, out Sound soundEvent)) return true;

            __instance.Stop();
            __instance._current = sound;
            soundEvent.getLength(out var length, TIMEUNIT.MS);
            __instance._length = (int)length*1000;
            __instance._lengthSeconds = length;
            if (!CustomSoundChannels.TryGetValue(sound, out SoundChannel soundChannel))
                soundChannel = SoundChannel.Master;

            PlayedChannels[sound] = AudioUtils.PlaySound(soundEvent, soundChannel);
            if (!string.IsNullOrEmpty(subtitles))
            {
                Subtitles main = Subtitles.main;
                if (main)
                {
                    main.Add(subtitles);
                }
            }
            return false;
        }
        
        [HarmonyPatch(typeof(SoundQueue), nameof(SoundQueue.Stop))]
        [HarmonyPrefix]
        public static bool SoundQueue_Stop_Prefix(SoundQueue __instance)
        {
            if (string.IsNullOrEmpty(__instance._current) || !PlayedChannels.TryGetValue(__instance._current, out var channel)) return true;

            channel.stop();
            __instance._current = null;
            
            return false;
        }
        
        [HarmonyPatch(typeof(SoundQueue), nameof(SoundQueue.Update))]
        [HarmonyPrefix]
        public static bool SoundQueue_Update_Prefix(SoundQueue __instance)
        {
            if (__instance is null || string.IsNullOrEmpty(__instance._current)  || !PlayedChannels.TryGetValue(__instance._current, out var channel)) return true;

            ATTRIBUTES_3D attributes = Player.main.transform.To3DAttributes();
#if SUBNAUTICA_STABLE
            channel.set3DAttributes(ref attributes.position, ref attributes.velocity, ref attributes.forward);
#elif SUBNAUTICA_EXP
            channel.set3DAttributes(ref attributes.position, ref attributes.velocity);
#endif
            
            channel.getPosition(out var position, TIMEUNIT.MS);
            __instance.position = (int)position*1000;
            __instance._positionSeconds = position;
            
            return false;
        }

        [HarmonyPatch(typeof(SoundQueue), nameof(SoundQueue.GetIsStartingOrPlaying))]
        [HarmonyPrefix]
        public static bool SoundQueue_GetIsStartingOrPlaying_Prefix(ref bool __result)
        {
            if (PDASounds.queue is null || string.IsNullOrEmpty(PDASounds.queue._current)) return true;
            if (!PlayedChannels.TryGetValue(PDASounds.queue?._current, out var channel)) return true;
            var result = channel.isPlaying(out __result);
            __result = __result && result == RESULT.OK;
            return false;
        }
        
        [HarmonyPatch(typeof(SoundQueue), nameof(SoundQueue.position), MethodType.Setter)]
        [HarmonyPrefix]
        public static bool SoundQueue_Position_Setter_Prefix(SoundQueue __instance, int value)
        {
            if (!PlayedChannels.TryGetValue(__instance._current, out var channel)) return true;

            channel.setPosition((uint)Mathf.Clamp(value*0.001f, 0, __instance._length), TIMEUNIT.MS);
            
            return false;
        }
#elif BELOWZERO
        [HarmonyPatch(typeof(FMODUWE), nameof(FMODUWE.PlayOneShotImpl))]
        [HarmonyPrefix]
        public static bool FMODUWE_PlayOneShotImpl_Prefix(string eventPath, Vector3 position, float volume)
        {
            if (string.IsNullOrEmpty(eventPath) || !CustomSounds.TryGetValue(eventPath, out Sound soundEvent)) return true;

            if (!CustomSoundChannels.TryGetValue(eventPath, out SoundChannel soundChannel))
                soundChannel = SoundChannel.Master;

            AudioUtils.PlaySound(soundEvent, soundChannel);
            return false;
        }

        [HarmonyPatch(typeof(SoundQueue), nameof(SoundQueue.PlayImpl))]
        [HarmonyPrefix]
        public static bool SoundQueue_Play_Prefix(SoundQueue __instance, string sound, SoundHost host, string subtitles, int subtitlesLine, int timelinePosition = 0)
        {
            if (string.IsNullOrEmpty(sound) || !CustomSounds.TryGetValue(sound, out Sound soundEvent)) return true;

            __instance.StopImpl();
            __instance._current =  new SoundQueue.Entry?(new SoundQueue.Entry
            {
                sound = sound,
                subtitles = subtitles,
                subtitleLine = subtitlesLine,
                host = host
            });
            __instance._length = sound.Length;
            __instance._lengthSeconds = __instance._length * 0.001f;
            if (!CustomSoundChannels.TryGetValue(sound, out SoundChannel soundChannel))
                soundChannel = SoundChannel.Master;

            PlayedChannels[sound] = AudioUtils.PlaySound(soundEvent, soundChannel);
            return false;
        }
        
        [HarmonyPatch(typeof(SoundQueue), nameof(SoundQueue.StopImpl))]
        [HarmonyPrefix]
        public static bool SoundQueue_Stop_Prefix(SoundQueue __instance)
        {
            if (__instance._current is null || string.IsNullOrEmpty(__instance._current.Value.sound) || !PlayedChannels.TryGetValue(__instance._current.Value.sound, out var channel)) return true;

            channel.stop();
            PlayedChannels.Remove(__instance._current.Value.sound);
            __instance._current = null;
            
            return false;
        }
        
        [HarmonyPatch(typeof(SoundQueue), nameof(SoundQueue.Update))]
        [HarmonyPrefix]
        public static bool SoundQueue_Update_Prefix(SoundQueue __instance)
        {
            var instanceCurrent = __instance._current ?? default;
            if (string.IsNullOrEmpty(instanceCurrent.sound)  || !PlayedChannels.TryGetValue(instanceCurrent.sound, out var channel)) return true;

            ATTRIBUTES_3D attributes = Player.main.transform.To3DAttributes();
            channel.set3DAttributes(ref attributes.position, ref attributes.velocity);
            channel.getPosition(out var position, TIMEUNIT.MS);
            instanceCurrent.position = (int)position;
            __instance._positionSeconds = (float)instanceCurrent.position * 0.001f;
            __instance._current = instanceCurrent;
            return false;
        }

        [HarmonyPatch(typeof(SoundQueue), nameof(SoundQueue.GetIsStartingOrPlaying))]
        [HarmonyPrefix]
        public static bool SoundQueue_GetIsStartingOrPlaying_Prefix( ref bool __result)
        {
            var instanceCurrent = PDASounds.queue?._current ?? default;
            if (string.IsNullOrEmpty(instanceCurrent.sound)  || !PlayedChannels.TryGetValue(instanceCurrent.sound, out var channel)) return true;

            var result = channel.isPlaying(out __result);
            __result = __result && result == RESULT.OK;
            return false;
        }
        
        [HarmonyPatch(typeof(SoundQueue), nameof(SoundQueue.SetPosition))]
        [HarmonyPrefix]
        public static bool SoundQueue_Position_Setter_Prefix(SoundQueue __instance, int value)
        {
            var instanceCurrent = __instance._current ?? default;
            if (!PlayedChannels.TryGetValue(instanceCurrent.sound, out var channel)) return true;

            channel.setPosition((uint)Mathf.Clamp(value, 0, __instance._length), TIMEUNIT.MS);
            
            return false;
        }
#endif
    }
}