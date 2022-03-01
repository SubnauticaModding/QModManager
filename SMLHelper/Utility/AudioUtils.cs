namespace SMLHelper.V2.Utility
{
    using System.Runtime.InteropServices;
    using FMOD;
    using FMODUnity;
    using UnityEngine;

    /// <summary>
    /// Utilities for audio and sound
    /// </summary>
    public static class AudioUtils
    {
#if SUBNAUTICA_STABLE
        private static System FMOD_System => RuntimeManager.LowlevelSystem;
#else
        private static System FMOD_System => RuntimeManager.CoreSystem;
#endif
        /// <summary>
        /// Creates a <see cref="Sound"/> instance from a path. Can be stored and later used with <see cref="PlaySound(Sound)"/>
        /// </summary>
        /// <param name="path">The path of the sound. Relative to the base game folder.</param>
        /// <param name="mode"></param>
        /// <returns>The <see cref="Sound"/> instance</returns>
        public static Sound CreateSound(string path, MODE mode = MODE.DEFAULT)
        {
            FMOD_System.createSound(path, mode, out Sound sound);
            return sound;
        }

        /// <summary>
        /// Creates a <see cref="Sound"/> instance from an <see cref="AudioClip"/>. Can be stored and later used with <see cref="PlaySound(Sound)"/>
        /// </summary>
        /// <param name="audio">the AudioClip to make a Sound instance of.</param>
        /// <param name="mode"></param>
        /// <returns>The <see cref="Sound"/> instance.</returns>
        public static Sound CreateSound(AudioClip audio, MODE mode = MODE.DEFAULT)
        {
            return CreateSoundFromAudioClip(audio, mode);
        }

        /// <summary>
        /// Plays a <see cref="Sound"/> from an <see cref="AudioClip"/>.
        /// </summary>
        /// <param name="audio">The AudioClip of the sound.</param>
        /// <param name="mode"></param>
        /// <returns>The channel on which the sound was created.</returns>
        public static Channel PlaySound(AudioClip audio, MODE mode = MODE.DEFAULT)
        {
            return PlaySound(CreateSound(audio, mode));
        }

        /// <summary>
        /// Plays a <see cref="Sound"/> from an <see cref="AudioClip"/>. Has overload for controlling volume.
        /// </summary>
        /// <param name="audio">The AudioClip of the sound.</param>
        /// <param name="mode"></param>
        /// <param name="volumeControl">Which volume control to adjust sound levels by. How loud sound is.</param>
        /// <returns>The channel on which the sound was created</returns>
        public static Channel PlaySound(AudioClip audio, SoundChannel volumeControl, MODE mode = MODE.DEFAULT)
        {
            return PlaySound(CreateSound(audio, mode), volumeControl);
        }

        /// <summary>
        /// Plays a <see cref="Sound"/> globally from a path. Must be a .wav file
        /// </summary>
        /// <param name="path">The path of the sound. Relative to the base game folder.</param>
        /// <param name="mode"></param>
        /// <returns>The channel on which the sound was created</returns>
        public static Channel PlaySound(string path, MODE mode = MODE.DEFAULT)
        {
            return PlaySound(CreateSound(path, mode));
        }

        /// <summary>
        /// Plays a <see cref="Sound"/> globally from a path. Must be a .wav file. Has overload for controlling volume
        /// </summary>
        /// <param name="path">The path of the sound. Relative to the base game folder.</param>
        /// <param name="mode"></param>
        /// <param name="volumeControl">Which volume control to adjust sound levels by. How loud sound is.</param>
        /// <returns>The channel on which the sound was created</returns>
        public static Channel PlaySound(string path, SoundChannel volumeControl, MODE mode = MODE.DEFAULT)
        {
            return PlaySound(CreateSound(path, mode), volumeControl);
        }

        /// <summary>
        /// Plays a <see cref="Sound"/> globally
        /// </summary>
        /// <param name="sound">The sound which should be played</param>
        /// <returns>The channel on which the sound was created</returns>
        public static Channel PlaySound(Sound sound)
        {
            FMOD_System.getMasterChannelGroup(out ChannelGroup channels);
            FMOD_System.playSound(sound, channels, false, out Channel channel);

            return channel;
        }

        /// <summary>
        /// Plays a <see cref="Sound"/> globally at specified volume
        /// </summary>
        /// <param name="sound">The sound which should be played</param>
        /// <param name="volumeControl">Which volume control to adjust sound levels by. How loud sound is.</param>
        /// <returns>The channel on which the sound was created</returns>
        public static Channel PlaySound(Sound sound, SoundChannel volumeControl)
        {
            float volumeLevel = volumeControl switch
            {
                SoundChannel.Master  => SoundSystem.masterVolume,
                SoundChannel.Music   => SoundSystem.musicVolume,
                SoundChannel.Voice   => SoundSystem.voiceVolume,
                SoundChannel.Ambient => SoundSystem.ambientVolume,
                _ => 1f,
            };

            FMOD_System.getMasterChannelGroup(out ChannelGroup channels);
            var newChannels = channels;
            newChannels.setVolume(volumeLevel);
            FMOD_System.playSound(sound, newChannels, false, out Channel channel);

            return channel;
        }
        
        private static Sound CreateSoundFromAudioClip(AudioClip audioClip, MODE mode)
        {
            var samplesSize = audioClip.samples * audioClip.channels;
            var samples = new float[samplesSize];
            audioClip.GetData(samples, 0);

            var bytesLength = (uint) (samplesSize * sizeof(float));

            var soundInfo = new CREATESOUNDEXINFO
            {
                cbsize = Marshal.SizeOf(typeof(CREATESOUNDEXINFO)),
                length = bytesLength,
                format = SOUND_FORMAT.PCMFLOAT,
                defaultfrequency = audioClip.frequency,
                numchannels = audioClip.channels
            };
            
            FMOD_System.createSound("", MODE.OPENUSER, ref soundInfo, out var sound);

            sound.@lock(0, bytesLength, out var ptr1, out var ptr2, out var len1, out var len2);

            var samplesLength = (int) (len1 / sizeof(float));
            Marshal.Copy(samples, 0, ptr1, samplesLength);
            if (len2 > 0)
            {
                Marshal.Copy(samples, samplesLength, ptr2, (int) (len2 / sizeof(float)));
            }

            sound.unlock(ptr1, ptr2, len1, len2);
            sound.setMode(mode);

            return sound;
        }
    }
}
