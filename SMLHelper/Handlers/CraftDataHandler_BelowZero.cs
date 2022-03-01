#if BELOWZERO
namespace SMLHelper.V2.Handlers
{
    using Crafting;
    using Interfaces;
    using Patchers;
    using System.Collections.Generic;

    /// <summary>
    /// A handler class for adding and editing crafted items.
    /// </summary>
    public partial class CraftDataHandler : ICraftDataHandler
    {

        #region BelowZero specific Static Methods

        /// <summary>
        /// <para>Allows you to edit recipes, i.e. RecipeData for TechTypes.</para>
        /// <para>Can be used for existing TechTypes too.</para>
        /// </summary>
        /// <param name="techType">The TechType whose TechData you want to edit.</param>
        /// <param name="recipeData">The TechData for that TechType.</param>
        /// <seealso cref="RecipeData"/>
        public static void SetTechData(TechType techType, RecipeData recipeData)
        {
            Main.SetTechData(techType, recipeData);
        }

        /// <summary>
        /// <para>Allows you to add ingredients for a TechType crafting recipe.</para>
        /// <para>Can be used for existing TechTypes too.</para>
        /// </summary>
        /// <param name="techType">The TechType whose ingredient list you want to edit.</param>
        /// <param name="ingredients">The collection of Ingredients for that TechType.</param>
        /// <seealso cref="Ingredient"/>
        public static void SetIngredients(TechType techType, ICollection<Ingredient> ingredients)
        {
            Main.SetIngredients(techType, ingredients);
        }


        /// <summary>
        /// <para>Allows you to edit the Cold Resistance for TechTypes.</para>
        /// <para>Can be used for existing TechTypes too.</para>
        /// </summary>
        /// <param name="techType">The TechType whose Cold Resistance you want to edit.</param>
        /// <param name="resistance">The int value for the Cold Resistance.</param>
        public static void SetColdResistance(TechType techType, int resistance) 
        {
            Main.SetColdResistance(techType, resistance);
        }

        /// <summary>
        /// <para>Allows you to add linked items for a TechType crafting recipe.</para>
        /// <para>Can be used for existing TechTypes too.</para>
        /// </summary>
        /// <param name="techType">The TechType whose ingredient list you want to edit.</param>
        /// <param name="linkedItems">The collection of linked items for that TechType</param>
        public static void SetLinkedItems(TechType techType, ICollection<TechType> linkedItems)
        {
            Main.SetLinkedItems(techType, linkedItems);
        }

        /// <summary>
        /// Safely accesses the crafting data from a modded or vanilla item.<para/>
        /// WARNING: This method is highly dependent on mod load order. 
        /// Make sure your mod is loading after the mod whose TechData you are trying to access.
        /// </summary>
        /// <param name="techType">The TechType whose TechData you want to access.</param>
        /// <returns>The JsonValue from the modded item if it exists; Otherwise, returns <c>null</c>.</returns>
        public static RecipeData GetRecipeData(TechType techType)
        {
            return Main.GetRecipeData(techType);
        }


        /// <summary>
        /// Safely accesses the crafting data from a modded or vanilla item.<para/>
        /// WARNING: This method is highly dependent on mod load order. 
        /// Make sure your mod is loading after the mod whose TechData you are trying to access.
        /// </summary>
        /// <param name="techType">The TechType whose TechData you want to access.</param>
        /// <returns>The JsonValue from the modded item if it exists; Otherwise, returns <c>null</c>.</returns>
        public static RecipeData GetTechData(TechType techType)
        {
            return Main.GetRecipeData(techType);
        }

        /// <summary>
        /// Safely accesses the crafting data from a modded item.<para/>
        /// WARNING: This method is highly dependent on mod load order. 
        /// Make sure your mod is loading after the mod whose TechData you are trying to access.
        /// </summary>
        /// <param name="techType">The TechType whose TechData you want to access.</param>
        /// <returns>The JsonValue from the modded item if it exists; Otherwise, returns <c>null</c>.</returns>
        public static RecipeData GetModdedRecipeData(TechType techType)
        {
            return Main.GetModdedRecipeData(techType);
        }

        /// <summary>
        /// Safely accesses the crafting data from a modded item.<para/>
        /// WARNING: This method is highly dependent on mod load order. 
        /// Make sure your mod is loading after the mod whose TechData you are trying to access.
        /// </summary>
        /// <param name="techType">The TechType whose TechData you want to access.</param>
        /// <returns>The JsonValue from the modded item if it exists; Otherwise, returns <c>null</c>.</returns>
        public static RecipeData GetModdedTechData(TechType techType)
        {
            return Main.GetModdedRecipeData(techType);
        }

        /// <summary>
        /// Sets the maximum charge.
        /// </summary>
        /// <param name="techType">The TechType whose MaxCharge you want to edit.</param>
        /// <param name="maxCharge">The maximum charge.</param>
        public static void SetMaxCharge(TechType techType, float maxCharge)
        {
            Main.SetMaxCharge(techType, maxCharge);
        }

        /// <summary>
        /// Sets the energy cost.
        /// </summary>
        /// <param name="techType">The TechType whose EnergyCost you want to edit.</param>
        /// <param name="energyCost">The energy cost.</param>
        public static void SetEnergyCost(TechType techType, float energyCost)
        {
            Main.SetEnergyCost(techType, energyCost);
        }


        /// <summary>
        /// Sets the type of the sound.
        /// </summary>
        /// <param name="techType">Type of the tech.</param>
        /// <param name="soundType">Type of the sound.</param>
        public static void SetSoundType(TechType techType, TechData.SoundType soundType)
        {
            Main.SetSoundType(techType, soundType);
        }

        #endregion

        #region BelowZero specific implementations

        /// <summary>
        /// <para>Allows you to add or edit RecipeData for TechTypes.</para>
        /// <para>Can be used for existing TechTypes too.</para>
        /// </summary>
        /// <param name="techType">The TechType whose TechData you want to edit.</param>
        /// <param name="recipeData">The TechData for that TechType.</param>
        /// <seealso cref="RecipeData"/>
        void ICraftDataHandler.SetTechData(TechType techType, RecipeData recipeData)
        {
            if (CraftDataPatcher.CustomTechData.TryGetValue(techType, out JsonValue jsonValue))
            {
                jsonValue[TechData.PropertyToID("techType")] = new JsonValue((int)techType);
                jsonValue[TechData.PropertyToID("craftAmount")] = new JsonValue(recipeData.craftAmount);
            }
            else
            {
                jsonValue = new JsonValue
                {
                    { TechData.PropertyToID("techType"), new JsonValue((int)techType) },
                    { TechData.PropertyToID("craftAmount"), new JsonValue(recipeData.craftAmount) }
                };

                CraftDataPatcher.CustomTechData.Add(techType, jsonValue);
            }

            if (recipeData.ingredientCount > 0)
            {
                Main.SetIngredients(techType, recipeData.Ingredients);
            }
            if (recipeData.linkedItemCount > 0)
            {
                Main.SetLinkedItems(techType, recipeData.LinkedItems);
            }
        }

        /// <summary>
        /// <para>Allows you to edit recipes for TechTypes.</para>
        /// <para>Can be used for existing TechTypes too.</para>
        /// </summary>
        /// <param name="techType">The TechType whose TechData you want to edit.</param>
        /// <param name="ingredients">The collection of Ingredients for that TechType.</param>
        /// <seealso cref="Ingredient"/>
        void ICraftDataHandler.SetIngredients(TechType techType, ICollection<Ingredient> ingredients)
        {
            if (!CraftDataPatcher.CustomTechData.TryGetValue(techType, out JsonValue smlJsonValue))
            {
                smlJsonValue = new JsonValue();
                CraftDataPatcher.CustomTechData.Add(techType, smlJsonValue);
            }

            if (!smlJsonValue.Contains(TechData.PropertyToID("ingredients")))
            {
                smlJsonValue.Add(TechData.PropertyToID("ingredients"), new JsonValue(JsonValue.Type.Array));
            }
            else
            {
                smlJsonValue[TechData.PropertyToID("ingredients")] = new JsonValue(JsonValue.Type.Array);
            }

            JsonValue ingredientslist = smlJsonValue[TechData.PropertyToID("ingredients")];

            int amount = TechData.PropertyToID("amount");
            int tech = TechData.PropertyToID("techType");
            int current = 0;

            foreach (Ingredient i in ingredients)
            {
                ingredientslist.Add(new JsonValue(current));
                ingredientslist[current] = new JsonValue(JsonValue.Type.Object)
                {
                    { amount, new JsonValue(i.amount) },
                    { tech, new JsonValue((int)i.techType) }
                };
                current++;
            }
        }

        /// <summary>
        /// <para>Allows you to edit Linked Items for TechTypes.</para>
        /// <para>Can be used for existing TechTypes too.</para>
        /// </summary>
        /// <param name="techType">The TechType whose TechData you want to edit.</param>
        /// <param name="linkedItems">The collection of Ingredients for that TechType.</param>
        /// <seealso cref="Ingredient"/>
        void ICraftDataHandler.SetLinkedItems(TechType techType, ICollection<TechType> linkedItems)
        {
            if (!CraftDataPatcher.CustomTechData.TryGetValue(techType, out JsonValue smlJsonValue))
            {
                CraftDataPatcher.CustomTechData.Add(techType, new JsonValue());
                smlJsonValue = CraftDataPatcher.CustomTechData[techType];
            }

            if (!smlJsonValue.Contains(TechData.PropertyToID("linkedItems")))
            {
                smlJsonValue.Add(TechData.PropertyToID("linkedItems"), new JsonValue(JsonValue.Type.Array));
            }
            else
            {
                smlJsonValue[TechData.PropertyToID("linkedItems")] = new JsonValue(JsonValue.Type.Array);
            }

            JsonValue linkedItemslist = smlJsonValue[TechData.PropertyToID("linkedItems")];

            int current = 0;

            foreach (TechType i in linkedItems)
            {
                linkedItemslist.Add(new JsonValue(current));
                linkedItemslist[current] = new JsonValue((int)i);
                current++;
            }
        }

        /// <summary>
        /// Safely accesses the crafting data from a Modded or Vanilla item.<para/>
        /// WARNING: This method is highly dependent on mod load order. 
        /// Make sure your mod is loading after the mod whose TechData you are trying to access.
        /// </summary>
        /// <param name="techType">The TechType whose TechData you want to access.</param>
        /// <returns>The RecipeData from the item if it exists; Otherwise, returns <c>null</c>.</returns>
        RecipeData ICraftDataHandler.GetRecipeData(TechType techType)
        {
            RecipeData moddedRecipeData = GetModdedRecipeData(techType);

            if (moddedRecipeData != null)
            {
                return moddedRecipeData;
            }

            if (!TechData.Contains(TechType.Knife))
            {
                TechData.Initialize();
            }

            if (TechData.TryGetValue(techType, out JsonValue techData))
            {
                return ConvertToRecipeData(techData);
            }

            return null;
        }

        private static RecipeData ConvertToRecipeData(JsonValue techData)
        {
            RecipeData currentRecipeData = new RecipeData
            {
                craftAmount = techData.GetInt(TechData.propertyCraftAmount, out int craftAmount, 0) ? craftAmount : TechData.defaultCraftAmount
            };

            if (techData.GetArray(TechData.propertyIngredients, out JsonValue jsonValue, null))
            {
                for (int i = 0; i < jsonValue.Count; i++)
                {
                    JsonValue jsonValue2 = jsonValue[i];
                    TechType techType = (TechType)jsonValue2.GetInt(TechData.propertyTechType, 0);
                    int int2 = jsonValue2.GetInt(TechData.propertyAmount, 0);
                    if (techType != TechType.None && int2 > 0)
                    {
                        if (currentRecipeData.Ingredients == null)
                        {
                            currentRecipeData.Ingredients = new List<Ingredient>();
                        }
                        currentRecipeData.Ingredients.Add(new Ingredient(techType, int2));
                    }
                }
            }

            if (techData.GetArray(TechData.propertyLinkedItems, out JsonValue jsonValue3, null))
            {
                for (int j = 0; j < jsonValue3.Count; j++)
                {
                    TechType techType1 = (TechType)jsonValue3[j].GetInt(0);
                    if (currentRecipeData.LinkedItems == null)
                    {
                        currentRecipeData.LinkedItems = new List<TechType>();
                    }
                    currentRecipeData.LinkedItems.Add(techType1);
                }
            }
            return currentRecipeData;
        }

        /// <summary>
        /// Safely accesses the crafting data from a Modded item.<para/>
        /// WARNING: This method is highly dependent on mod load order. 
        /// Make sure your mod is loading after the mod whose TechData you are trying to access.
        /// </summary>
        /// <param name="techType">The TechType whose TechData you want to access.</param>
        /// <returns>The RecipeData from the modded item if it exists; Otherwise, returns <c>null</c>.</returns>
        RecipeData ICraftDataHandler.GetModdedRecipeData(TechType techType)
        {
            return CraftDataPatcher.CustomTechData.TryGetValue(techType, out JsonValue techData) ? ConvertToRecipeData(techData) : null;
        }

        /// <summary>
        /// <para>Allows you to edit EquipmentTypes for TechTypes.</para>
        /// <para>Can be used for existing TechTypes too.</para>
        /// </summary>
        /// <param name="techType">The TechType whose EqiupmentType you want to edit.</param>
        /// <param name="equipmentType">The EquipmentType for that TechType.</param>
        void ICraftDataHandler.SetEquipmentType(TechType techType, EquipmentType equipmentType)
        {
            AddJsonProperty(techType, "equipmentType", new JsonValue((int)equipmentType));
        }

        /// <summary>
        /// <para>Allows you to edit QuickSlotType for TechTypes. Can be used for existing TechTypes too.</para>
        /// <para>Careful: This has to be called after <see cref="SetTechData(TechType, RecipeData)"/>.</para>
        /// </summary>
        /// <param name="techType">The TechType whose QuickSlotType you want to edit.</param>
        /// <param name="slotType">The QuickSlotType for that TechType.</param>
        void ICraftDataHandler.SetQuickSlotType(TechType techType, QuickSlotType slotType)
        {
            AddJsonProperty(techType, "slotType", new JsonValue((int)slotType));
        }

        /// <summary>
        /// <para>Allows you to edit harvest output, i.e. what TechType you get when you "harvest" a TechType.</para>
        /// <para>Can be used for existing TechTypes too.</para>
        /// </summary>
        /// <param name="techType">The TechType whose harvest output you want to edit.</param>
        /// <param name="harvestOutput">The harvest output for that TechType.</param>
        void ICraftDataHandler.SetHarvestOutput(TechType techType, TechType harvestOutput)
        {
            AddJsonProperty(techType, "harvestOutput", new JsonValue((int)harvestOutput));
        }

        /// <summary>
        /// <para>Allows you to edit how TechTypes are harvested.</para>
        /// <para>Can be used for existing TechTypes too.</para>
        /// </summary>
        /// <param name="techType">The TechType whose HarvestType you want to edit.</param>
        /// <param name="harvestType">The HarvestType for that TechType.</param>
        void ICraftDataHandler.SetHarvestType(TechType techType, HarvestType harvestType)
        {
            AddJsonProperty(techType, "harvestType", new JsonValue((int)harvestType));
        }

        /// <summary>
        /// <para>Allows you to edit how much additional slices/seeds are given upon last knife hit.</para>
        /// <para>Can be used for existing TechTypes too.</para>
        /// </summary>
        /// <param name="techType">The TechType whose final cut bonus you want to edit.</param>
        /// <param name="bonus">The number of additional slices/seeds you'll receive on last cut.</param>
        void ICraftDataHandler.SetHarvestFinalCutBonus(TechType techType, int bonus)
        {
            AddJsonProperty(techType, "harvestFinalCutBonus", new JsonValue(bonus));
        }

        /// <summary>
        /// <para>Allows you to edit item sizes for TechTypes.</para>
        /// <para>Can be used for existing TechTypes too.</para>
        /// </summary>
        /// <param name="techType">The TechType whose item size you want to edit.</param>
        /// <param name="size">The item size for that TechType.</param>
        void ICraftDataHandler.SetItemSize(TechType techType, Vector2int size)
        {
            SetItemSize(techType, size.x, size.y);
        }

        /// <summary>
        /// <para>Allows you to edit item sizes for TechTypes.</para>
        /// <para>Can be used for existing TechTypes too.</para>
        /// </summary>
        /// <param name="techType">The TechType whose item size you want to edit.</param>
        /// <param name="x">The width of the item</param>
        /// <param name="y">The height of the item</param>
        void ICraftDataHandler.SetItemSize(TechType techType, int x, int y)
        {
            JsonValue jsonValue = new JsonValue()
            {
                {
                    TechData.propertyX,
                    new JsonValue(x)
                },
                {
                    TechData.propertyY,
                    new JsonValue(y)
                }
            };
            AddJsonProperty(techType, "itemSize", jsonValue);
        }

        /// <summary>
        /// <para>Allows you to edit crafting times for TechTypes.</para>
        /// <para>Can be used for existing TechTypes too.</para>
        /// </summary>
        /// <param name="techType">The TechType whose crafting time you want to edit.</param>
        /// <param name="time">The crafting time, in seconds, for that TechType.</param>
        void ICraftDataHandler.SetCraftingTime(TechType techType, float time)
        {
            AddJsonProperty(techType, "craftTime", new JsonValue(time));
        }

        /// <summary>
        /// <para>Allows you to edit the cooked creature list, i.e. associate the unedible TechType to the cooked TechType.</para>
        /// <para>Can be used for existing TechTypes too.</para>
        /// </summary>
        /// <param name="uncooked">The TechType whose cooked creature counterpart to edit.</param>
        /// <param name="cooked">The cooked creature counterpart for that TechType.</param>
        void ICraftDataHandler.SetCookedVariant(TechType uncooked, TechType cooked)
        {
            AddJsonProperty(uncooked, "processed", new JsonValue((int)cooked));
        }

        /// <summary>
        /// <para>Allows you to edit the Cold Resistance of a TechType.</para>
        /// <para>Can be used for existing TechTypes too.</para>
        /// </summary>
        /// <param name="uncooked">The TechType whose Cold Resistance to edit.</param>
        /// <param name="resistance">The Cold Resistance for that TechType.</param>
        void ICraftDataHandler.SetColdResistance(TechType uncooked, int resistance)
        {
            AddJsonProperty(uncooked, "coldResistance", new JsonValue((int)resistance));
        }

        /// <summary>
        /// <para>Allows you to edit inventory background colors for TechTypes.</para>
        /// </summary>
        /// <param name="techType">The TechType whose BackgroundType you want to edit.</param>
        /// <param name="backgroundColor">The background color for that TechType.</param>
        /// <seealso cref="CraftData.BackgroundType"/>
        void ICraftDataHandler.SetBackgroundType(TechType techType, CraftData.BackgroundType backgroundColor)
        {
            AddJsonProperty(techType, "backgroundType", new JsonValue((int)backgroundColor));
        }

        /// <summary>
        /// Allows you to add items to the buildable list.
        /// </summary>
        /// <param name="techType">The TechType which you want to add to the buildable list.</param>
        void ICraftDataHandler.AddBuildable(TechType techType)
        {
            AddJsonProperty(techType, "buildable", new JsonValue(true));
        }

        /// <summary>
        /// Sets the maximum charge.
        /// </summary>
        /// <param name="techType">The TechType whose MaxCharge you want to edit.</param>
        /// <param name="maxCharge">The maximum charge.</param>
        void ICraftDataHandler.SetMaxCharge(TechType techType, float maxCharge)
        {
            AddJsonProperty(techType, "maxCharge", new JsonValue((double)maxCharge));
        }

        /// <summary>
        /// Sets the energy cost.
        /// </summary>
        /// <param name="techType">The TechType whose EnergyCost you want to edit.</param>
        /// <param name="energyCost">The energy cost.</param>
        void ICraftDataHandler.SetEnergyCost(TechType techType, float energyCost)
        {
            AddJsonProperty(techType, "energyCost", new JsonValue((double)energyCost));
        }

        /// <summary>
        /// Sets the type of the sound.
        /// </summary>
        /// <param name="techType">Type of the tech.</param>
        /// <param name="soundType">Type of the sound.</param>
        void ICraftDataHandler.SetSoundType(TechType techType, TechData.SoundType soundType)
        {
            AddJsonProperty(techType, "soundType", new JsonValue((int)soundType));
        }

        private static void AddJsonProperty(TechType techType, string key, JsonValue newValue)
        {
            if (CraftDataPatcher.CustomTechData.TryGetValue(techType, out JsonValue techData))
            {
                techData[TechData.PropertyToID(key)] = newValue;
            }
            else
            {
                CraftDataPatcher.CustomTechData[techType] = new JsonValue
                {
                    {
                        TechData.PropertyToID("techType"),
                        new JsonValue((int)techType)
                    },
                    {
                        TechData.PropertyToID(key),
                        newValue
                    }
                };
            }
        }

        #endregion
    }
}
#endif
