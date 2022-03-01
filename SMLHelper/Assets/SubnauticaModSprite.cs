#if SUBNAUTICA
namespace SMLHelper.V2.Assets
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A class that handles a custom sprite and what item it is associated to.
    /// </summary>
    internal class ModSprite
    {
        internal static void Add(SpriteManager.Group group, string name, Atlas.Sprite sprite)
        {
            if (group == SpriteManager.Group.None)
                group = SpriteManager.Group.Item;
            // There are no calls for sprites in the None Group.
            // All sprite calls for almost anything we don't manually group is in the Item group.

            if (!ModSprites.ContainsKey(group))
                ModSprites.Add(group, new Dictionary<string, Atlas.Sprite>(StringComparer.InvariantCultureIgnoreCase));

            if(ModSprites[group].ContainsKey(name))
                Logger.Debug($"ModSprite already registered for {group}/{name}.  Old sprite will be overwritten.");
            ModSprites[group][name] = sprite;
        }

        internal static void Add(ModSprite sprite) => Add(sprite.Group, sprite.Id, sprite.Sprite);

        internal static Dictionary<SpriteManager.Group, Dictionary<string, Atlas.Sprite>> ModSprites 
            = new Dictionary<SpriteManager.Group, Dictionary<string, Atlas.Sprite>>();

        /// <summary>
        /// The tech type of a specific item associated with this sprite.
        /// Can be <see cref="TechType.None"/> if this sprite is for used on a group.
        /// </summary>
        public TechType TechType;

        /// <summary>
        /// The actual sprite used in-game when this sprite is references.
        /// </summary>
        public Atlas.Sprite Sprite;

        /// <summary>
        /// The group that this sprite belongs to. 
        /// Can be <see cref="SpriteManager.Group.None"/> if this sprite is for used on an item.
        /// </summary>
        public SpriteManager.Group Group;

        /// <summary>
        /// The internal identifier of this sprite when it isn't associated to an item.
        /// </summary>
        public string Id;

        /// <summary>
        /// Creates a new ModSprite to be used with a specific TechType.
        /// Created with an Atlas Sprite.
        /// </summary>
        /// <param name="type">The techtype paired to this sprite.</param>
        /// <param name="sprite">The sprite to be added.</param>
        public ModSprite(TechType type, Atlas.Sprite sprite)
        {
            TechType = type;
            Id = type.AsString();
            Sprite = sprite;
            Group = SpriteManager.Group.Item;
        }

        /// <summary>
        /// Creates a new ModSprite to be used with a specific group and internal ID.
        /// Created with an Atlas Sprite.
        /// </summary>
        /// <param name="group">The sprite group.</param>
        /// <param name="id">The sprite internal identifier.</param>
        /// <param name="sprite">The sprite to be added.</param>
        public ModSprite(SpriteManager.Group group, string id, Atlas.Sprite sprite)
        {
            Group = group;
            Id = id;
            Sprite = sprite;
            TechType = TechType.None;
        }

        /// <summary>
        /// Creates a new ModSprite to be used with a specific group and internal ID.
        /// Created with an Atlas Sprite.
        /// </summary>
        /// <param name="group">The sprite group.</param>
        /// <param name="type">The techtype paired to this sprite.</param>
        /// <param name="sprite">The sprite to be added.</param>
        public ModSprite(SpriteManager.Group group, TechType type, Atlas.Sprite sprite)
        {
            Group = group;
            Id = type.AsString();
            Sprite = sprite;
            TechType = type;
        }

        /// <summary>
        /// Creates a new ModSprite to be used with a specific group and internal ID.
        /// Created with a UnityEngine Sprite.
        /// </summary>
        /// <param name="group">The sprite group this sprite will be added to.</param>
        /// <param name="id">The sprite internal identifier.</param>
        /// <param name="sprite">The sprite to be added.</param>
        public ModSprite(SpriteManager.Group group, string id, UnityEngine.Sprite sprite) : this(group, id, new Atlas.Sprite(sprite, false))
        {
        }

        /// <summary>
        /// Creates a new ModSprite to be used with a specific TechType.
        /// Created with a UnityEngine Sprite.
        /// </summary>
        /// <param name="type">The techtype paired to this sprite.</param>
        /// <param name="sprite">The sprite to be added.</param>
        public ModSprite(TechType type, UnityEngine.Sprite sprite) : this(type, new Atlas.Sprite(sprite, false))
        {
        }
    }
}
#endif