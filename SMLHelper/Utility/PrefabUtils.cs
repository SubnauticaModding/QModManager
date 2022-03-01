namespace SMLHelper.V2.Utility
{
    using System;
    using UnityEngine;

    /// <summary>
    /// A small collection of prefab related utilities.
    /// </summary>
    public static class PrefabUtils
    {
        /// <summary>
        /// Adds and configures the following components on the gameobject passed by reference:<para/>
        /// - <see cref="Rigidbody"/>
        /// - <see cref="LargeWorldEntity"/>
        /// - <see cref="Renderer"/>
        /// - <see cref="SkyApplier"/>
        /// - <see cref="WorldForces"/>
        /// </summary>
        /// <param name="_object"></param>
        /// <param name="classId"></param>
        public static void AddBasicComponents(ref GameObject _object, string classId)
        {
            Rigidbody rb = _object.AddComponent<Rigidbody>();
            _object.AddComponent<PrefabIdentifier>().ClassId = classId;
            _object.AddComponent<LargeWorldEntity>().cellLevel = LargeWorldEntity.CellLevel.Near;
            Renderer rend = _object.GetComponentInChildren<Renderer>();
            rend.material.shader = Shader.Find("MarmosetUBER");
            SkyApplier applier = _object.AddComponent<SkyApplier>();
            applier.renderers = new Renderer[] { rend };
            applier.anchorSky = Skies.Auto;
            WorldForces forces = _object.AddComponent<WorldForces>();
            forces.useRigidbody = rb;
        }

        /// <summary>
        /// Will attempt to return <see cref="GameObject.GetComponent{T}"/>.<para/>
        /// If the component is not found, it will be added through <see cref="GameObject.AddComponent{T}"/>.
        /// </summary>
        /// <typeparam name="T">A type of Unity <see cref="Component"/>.</typeparam>
        /// <param name="obj">The gameobject that should have the component.</param>
        /// <returns>The existing component attached to the gameobject or a newly created and attached one.</returns>
        [Obsolete("This functionality can be found in Assembly-CSharp-firstpass as Radical.EnsureComponent<T>", true)]
        public static T GetOrAddComponent<T>(this GameObject obj) where T : Component
        {
            return obj.GetComponent<T>() ?? obj.AddComponent<T>();
        }
    }
}
