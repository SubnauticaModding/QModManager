namespace SMLHelper.V2.FishFramework
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class CustomFish
    {
        /// <summary>
        /// The id to use to create the creature. This should be unique
        /// </summary>
        public string id;
        /// <summary>
        /// The display name for your fish. This is shown in the inventory, and can be whatever you want
        /// </summary>
        public string displayName;
        /// <summary>
        /// The short description of your fish in the inventory
        /// </summary>
        public string tooltip;

        /// <summary>
        /// The model to use to create the creature. This would ideally only have renderer/collider components attached, but will still work if it has behaviours
        /// </summary>
        public GameObject modelPrefab;

        /// <summary>
        /// Determines whether your creature can be picked up by the player
        /// </summary>
        public bool isPickupable;
        /// <summary>
        /// Determines whether the creature moves on land or in water. Default true, which is in water
        /// </summary>
        public bool isWaterCreature = true;
        /// <summary>
        /// A value to change the size of your creature, in case your model is far too large or small
        /// </summary>
        public float scale = 1f;

        /// <summary>
        /// The speed at which your creature will swim
        /// </summary>
        public float swimSpeed;
        /// <summary>
        /// The area in which your creature will look for a random position when swimming. This should be larger for larger creatures
        /// </summary>
        public Vector3 swimRadius;

        /// <summary>
        /// Optional list of components to add to your creature when its created. These can be existing components in the game or
        /// custom ones in your mod
        /// </summary>
        public List<Type> components = new List<Type>();
    }
}