namespace SMLHelper.V2.Crafting
{
    using Assets;
    using Patchers;
    using UnityEngine;

    /// <summary>
    /// A tab node of a CraftTree. Tab nodes help organize crafting nodes by grouping them into categories.
    /// </summary>
    /// <seealso cref="ModCraftTreeLinkingNode" />
    public class ModCraftTreeTab : ModCraftTreeLinkingNode
    {
        private readonly string DisplayText;
#if SUBNAUTICA
        private readonly Atlas.Sprite Asprite;
#endif
        private readonly Sprite Usprite;
        private readonly bool IsExistingTab;
        private readonly string ModName;

#if SUBNAUTICA
        internal ModCraftTreeTab(string modName, string nameID, string displayText, Atlas.Sprite sprite)
            : base(nameID, TreeAction.Expand, TechType.None)
        {
            DisplayText = displayText;
            Asprite = sprite;
            Usprite = null;
            ModName = modName;
        }
#endif

        internal ModCraftTreeTab(string modName, string nameID, string displayText, Sprite sprite)
            : base(nameID, TreeAction.Expand, TechType.None)
        {
            DisplayText = displayText;
#if SUBNAUTICA
            Asprite = null;
#endif
            Usprite = sprite;
            ModName = modName;
        }

        internal ModCraftTreeTab(string modName, string nameID)
            : base(nameID, TreeAction.Expand, TechType.None)
        {
            IsExistingTab = true;
            ModName = modName;
        }

        internal override void LinkToParent(ModCraftTreeLinkingNode parent)
        {
            base.LinkToParent(parent);

            if (IsExistingTab) return;

            LanguagePatcher.AddCustomLanguageLine(ModName, $"{base.SchemeAsString}Menu_{Name}", DisplayText);

            string spriteID = $"{SchemeAsString}_{Name}";

#if SUBNAUTICA
            ModSprite modSprite;
            if (Asprite != null)
            {
                modSprite = new ModSprite(SpriteManager.Group.Category, spriteID, Asprite);
            }
            else
            {
                modSprite = new ModSprite(SpriteManager.Group.Category, spriteID, Usprite);

            }

            ModSprite.Add(modSprite);
#elif BELOWZERO

            ModSprite modSprite;
            modSprite = new ModSprite(SpriteManager.Group.Category, spriteID, Usprite);
            ModSprite.Add(modSprite);

#endif
        }
    }
}
