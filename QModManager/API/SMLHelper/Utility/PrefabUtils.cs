namespace QModManager.API.SMLHelper.Utility
{
    using UnityEngine;

    /// <summary>
    /// Utilities for prefabs
    /// </summary>
    public static class PrefabUtils
    {
        /// <summary>
        /// Adds basic components to a <see cref="GameObject"/>
        /// </summary>
        /// <param name="_object">The <see cref="GameObject"/> to which the components should be added</param>
        /// <param name="classId">The desired class ID</param>
        public static void AddBasicComponents(GameObject _object, string classId)
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
    }
}
