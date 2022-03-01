namespace SMLHelper.V2.Assets
{
    using UnityEngine;

    /// <summary>
    /// Class used by CustomFish for constructing a prefab based on the values provided by the user.
    /// You can use this yourself if you want, but you will need to manually provide a TechType
    /// </summary>
    public class FishPrefab : ModPrefab
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
        /// Creates a new <see cref="FishPrefab"/> with the given values
        /// </summary>
        /// <param name="classId"></param>
        /// <param name="prefabFileName"></param>
        /// <param name="techType"></param>
        public FishPrefab(string classId, string prefabFileName, TechType techType = TechType.None) : base(classId, prefabFileName, techType) { }

        /// <summary>
        /// Gets the prefab game object
        /// </summary>
        public sealed override GameObject GetGameObject()
        {
            V2.Logger.Debug($"[FishFramework] Initializing fish: {this.ClassID}");
            GameObject mainObj = modelPrefab;

            Renderer[] renderers = mainObj.GetComponentsInChildren<Renderer>();
            foreach (Renderer rend in renderers)
            {
                rend.material.shader = Shader.Find("MarmosetUBER");
            }

            Rigidbody rb = mainObj.EnsureComponent<Rigidbody>();
            rb.useGravity = false;
            rb.angularDrag = 1f;

            WorldForces forces = mainObj.EnsureComponent<WorldForces>();
            forces.useRigidbody = rb;
            forces.aboveWaterDrag = 0f;
            forces.aboveWaterGravity = 9.81f;
            forces.handleDrag = true;
            forces.handleGravity = true;
            forces.underwaterDrag = 1f;
            forces.underwaterGravity = 0;
#if BELOWZERO || SUBNAUTICA_EXP
            forces.waterDepth = Ocean.GetOceanLevel();
#else
            forces.waterDepth = Ocean.main.GetOceanLevel();
#endif
            forces.enabled = false;
            forces.enabled = true;

            mainObj.EnsureComponent<EntityTag>().slotType = EntitySlot.Type.Creature;
            mainObj.EnsureComponent<PrefabIdentifier>().ClassId = this.ClassID;
            mainObj.EnsureComponent<TechTag>().type = this.TechType;
            mainObj.EnsureComponent<SkyApplier>().renderers = renderers;
            mainObj.EnsureComponent<LargeWorldEntity>().cellLevel = LargeWorldEntity.CellLevel.Near;
            mainObj.EnsureComponent<LiveMixin>().health = 10f;

            Creature creature = mainObj.EnsureComponent<Creature>();
            creature.initialCuriosity = AnimationCurve.Linear(0f, 0.5f, 1f, 0.5f);
            creature.initialFriendliness = AnimationCurve.Linear(0f, 0.5f, 1f, 0.5f);
            creature.initialHunger = AnimationCurve.Linear(0f, 0.5f, 1f, 0.5f);

            SwimBehaviour behaviour = null;
            if (isWaterCreature)
            {
                behaviour = mainObj.EnsureComponent<SwimBehaviour>();
                SwimRandom swim = mainObj.EnsureComponent<SwimRandom>();
                swim.swimVelocity = swimSpeed;
                swim.swimRadius = swimRadius;
                swim.swimInterval = swimInterval;
            }
            else
            {
                behaviour = mainObj.EnsureComponent<WalkBehaviour>();
                WalkOnGround walk = mainObj.EnsureComponent<WalkOnGround>();
                OnSurfaceMovement move = mainObj.EnsureComponent<OnSurfaceMovement>();
                move.onSurfaceTracker = mainObj.EnsureComponent<OnSurfaceTracker>();
            }

            Locomotion loco = mainObj.EnsureComponent<Locomotion>();
            loco.useRigidbody = rb;

            mainObj.EnsureComponent<EcoTarget>().type = EcoTargetType.Peeper;
            mainObj.EnsureComponent<CreatureUtils>();
            mainObj.EnsureComponent<VFXSchoolFishRepulsor>();

            SplineFollowing spline = mainObj.EnsureComponent<SplineFollowing>();
            spline.locomotion = loco;
            spline.levelOfDetail = mainObj.EnsureComponent<BehaviourLOD>();
            spline.GoTo(mainObj.transform.position + mainObj.transform.forward, mainObj.transform.forward, 5f);

            behaviour.splineFollowing = spline;

            if (pickupable)
            {
                mainObj.EnsureComponent<Pickupable>();
            }

            creature.ScanCreatureActions();

            return mainObj;
        }
    }
}
