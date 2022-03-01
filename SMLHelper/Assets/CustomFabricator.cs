namespace SMLHelper.V2.Assets
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using SMLHelper.V2.Crafting;
    using SMLHelper.V2.Handlers;
    using UnityEngine;
    using Logger = V2.Logger;

#if SUBNAUTICA
    using Sprite = Atlas.Sprite;
#elif BELOWZERO
    using Sprite = UnityEngine.Sprite;
#endif

    /// <summary>
    /// An asset class inheriting from <seealso cref="Buildable"/> that streamlines the process of creating a custom fabricator with a custom crafting tree.
    /// </summary>
    /// <seealso cref="ModPrefab"/>
    public abstract class CustomFabricator : Buildable
    {
        /// <summary>
        /// Defines a list of available models for your <see cref="CustomFabricator"/>.
        /// </summary>
        public enum Models
        {
            /// <summary>
            /// The regular fabricator like the one in the life pod.
            /// </summary>
            Fabricator,

            /// <summary>
            /// The modification station that upgrades your equipment.
            /// </summary>
            Workbench,
#if SUBNAUTICA
            /// <summary>
            /// The style of fabricator found in the Moon Pool and the Cyclops sub.
            /// </summary>
            MoonPool,
#endif
            /// <summary>
            /// Use this option only if you want to provide your own custom model for your fabricator.<para/>
            /// To use this value, you must override the <see cref="GetCustomCrafterPreFab"/> method.
            /// </summary>
            Custom
        }

        private const string RootNode = "root";
        internal readonly Dictionary<string, ModCraftTreeLinkingNode> CraftTreeLinkingNodes = new Dictionary<string, ModCraftTreeLinkingNode>();
        internal readonly List<Action> OrderedCraftTreeActions = new List<Action>();

        /// <summary>
        /// Initialized a new <see cref="CustomFabricator"/> based on the <see cref="Spawnable"/> asset class.
        /// </summary>
        /// <param name="classId">The main internal identifier for this item. Your item's <see cref="TechType"/> will be created using this name.</param>
        /// <param name="friendlyName">The name displayed in-game for this item whether in the open world or in the inventory.</param>
        /// <param name="description">The description for this item; Typically seen in the PDA, inventory, or crafting screens.</param>
        protected CustomFabricator(string classId, string friendlyName, string description)
            : base(classId, friendlyName, description)
        {
            OnStartedPatching += () =>
            {
                CreateCustomCraftTree(out CraftTree.Type craftTreeType);
                this.TreeTypeID = craftTreeType;
            };
        }

        /// <summary>
        /// Override this property to change which model your custom fabricator will use.<para/>
        /// By default, this will be the standard <see cref="Models.Fabricator"/> as seen in the lifepod.
        /// </summary>
        public virtual Models Model => Models.Fabricator;

        /// <summary>
        /// Override this property to change the value of <see cref="Constructable.allowedInBase"/>.<para/>
        /// Defaults to <c>true</c>.
        /// </summary>
        public virtual bool AllowedInBase => true;

        /// <summary>
        /// Override this property to change the value of <see cref="Constructable.allowedInSub"/>.<para/>
        /// Defaults to <c>true</c>.
        /// </summary>
        public virtual bool AllowedInCyclops => true;

        /// <summary>
        /// Override this property to change the value of <see cref="Constructable.allowedOutside"/>.<para/>
        /// Defaults to <c>false</c>.
        /// </summary>
        public virtual bool AllowedOutside => false;

        /// <summary>
        /// Override this property to change the value of <see cref="Constructable.allowedOnCeiling"/>.<para/>
        /// Defaults to <c>false</c>.
        /// </summary>
        public virtual bool AllowedOnCeiling => false;

        /// <summary>
        /// Override this property to change the value of <see cref="Constructable.allowedOnGround"/>.<para/>
        /// Defaults to <c>true</c> for <see cref="Models.Workbench"/> and <c>false</c> for all others.
        /// </summary>
        public virtual bool AllowedOnGround => this.Model == Models.Workbench;

        /// <summary>
        /// Override this property to change the value of <see cref="Constructable.allowedOnWall"/>.<para/>
        /// Defaults to <c>false</c> for <see cref="Models.Workbench"/> and <c>true</c> for all others.
        /// </summary>
        public virtual bool AllowedOnWall => this.Model != Models.Workbench;

        /// <summary>
        /// Override this property to change the value of <see cref="Constructable.rotationEnabled"/>.<para/>
        /// Defaults to <c>true</c> for <see cref="Models.Workbench"/> and <c>false</c> for all others.
        /// </summary>
        public virtual bool RotationEnabled => this.Model == Models.Workbench;

        /// <summary>
        /// Override this value tp <c>true</c> along with <see cref="ColorTint"/> to apply a simple tint to your custom fabricator for easy customization.
        /// </summary>
        public virtual bool UseCustomTint => false;

        /// <summary>
        /// Override this value to your desired <see cref="Color"/> along with <see cref="UseCustomTint"/> to apply a simple tint to your custom fabricator for easy customization.
        /// </summary>
        public virtual Color ColorTint => Color.white;

        /// <summary>
        /// The ID value for your custom craft tree. This is set after this <see cref="Spawnable.Patch"/> method is invoked.
        /// </summary>
        public CraftTree.Type TreeTypeID { get; private set; }

        /// <summary>
        /// Gets the root node of the crafting tree. This is set after this <see cref="Spawnable.Patch"/> method is invoked.
        /// </summary>
        public ModCraftTreeRoot Root { get; private set; }

        /// <summary>
        /// Override with the category within the group in the PDA blueprints where this item appears.
        /// </summary>
        public override TechCategory CategoryForPDA => TechCategory.InteriorModule;

        /// <summary>
        /// Override with the main group in the PDA blueprints where this item appears.
        /// </summary>
        public override TechGroup GroupForPDA => TechGroup.InteriorModules;

        /// <summary>
        /// The in-game <see cref="GameObject"/>.
        /// </summary>
        /// <returns></returns>
        public override GameObject GetGameObject()
        {
#if SUBNAUTICA_EXP || BELOWZERO
            return null;
#else
            GameObject prefab = this.Model switch
            {
                Models.Fabricator => GameObject.Instantiate(CraftData.GetPrefabForTechType(TechType.Fabricator)),
                Models.Workbench => GameObject.Instantiate(CraftData.GetPrefabForTechType(TechType.Workbench)),
#if SUBNAUTICA
                Models.MoonPool => GameObject.Instantiate(Resources.Load<GameObject>("Submarine/Build/CyclopsFabricator")),
#endif
                Models.Custom => GetCustomCrafterPreFab(),
                _ => null
            };

            return PreProcessPrefab(prefab);
#endif
        }

        /// <summary>
        /// The in-game <see cref="GameObject"/>, async way.
        /// </summary>
        /// <returns></returns>
        public override IEnumerator GetGameObjectAsync(IOut<GameObject> gameObject)
        {
            GameObject prefab;
            var taskResult = new TaskResult<GameObject>();

            switch (this.Model)
            {
                case Models.Fabricator:
                default:
                    yield return CraftData.GetPrefabForTechTypeAsync(TechType.Fabricator, false, taskResult);
                    prefab = GameObject.Instantiate(taskResult.Get());
                    break;
                case Models.Workbench:
                    yield return CraftData.GetPrefabForTechTypeAsync(TechType.Workbench, false, taskResult);
                    prefab = GameObject.Instantiate(taskResult.Get());
                    break;
#if SUBNAUTICA
                case Models.MoonPool:
#pragma warning disable CS0618 // obsolete
                    var request = UWE.PrefabDatabase.GetPrefabForFilenameAsync("Submarine/Build/CyclopsFabricator.prefab");
#pragma warning restore CS0618
                    yield return request;
                    request.TryGetPrefab(out prefab);
                    prefab = GameObject.Instantiate(prefab);
                    break;
#endif
                case Models.Custom:
                    yield return GetCustomCrafterPreFabAsync(taskResult);
                    prefab = taskResult.Get();
                    break;
            };

            gameObject.Set(PreProcessPrefab(prefab));
        }

        private GameObject PreProcessPrefab(GameObject prefab)
        {
            Constructable constructible = null;
            GhostCrafter crafter;
            switch (this.Model)
            {
                case Models.Fabricator:
                default:
                    crafter = prefab.GetComponent<Fabricator>();
                    break;
                case Models.Workbench:
                    crafter = prefab.GetComponent<Workbench>();
                    break;
#if SUBNAUTICA
                case Models.MoonPool:
                    crafter = prefab.GetComponent<Fabricator>();

                    // Add prefab ID because CyclopsFabricator normaly doesn't have one
                    PrefabIdentifier prefabId = prefab.AddComponent<PrefabIdentifier>();
                    prefabId.ClassId = this.ClassID;
                    prefabId.name = this.FriendlyName;

                    // Add tech tag because CyclopsFabricator normaly doesn't have one
                    TechTag techTag = prefab.AddComponent<TechTag>();
                    techTag.type = this.TechType;

                    // Retrieve sub game objects
                    GameObject cyclopsFabLight = prefab.FindChild("fabricatorLight");
                    GameObject cyclopsFabModel = prefab.FindChild("submarine_fabricator_03");
                    // Translate CyclopsFabricator model and light
                    prefab.transform.localPosition = new Vector3(cyclopsFabModel.transform.localPosition.x, // Same X position
                                                                 cyclopsFabModel.transform.localPosition.y - 0.8f, // Push towards the wall slightly
                                                                 cyclopsFabModel.transform.localPosition.z); // Same Z position
                    prefab.transform.localPosition = new Vector3(cyclopsFabLight.transform.localPosition.x, // Same X position
                                                                 cyclopsFabLight.transform.localPosition.y - 0.8f, // Push towards the wall slightly
                                                                 cyclopsFabLight.transform.localPosition.z); // Same Z position
                    // Add constructable - This prefab normally isn't constructed.
                    constructible = prefab.AddComponent<Constructable>();
                    constructible.model = cyclopsFabModel;
                    break;
#endif
                case Models.Custom:
                    crafter = prefab.EnsureComponent<Fabricator>();
                    break;
            }

            crafter.craftTree = this.TreeTypeID;
            crafter.handOverText = $"Use {this.FriendlyName}";

            if (constructible is null)
                constructible = prefab.GetComponent<Constructable>();

            constructible.allowedInBase = this.AllowedInBase;
            constructible.allowedInSub = this.AllowedInCyclops;
            constructible.allowedOutside = this.AllowedOutside;
            constructible.allowedOnCeiling = this.AllowedOnCeiling;
            constructible.allowedOnGround = this.AllowedOnGround;
            constructible.allowedOnWall = this.AllowedOnWall;
            constructible.allowedOnConstructables = false;
            constructible.controlModelState = true;
            constructible.rotationEnabled = this.RotationEnabled;
            constructible.techType = this.TechType; // This was necessary to correctly associate the recipe at building time            

            SkyApplier skyApplier = prefab.EnsureComponent<SkyApplier>();
            skyApplier.renderers = prefab.GetComponentsInChildren<Renderer>();
            skyApplier.anchorSky = Skies.Auto;

            if (this.UseCustomTint)
            {
                SkinnedMeshRenderer skinnedMeshRenderer = prefab.GetComponentInChildren<SkinnedMeshRenderer>();
                skinnedMeshRenderer.material.color = this.ColorTint; // Tint option available
            }

            crafter.powerRelay = PowerSource.FindRelay(prefab.transform);

            return prefab;
        }

        /// <summary>
        /// Override this method if you want to provide your own prefab and model for your custom fabricator.<para/>
        /// </summary>
        /// <returns></returns>
        protected virtual GameObject GetCustomCrafterPreFab()
        {
            throw new NotImplementedException($"To use a custom fabricator model, the prefab must be created in {nameof(GetCustomCrafterPreFab)}.");
        }

        /// <summary>
        /// Override this method if you want to provide your own prefab and model for your custom fabricator.<para/>
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator GetCustomCrafterPreFabAsync(IOut<GameObject> gameObject)
        {
            throw new NotImplementedException($"To use a custom fabricator model, the prefab must be created in {nameof(GetCustomCrafterPreFabAsync)}.");
        }

        /// <summary>
        /// Override this method if you want full control over how your custom craft tree is built up.<para/>
        /// To use this method's default behavior, you must use the following methods to build up your crafting tree.<para/>
        /// - <see cref="AddCraftNode(TechType, string)"/><para/>
        /// - <see cref="AddCraftNode(string, string)"/><para/>
        /// - <see cref="AddCraftNode(Craftable, string)"/><para/>
        /// </summary>
        /// <param name="craftTreeType"></param>
        internal virtual void CreateCustomCraftTree(out CraftTree.Type craftTreeType)
        {
            ModCraftTreeRoot root = CraftTreeHandler.CreateCustomCraftTreeAndType(this.ClassID, out craftTreeType);
            this.Root = root;
            CraftTreeLinkingNodes.Add(RootNode, root);

            // Since we shouldn't rely on attached events to be executed in any particular order,
            // this list of actions will ensure that the craft tree is built up in the order in which nodes were received.
            foreach (Action action in OrderedCraftTreeActions)
                action.Invoke();
        }

        /// <summary>
        /// Adds a new tab node to the custom crafting tree of this fabricator.
        /// </summary>
        /// <param name="tabId">The internal ID for the tab node.</param>
        /// <param name="displayText">The in-game text shown for the tab node.</param>
        /// <param name="tabSprite">The sprite used for the tab node.</param>
        /// <param name="parentTabId">Optional. The parent tab of this tab.<para/>
        /// When this value is null, the tab will be added to the root of the craft tree.</param>
        public void AddTabNode(string tabId, string displayText, Sprite tabSprite, string parentTabId = null)
        {
            OrderedCraftTreeActions.Add(() =>
            {
                ModCraftTreeLinkingNode parentNode = CraftTreeLinkingNodes[parentTabId ?? RootNode];
                ModCraftTreeTab tab = parentNode.AddTabNode(tabId, displayText, tabSprite);
                CraftTreeLinkingNodes[tabId] = tab;
            });
        }

        /// <summary>
        /// Adds a new crafting node to the custom crafting tree of this fabricator.
        /// </summary>
        /// <param name="techType">The item to craft.</param>
        /// <param name="parentTabId">Optional. The parent tab of this craft node.<para/>
        /// When this value is null, the craft node will be added to the root of the craft tree.</param>
        public void AddCraftNode(TechType techType, string parentTabId = null)
        {
            Logger.Debug($"'{techType.AsString()}' will be added to the custom craft tree '{this.ClassID}'");
            OrderedCraftTreeActions.Add(() =>
            {
                ModCraftTreeLinkingNode parentTab = CraftTreeLinkingNodes[parentTabId ?? RootNode];
                parentTab.AddCraftingNode(techType);
            });
        }

        /// <summary>
        /// Safely attempts to add a new crafting node to the custom crafting tree of this fabricator.<para/>
        /// If the modded TechType is not found, the craft node will not be added.
        /// </summary>
        /// <param name="moddedTechType">The modded item to craft.</param>
        /// <param name="parentTabId">Optional. The parent tab of this craft node.<para/>
        /// When this value is null, the craft node will be added to the root of the craft tree.</param>
        public void AddCraftNode(string moddedTechType, string parentTabId = null)
        {
            Logger.Debug($"'{moddedTechType}' will be added to the custom craft tree '{this.ClassID}'");
            OrderedCraftTreeActions.Add(() =>
            {
                if (this.TechTypeHandler.TryGetModdedTechType(moddedTechType, out TechType techType))
                {
                    ModCraftTreeLinkingNode parentTab = CraftTreeLinkingNodes[parentTabId ?? RootNode];
                    parentTab.AddCraftingNode(techType);
                }
                else
                {
                    Logger.Info($"Did not find a TechType value for '{moddedTechType}' to add to the custom craft tree '{this.ClassID}'");
                }
            });
        }

        /// <summary>
        /// Safely adds a new crafting node to the custom crafting tree of this fabricator.<para/>
        /// If the item has not been patched yet, its <see cref="Spawnable.Patch"/> method will first be invoked.
        /// </summary>
        /// <param name="item">The <see cref="Craftable"/> item to craft from this fabricator.</param>
        /// <param name="parentTabId">Optional. The parent tab of this craft node.<para/>
        /// When this value is null, the item's <see cref="Craftable.StepsToFabricatorTab"/> property will be checked instead.<para/>
        /// The craft node will be added to the root of the craft tree if both are null.</param>
        public void AddCraftNode(Craftable item, string parentTabId = null)
        {
            Logger.Debug($"'{item.ClassID}' will be added to the custom craft tree '{this.ClassID}'");
            OrderedCraftTreeActions.Add(() =>
            {
                if (item.TechType == TechType.None)
                {
                    Logger.Info($"'{item.ClassID} had to be patched early to obtain its TechType value for the custom craft tree '{this.ClassID}'");
                    item.Patch();
                }

                string[] stepsToParent = item.StepsToFabricatorTab;

                if (parentTabId == null)
                {
                    if (stepsToParent != null && stepsToParent.Length > 0)
                    {
                        int last = stepsToParent.Length - 1;
                        parentTabId = stepsToParent[last];
                    }
                    else
                    {
                        parentTabId = RootNode;
                    }
                }

                ModCraftTreeLinkingNode parentTab = CraftTreeLinkingNodes[parentTabId];
                parentTab.AddCraftingNode(item.TechType);
            });
        }
    }
}
