using Harmony;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using CraftDataPatcher2 = SMLHelper.V2.Patchers.CraftDataPatcher;
using TechDataHelper2 = SMLHelper.V2.Patchers.TechDataHelper;
using IngredientHelper2 = SMLHelper.V2.Patchers.IngredientHelper;

namespace SMLHelper.Patchers
{
    public class CraftDataPatcher
    {
        public static Dictionary<TechType, TechDataHelper> customTechData = new Dictionary<TechType, TechDataHelper>();
        public static Dictionary<TechType, TechType> customHarvestOutputList = new Dictionary<TechType, TechType>();
        public static Dictionary<TechType, HarvestType> customHarvestTypeList = new Dictionary<TechType, HarvestType>();
        public static Dictionary<TechType, Vector2int> customItemSizes = new Dictionary<TechType, Vector2int>();
        public static Dictionary<TechType, EquipmentType> customEquipmentTypes = new Dictionary<TechType, EquipmentType>();
        public static List<TechType> customBuildables = new List<TechType>();

        public static void AddToCustomGroup(TechGroup group, TechCategory category, TechType techType)
        {
            CraftDataPatcher2.AddToCustomGroup(group, category, techType);
        }

        public static void RemoveFromCustomGroup(TechGroup group, TechCategory category, TechType techType)
        {
            CraftDataPatcher2.RemoveFromCustomGroup(group, category, techType);
        }

        public static void Patch()
        {
            customTechData.ForEach(x => CraftDataPatcher2.customTechData.Add(x.Key, x.Value.GetTechDataHelper()));

            customHarvestOutputList.ForEach(x => CraftDataPatcher2.customHarvestOutputList.Add(x.Key, x.Value));
            customHarvestTypeList.ForEach(x => CraftDataPatcher2.customHarvestTypeList.Add(x.Key, x.Value));
            customItemSizes.ForEach(x => CraftDataPatcher2.customItemSizes.Add(x.Key, x.Value));
            customEquipmentTypes.ForEach(x => CraftDataPatcher2.customEquipmentTypes.Add(x.Key, x.Value));
            customBuildables.ForEach(x => CraftDataPatcher2.customBuildables.Add(x));

            V2.Logger.Log("Old CraftDataPatcher is done.");
        }
    }

    public class TechDataHelper
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
            if (_ingredients == null || index > (_ingredients.Count - 1) || index < 0)
            {
                return _ingredients[index];
            }

            return new IngredientHelper(TechType.None, 0);
        }

        public TechType GetLinkedItem(int index)
        {
            if (_linkedItems == null || index > (_linkedItems.Count - 1) || index < 0)
            {
                return _linkedItems[index];
            }

            return TechType.None;
        }

        public TechDataHelper2 GetTechDataHelper()
        {
            var techDataObj = new TechDataHelper2();
            techDataObj._craftAmount = _craftAmount;
            techDataObj._ingredients = GetIngredients();
            techDataObj._linkedItems = _linkedItems;
            techDataObj._techType = _techType;

            return techDataObj;
        }

        public List<IngredientHelper2> GetIngredients()
        {
            var list = new List<IngredientHelper2>();

            foreach(var ingredient in _ingredients)
            {
                list.Add(new IngredientHelper2(ingredient._techType, ingredient._amount));
            }

            return list;
        }

        private object GetIngredientsObj()
        {
            var ingredientsType = typeof(CraftData).GetNestedType("Ingredients", BindingFlags.NonPublic);
            var ingredientsObj = Activator.CreateInstance(ingredientsType);
            var addMethod = ingredientsType.GetMethod("Add", new Type[] { IngredientHelper.IngredientType });

            foreach (var ingredient in _ingredients)
            {
                addMethod.Invoke(ingredientsObj, new object[] { ingredient.GetIngredientObj() });
            }

            return ingredientsObj;
        }

        public object GetTechDataObj()
        {
            var techDataObj = Activator.CreateInstance(TechDataType);
            var ingredientsObj = GetIngredientsObj();

            TechDataType.GetField("_craftAmount").SetValue(techDataObj, _craftAmount);
            TechDataType.GetField("_ingredients").SetValue(techDataObj, ingredientsObj);
            TechDataType.GetField("_linkedItems").SetValue(techDataObj, _linkedItems);
            TechDataType.GetField("_techType").SetValue(techDataObj, _techType);

            return techDataObj;
        }

    }

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
