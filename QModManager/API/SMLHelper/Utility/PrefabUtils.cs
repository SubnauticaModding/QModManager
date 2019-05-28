namespace QModManager.API.SMLHelper.Utility
{
    using UnityEngine;

    // TODO: Maybe more Prefab-related functions here?
    public static class PrefabUtils
    {
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
    }
}
