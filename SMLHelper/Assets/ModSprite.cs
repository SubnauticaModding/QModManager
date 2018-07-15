namespace SMLHelper.V2.Assets
{
    using System.Collections.Generic;

    /// <summary>
    /// A class that handles a custom sprite and what item it is associated to.
    /// </summary>
    internal class ModSprite
    {
        internal static List<ModSprite> Sprites = new List<ModSprite>();

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
            Sprite = sprite;
            Group = SpriteManager.Group.None;
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
