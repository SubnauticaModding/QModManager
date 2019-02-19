using System;
using System.Collections.Generic;
using System.Reflection;
using CraftDataPatcher2 = SMLHelper.V2.Patchers.CraftDataPatcher;

namespace SMLHelper.Patchers
{
    [Obsolete("Use SMLHelper.V2 instead.")]
    public class CraftDataPatcher
    {
        [Obsolete("Use SMLHelper.V2 instead.")]
        public static Dictionary<TechType, TechDataHelper> customTechData = new Dictionary<TechType, TechDataHelper>();

        [Obsolete("Use SMLHelper.V2 instead.")]
        public static Dictionary<TechType, TechType> customHarvestOutputList = new Dictionary<TechType, TechType>();

        [Obsolete("Use SMLHelper.V2 instead.")]
        public static Dictionary<TechType, HarvestType> customHarvestTypeList = new Dictionary<TechType, HarvestType>();

        [Obsolete("Use SMLHelper.V2 instead.")]
        public static Dictionary<TechType, Vector2int> customItemSizes = new Dictionary<TechType, Vector2int>();

        [Obsolete("Use SMLHelper.V2 instead.")]
        public static Dictionary<TechType, EquipmentType> customEquipmentTypes = new Dictionary<TechType, EquipmentType>();

        [Obsolete("Use SMLHelper.V2 instead.")]
        public static List<TechType> customBuildables = new List<TechType>();

        [Obsolete("Use SMLHelper.V2 instead.")]
        public static void AddToCustomGroup(TechGroup group, TechCategory category, TechType techType)
        {
            CraftDataPatcher2.AddToCustomGroup(group, category, techType, TechType.None);
        }

        [Obsolete("Use SMLHelper.V2 instead.")]
        public static void RemoveFromCustomGroup(TechGroup group, TechCategory category, TechType techType)
        {
            CraftDataPatcher2.RemoveFromCustomGroup(group, category, techType);
        }

        internal static void Patch()
        {
            customTechData.ForEach(x => CraftDataPatcher2.CustomTechData.Add(x.Key, x.Value));
            customHarvestOutputList.ForEach(x => CraftDataPatcher2.CustomHarvestOutputList.Add(x.Key, x.Value));
            customHarvestTypeList.ForEach(x => CraftDataPatcher2.CustomHarvestTypeList.Add(x.Key, x.Value));
            customItemSizes.ForEach(x => CraftDataPatcher2.CustomItemSizes.Add(x.Key, x.Value));
            customEquipmentTypes.ForEach(x => CraftDataPatcher2.CustomEquipmentTypes.Add(x.Key, x.Value));
            customBuildables.ForEach(x => CraftDataPatcher2.CustomBuildables.Add(x));

            V2.Logger.Log("Old CraftDataPatcher is done.");
        }
    }

    [Obsolete("Use SMLHelper.V2 instead.")]
    public class TechDataHelper : ITechData
    {
        public int _craftAmount;
        public TechType _techType;
        public List<IngredientHelper> _ingredients = new List<IngredientHelper>();
        public List<TechType> _linkedItems = new List<TechType>();

        public static Type TechDataType = typeof(CraftData).GetNestedType("TechData", BindingFlags.NonPublic);

        public int craftAmount { get { return _craftAmount; } }

        public int ingredientCount
        {
            get
            {
                if (_ingredients != null) return _ingredients.Count;
                else return 0;
            }
        }

        public int linkedItemCount
        {
            get
            {
                if (_linkedItems != null) return _linkedItems.Count;
                else return 0;
            }
        }

        public IIngredient GetIngredient(int index)
        {
            if (_ingredients != null || index > (_ingredients.Count - 1) || index < 0)
            {
                return _ingredients[index];
            }

            return new IngredientHelper(TechType.None, 0);
        }

        public TechType GetLinkedItem(int index)
        {
            if (_linkedItems != null || index > (_linkedItems.Count - 1) || index < 0)
            {
                return _linkedItems[index];
            }

            return TechType.None;
        }

        private object GetIngredientsObj()
        {
            Type ingredientsType = typeof(CraftData).GetNestedType("Ingredients", BindingFlags.NonPublic);
            object ingredientsObj = Activator.CreateInstance(ingredientsType);
            MethodInfo addMethod = ingredientsType.GetMethod("Add", new Type[] { IngredientHelper.IngredientType });

            foreach (IngredientHelper ingredient in _ingredients)
            {
                addMethod.Invoke(ingredientsObj, new object[] { ingredient.GetIngredientObj() });
            }

            return ingredientsObj;
        }

    }

    [Obsolete("Use SMLHelper.V2 instead.")]
    public class IngredientHelper : IIngredient
    {
        public TechType _techType;
        public int _amount;

        public TechType techType => _techType;
        public int amount => _amount;

        public static Type IngredientType = typeof(CraftData).GetNestedType("Ingredient", BindingFlags.NonPublic);

        public IngredientHelper(TechType techType, int amount)
        {
            _amount = amount;
            _techType = techType;
        }

        public object GetIngredientObj()
        {
            return Activator.CreateInstance(IngredientType, _techType, _amount);
        }
    }
}
