namespace SMLHelper.V2.Assets
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using SMLHelper.V2.Utility;
    using SMLHelper.V2.MonoBehaviours;

    /// <summary>
    /// Class used by CustomFish for constructing a prefab based on the values provided by the user.
    /// You can use this yourself if you want, but you will need to manually provide a TechType
    /// </summary>
    public class CustomFishPrefab : ModPrefab
    {
        public GameObject modelPrefab;
        public float scale;
        public bool pickupable;
        public bool isWaterCreature = true;

        public float swimSpeed;
        public Vector3 swimRadius;

        public List<Type> componentsToAdd = new List<Type>();

        GameObject cachedPrefab;

        public CustomFishPrefab(string classId, string prefabFileName, TechType techType = TechType.None) : base(classId, prefabFileName, techType)
        {
        }

        public override GameObject GetGameObject()
        {
            if (cachedPrefab)
            {
                return cachedPrefab;
            }
            Console.WriteLine("[FishFramework] Initializing fish: "+ClassID);
            GameObject mainObj = modelPrefab;

            mainObj.AddComponent<CustomCreature>().scale = scale;

            Console.WriteLine("[FishFramework] Setting correct shaders on renderers");
            Renderer[] renderers = mainObj.GetComponentsInChildren<Renderer>();
            foreach(Renderer rend in renderers)
            {
                rend.material.shader = Shader.Find("MarmosetUBER");
            }

            Console.WriteLine("[FishFramework] Adding essential components to object");

            Rigidbody rb = mainObj.GetOrAddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.angularDrag = 1f;

            WorldForces forces = mainObj.GetOrAddComponent<WorldForces>();
            forces.useRigidbody = rb;
            forces.aboveWaterDrag = 0f;
            forces.aboveWaterGravity = 9.81f;
            forces.handleDrag = true;
            forces.handleGravity = true;
            forces.underwaterDrag = 1f;
            forces.underwaterGravity = 0;
            forces.waterDepth = Ocean.main.GetOceanLevel();
            forces.enabled = false;
            forces.enabled = true;

            mainObj.GetOrAddComponent<EntityTag>().slotType = EntitySlot.Type.Creature;
            mainObj.GetOrAddComponent<PrefabIdentifier>().ClassId = ClassID;
            mainObj.GetOrAddComponent<TechTag>().type = TechType;

            mainObj.GetOrAddComponent<SkyApplier>().renderers = renderers;
            mainObj.GetOrAddComponent<LargeWorldEntity>().cellLevel = LargeWorldEntity.CellLevel.Near;
            mainObj.GetOrAddComponent<LiveMixin>().health = 10f;

            Creature creature = mainObj.GetOrAddComponent<Creature>();
            creature.initialCuriosity = AnimationCurve.Linear(0f, 0.5f, 1f, 0.5f);
            creature.initialFriendliness = AnimationCurve.Linear(0f, 0.5f, 1f, 0.5f);
            creature.initialHunger = AnimationCurve.Linear(0f, 0.5f, 1f, 0.5f);
            SwimBehaviour behaviour = null;
            if (isWaterCreature)
            {
                behaviour = mainObj.GetOrAddComponent<SwimBehaviour>();
                SwimRandom swim = mainObj.GetOrAddComponent<SwimRandom>();
                swim.swimVelocity = swimSpeed;
                swim.swimRadius = swimRadius;
                swim.swimInterval = 1f;
            }
            else
            {
                behaviour = mainObj.GetOrAddComponent<WalkBehaviour>();
                WalkOnGround walk = mainObj.GetOrAddComponent<WalkOnGround>();
                OnSurfaceMovement move = mainObj.GetOrAddComponent<OnSurfaceMovement>();
                move.onSurfaceTracker = mainObj.GetOrAddComponent<OnSurfaceTracker>();
            }
            Locomotion loco = mainObj.GetOrAddComponent<Locomotion>();
            loco.useRigidbody = rb;
            mainObj.GetOrAddComponent<EcoTarget>().type = EcoTargetType.Peeper;
            mainObj.GetOrAddComponent<CreatureUtils>();
            mainObj.GetOrAddComponent<VFXSchoolFishRepulsor>();
            SplineFollowing spline = mainObj.GetOrAddComponent<SplineFollowing>();
            spline.locomotion = loco;
            spline.levelOfDetail = mainObj.GetOrAddComponent<BehaviourLOD>();
            spline.GoTo(mainObj.transform.position + mainObj.transform.forward, mainObj.transform.forward, 5f);
            behaviour.splineFollowing = spline;

            if (pickupable)
            {
                Console.WriteLine("[FishFramework] Adding pickupable component");
                mainObj.GetOrAddComponent<Pickupable>();
            }

            Console.WriteLine("[FishFramework] Adding custom components");

            foreach (Type type in componentsToAdd)
            {
                try
                {
                    mainObj.AddComponent(type);
                }
                catch
                {
                    Console.WriteLine("[FishFramework] Failed to add component "+type.Name+" to GameObject");
                }
            }

            creature.ScanCreatureActions();
            cachedPrefab = mainObj;

            return mainObj;
        }
    }
}
