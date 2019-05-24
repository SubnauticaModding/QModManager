namespace SMLHelper.V2.Utility
{
    using UnityEngine;

    // TODO: Maybe more Prefab-related functions here?
    public static class PrefabUtils
    {
        public static void AddBasicComponents(ref GameObject _object, string classId)
        {
            var rb = _object.AddComponent<Rigidbody>();
            _object.AddComponent<PrefabIdentifier>().ClassId = classId;
            _object.AddComponent<LargeWorldEntity>().cellLevel = LargeWorldEntity.CellLevel.Near;
            var rend = _object.GetComponentInChildren<Renderer>();
            rend.material.shader = Shader.Find("MarmosetUBER");
            var applier = _object.AddComponent<SkyApplier>();
            applier.renderers = new Renderer[] { rend };
            applier.anchorSky = Skies.Auto;
            var forces = _object.AddComponent<WorldForces>();
            forces.useRigidbody = rb;
        }

        public static T GetOrAddComponent<T>(this GameObject obj) where T : Component
        {
            T comp = obj.GetComponent<T>();
            if (!comp)
            {
                comp = obj.AddComponent<T>();
            }

            return comp;
        }
    }
}
