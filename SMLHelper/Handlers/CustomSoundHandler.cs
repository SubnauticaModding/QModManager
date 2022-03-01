namespace SMLHelper.V2.Handlers
{
    using FMOD;
    using Interfaces;
    using Patchers;
    using UnityEngine;
    using Utility;
    
    /// <summary>
    /// A handler class for adding and overriding Sounds.
    /// </summary>
    public class CustomSoundHandler: ICustomSoundHandler
    {

        /// <summary>
        /// Main entry point for all calls to this handler.
        /// </summary>
        public static ICustomSoundHandler Main { get; } = new CustomSoundHandler();

        private CustomSoundHandler()
        {
            // Hides constructor
        }

        #region Interface Methods

        /// <summary>
        /// Register a Custom sound by file path. Some vanilla game sounds can be overridden by matching the id to the <see cref="FMODAsset.path"/>.
        /// </summary>
        /// <param name="id">The Id of your custom sound which is used when checking which sounds to play.</param>
        /// <param name="filePath">The file path on disk of the sound file to load</param>
        /// <param name="soundChannel">The sound channel to get the volume to play the sound at. defaults to <see cref="SoundChannel.Master"/></param>
        /// <returns>the <see cref="Sound"/> loaded</returns>

        Sound ICustomSoundHandler.RegisterCustomSound(string id, string filePath, SoundChannel soundChannel)
        {
            Sound sound = AudioUtils.CreateSound(filePath);
            CustomSoundPatcher.CustomSounds[id] = sound;
            CustomSoundPatcher.CustomSoundChannels[id] = soundChannel;
            return sound;
        }

        /// <summary>
        /// Register a custom sound by an <see cref="AudioClip"/> instance. Some vanilla game sounds can be overridden by matching the id to the <see cref="FMODAsset.path"/>.
        /// </summary>
        /// <param name="id">The Id of your custom sound which is used when checking which sounds to play.</param>
        /// <param name="audio">The AudioClip to register.</param>
        /// <param name="soundChannel">The sound channel to get the volume to play the sound at. defaults to <see cref="SoundChannel.Master"/></param>
        /// <returns>the <see cref="Sound"/> registered.</returns>
        Sound ICustomSoundHandler.RegisterCustomSound(string id, AudioClip audio, SoundChannel soundChannel)
        {
            var sound = AudioUtils.CreateSound(audio);
            CustomSoundPatcher.CustomSounds[id] = sound;
            CustomSoundPatcher.CustomSoundChannels[id] = soundChannel;
            return sound;
        }

        /// <summary>
        /// Register a Custom sound that has been loaded using AudioUtils. Some vanilla game sounds can be overridden by matching the id to the <see cref="FMODAsset.path"/>.
        /// </summary>
        /// <param name="id">The Id of your custom sound which is used when checking which sounds to play.</param>
        /// <param name="sound">The pre loaded sound</param>
        /// <param name="soundChannel">The sound channel to get the volume to play the sound at. <see cref="SoundChannel"/></param>

        void ICustomSoundHandler.RegisterCustomSound(string id, Sound sound, SoundChannel soundChannel)
        {
            CustomSoundPatcher.CustomSounds[id] = sound;
            CustomSoundPatcher.CustomSoundChannels[id] = soundChannel;
        }

        /// <summary>
        /// Try to find and play a custom <see cref="Sound"/> that has been registered.
        /// </summary>
        /// <param name="id">The Id of the custom sound</param>
        void ICustomSoundHandler.TryPlayCustomSound(string id)
        {
            if(!CustomSoundPatcher.CustomSounds.TryGetValue(id, out Sound sound)) return;
            if (!CustomSoundPatcher.CustomSoundChannels.TryGetValue(id, out var soundChannel))
                soundChannel = SoundChannel.Master;
            AudioUtils.PlaySound(sound, soundChannel);
        }
 
        /// <summary>
        /// Try to get a registered custom <see cref="Sound"/>.
        /// </summary>
        /// <param name="id">The Id of the custom sound</param>
        /// <param name="sound">Outputs the <see cref="Sound"/> if found and null if not found.</param>
        /// <returns>true or false depending on if the id was found</returns>
        bool ICustomSoundHandler.TryGetCustomSound(string id, out Sound sound)
        {
            return CustomSoundPatcher.CustomSounds.TryGetValue(id, out sound);
        }
        

        #endregion
        #region Static Methods

        /// <summary>
        /// Register a Custom sound by file path. Some vanilla game sounds can be overridden by matching the id to the <see cref="FMODAsset.path"/>.
        /// </summary>
        /// <param name="id">The Id of your custom sound which is used when checking which sounds to play.</param>
        /// <param name="filePath">The file path on disk of the sound file to load</param>
        /// <param name="soundChannel">The sound channel to get the volume to play the sound at. defaults to <see cref="SoundChannel.Master"/></param>
        /// <returns>the <see cref="Sound"/> loaded</returns>

        public static Sound RegisterCustomSound(string id, string filePath, SoundChannel soundChannel = SoundChannel.Master)
        {
            return Main.RegisterCustomSound(id, filePath, soundChannel);
        }

        /// <summary>
        /// Register a custom sound by an <see cref="AudioClip"/> instance. Some vanilla game sounds can be overridden by matching the id to the <see cref="FMODAsset.path"/>.
        /// </summary>
        /// <param name="id">The Id of your custom sound which is used when checking which sounds to play.</param>
        /// <param name="audio">The AudioClip to register.</param>
        /// <param name="soundChannel">The sound channel to get the volume to play the sound at. defaults to <see cref="SoundChannel.Master"/></param>
        /// <returns>the <see cref="Sound"/> registered.</returns>
        public static Sound RegisterCustomSound(string id, AudioClip audio, SoundChannel soundChannel = SoundChannel.Master)
        {
            return Main.RegisterCustomSound(id, audio, soundChannel);
        }

        /// <summary>
        /// Register a Custom sound that has been loaded using AudioUtils. Some vanilla game sounds can be overridden by matching the id to the <see cref="FMODAsset.path"/>.
        /// </summary>
        /// <param name="id">The Id of your custom sound which is used when checking which sounds to play.</param>
        /// <param name="sound">The pre loaded sound</param>
        /// <param name="soundChannel">The sound channel to get the volume to play the sound at. <see cref="SoundChannel"/></param>

        public static void RegisterCustomSound(string id, Sound sound, SoundChannel soundChannel = SoundChannel.Master)
        {
            Main.RegisterCustomSound(id, sound, soundChannel);
        }

        /// <summary>
        /// Try to find and play a custom <see cref="Sound"/> that has been registered.
        /// </summary>
        /// <param name="id">The Id of the custom sound</param>
        public static void TryPlayCustomSound(string id)
        {
            Main.TryPlayCustomSound(id);
        }
 
        /// <summary>
        /// Try to get a registered custom <see cref="Sound"/>.
        /// </summary>
        /// <param name="id">The Id of the custom sound</param>
        /// <param name="sound">Outputs the <see cref="Sound"/> if found and null if not found.</param>
        /// <returns>true or false depending on if the id was found</returns>
        public static bool TryGetCustomSound(string id, out Sound sound)
        {
            return Main.TryGetCustomSound(id, out sound);
        }

        #endregion
    }
}