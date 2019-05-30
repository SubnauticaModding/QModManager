namespace QModManager.API.SMLHelper.Assets
{
    using QModManager.Utility;
    using UnityEngine;
    using Logger = QModManager.Utility.Logger;

    /// <summary>
    /// Class used by CustomFish for constructing a prefab based on the values provided by the user.
    /// You can use this yourself if you want, but you will need to manually provide a TechType
    /// </summary>
    public class CustomFishPrefab : ModPrefab
    {
        /// <summary>
        /// The model to use to create the creature. This would ideally only have renderer/collider components attached, but will still work if it has behaviours
        /// </summary>
        public GameObject modelPrefab;
        /// <summary>
        /// Determines whether your creature can be picked up
        /// </summary>
        public bool pickupable;
        /// <summary>
        /// Determines whether your creature walks or swims. Only works for swimming at the moment, land will probably be fixed at a later time
        /// </summary>
        public bool isWaterCreature = true;

        /// <summary>
        /// The speed at which your creature moves
        /// </summary>
        public float swimSpeed;
        /// <summary>
        /// The area in which your creature's AI will look for a new spot to move to
        /// </summary>
        public Vector3 swimRadius;
        /// <summary>
        /// The interval in seconds between when your creature finds a new spot to move to
        /// </summary>
        public float swimInterval;

        /// <summary>
        /// Creates a new <see cref="CustomFishPrefab"/> with the given values
        /// </summary>
        /// <param name="classId"></param>
        /// <param name="prefabFileName"></param>
        /// <param name="techType"></param>
        public CustomFishPrefab(string classId, string prefabFileName, TechType techType = TechType.None) : base(classId, prefabFileName, techType) { }

        /// <summary>
        /// Gets the prefab game object
        /// </summary>
        public sealed override GameObject GetGameObject()
        {
            Logger.Debug($"[FishFramework] Initializing fish: {ClassID}");
            GameObject mainObj = modelPrefab;

            Renderer[] renderers = mainObj.GetComponentsInChildren<Renderer>();
            foreach(Renderer rend in renderers)
            {
                rend.material.shader = Shader.Find("MarmosetUBER");
            }

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
                swim.swimInterval = swimInterval;
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
                mainObj.GetOrAddComponent<Pickupable>();
            }

            creature.ScanCreatureActions();

            return mainObj;
        }
    }
}
