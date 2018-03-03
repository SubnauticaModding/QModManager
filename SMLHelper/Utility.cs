using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SMLHelper
{
    public class Utility
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
    }
}
