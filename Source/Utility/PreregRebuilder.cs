using PeteTimesSix.ResearchReinvented_SteppingStones.DefOfs;
using PeteTimesSix.ResearchReinvented_SteppingStones.Extensions;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions.Must;
using Verse;
using Verse.Noise;

namespace PeteTimesSix.ResearchReinvented_SteppingStones.Utility
{
    public static class PreregRebuilder
    {
        public static void SetPrerequisitesOnOprhans()
        {
            var noProjectRecipeDefs = new HashSet<RecipeDef>();
            var noProjectSurgeryRecipeDefs = new HashSet<RecipeDef>();
            var noProjectFullBodySurgeryRecipeDefs = new HashSet<RecipeDef>();
            foreach (var recipeDef in DefDatabase<RecipeDef>.AllDefsListForReading.Where(r => r.NoResearchPrerequisites()))
            {
             
                if (recipeDef.IsSurgery)
                {
                    if (recipeDef.targetsBodyPart)
                    {
                        noProjectSurgeryRecipeDefs.Add(recipeDef);
                    }
                    else
                    {
                        noProjectFullBodySurgeryRecipeDefs.Add(recipeDef);
                    }
                }
                else
                {
                    noProjectRecipeDefs.Add(recipeDef);
                }
            }
         

            var noProjectBuildableDefs = new HashSet<ThingDef>();
            var noProjectInstantBuildableDefs = new HashSet<ThingDef>();
            var ProjectBuildableDefsOverride = new HashSet<ThingDef>();
            foreach (var thingDef in DefDatabase<ThingDef>.AllDefsListForReading.Where(t =>t.BuildableByPlayer))
            {
                if (thingDef.IsInstantBuild())
                {
                    noProjectInstantBuildableDefs.Add(thingDef);
                }
                else if(thingDef.researchPrerequisites == null || !thingDef.researchPrerequisites.Any() ||
                    thingDef.researchPrerequisites?.First()?.prerequisites == null || !thingDef.researchPrerequisites.First().prerequisites.Any())
                {
                    noProjectBuildableDefs.Add(thingDef);
                }
                else
                {
                    ProjectBuildableDefsOverride.Add(thingDef);
                }
            }
            var noProjectPlacableDefs = new HashSet<ThingDef>();
            foreach (var thingDef in DefDatabase<ThingDef>.AllDefsListForReading.Where(t => t.placeWorkers != null))
            {
                noProjectPlacableDefs.Add(thingDef);
            }
            var noProjectPlantDefs = new HashSet<ThingDef>();
            foreach (var plantDef in DefDatabase<ThingDef>.AllDefsListForReading.Where(t => t.plant != null && t.plant.Sowable &&
            (t.plant.sowResearchPrerequisites == null || !t.plant.sowResearchPrerequisites.Any() || 
            t.plant.sowResearchPrerequisites.FirstOrDefault().prerequisites == null || !t.plant.sowResearchPrerequisites.FirstOrDefault().prerequisites.Any())))
            {
                noProjectPlantDefs.Add(plantDef);
            }
            var noProjectTerrainDefs = new HashSet<TerrainDef>();
            var ProjectTerrainDefsOverride = new HashSet<TerrainDef>();
            foreach (var terrainDef in DefDatabase<TerrainDef>.AllDefsListForReading.Where(t => (t.BuildableByPlayer)))
            if (terrainDef.researchPrerequisites == null || !terrainDef.researchPrerequisites.Any() || 
                 terrainDef.researchPrerequisites.FirstOrDefault().prerequisites == null || !terrainDef.researchPrerequisites.FirstOrDefault().prerequisites.Any())
            {
                noProjectTerrainDefs.Add(terrainDef);
            }
            else
            {
                    ProjectTerrainDefsOverride.Add(terrainDef);
            }
            var noProjectResearchDefs = new HashSet<ResearchProjectDef>();
            foreach (var researchDef in DefDatabase<ResearchProjectDef>.AllDefsListForReading.Where((ResearchProjectDef r) => r != null && (r.prerequisites == null || !r.prerequisites.Any()) && (r.hiddenPrerequisites == null || !r.hiddenPrerequisites.Any())))
            {
                noProjectResearchDefs.Add(researchDef);
            }
            GivePrerequisitesToBuildables(noProjectBuildableDefs, ProjectBuildableDefsOverride);
            GivePrerequisitesToPlacables(noProjectPlacableDefs);
            GivePrerequisitesToPlants(noProjectPlantDefs);
            GivePrerequisitesToTerrain(noProjectTerrainDefs, ProjectTerrainDefsOverride);
            try
            {
                GivePrerequisitesToRecipes(noProjectRecipeDefs, noProjectSurgeryRecipeDefs, noProjectFullBodySurgeryRecipeDefs);
            }
            catch (InvalidOperationException e)
            {
                Log.Error("error while assinging recipes to projects: " + e.Message);
            }
            try
            {
                GivePrerequisitesToResearch(noProjectResearchDefs);
            }
            catch (InvalidOperationException e)
            {
                Log.Error("error while assinging researches to projects: " + e.Message);
            }


            ThingDefOf_Custom.RR_ThinkingSpot.researchPrerequisites = null;
        }

        private static void GivePrerequisitesToBuildables(HashSet<ThingDef> noProjectBuildableDefs, HashSet<ThingDef> ProjectBuildableDefsOverride)
        {
            foreach (var buildable in noProjectBuildableDefs)
            {
                if (buildable.building.shipPart == true)
                    continue;
                
                if (buildable.researchPrerequisites == null)
                    buildable.researchPrerequisites = new List<ResearchProjectDef>();

                if (((buildable.recipes != null && buildable.recipes.Any()) ||
                    (buildable.inspectorTabs != null && buildable.inspectorTabs.Contains(typeof(ITab_Bills)))) && !buildable.defName.Contains("Campfire"))
                {
                    if (buildable.AllRecipes.Any((RecipeDef r) => r.defName == "ButcherCorpseFlesh"))
                    {
                        buildable.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_Butchering);
                    }
                    else if (buildable.MadeFromStuff || buildable.costList != null)
                    {
                        buildable.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_BasicFurniture);
                    }
                    else
                        buildable.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_Crafting);
                }
                else if (ModsConfig.IdeologyActive == true && buildable.designationCategory.defName == "Ideology")
                {
                    buildable.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_ReligiousThinking);
                }
                else if (buildable.thingClass == typeof(Building_Grave) || (buildable.defName.Contains("Grave") || buildable.defName.Contains("funeral") && buildable.recipes == null))
                {
                    buildable.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_BurialRites);
                }
                else if (buildable.thingClass == typeof(Building_ResearchBench))
                {
                    buildable.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_MethodicalResearch);
                }
                else if (buildable.building.joyKind != null)
                {
                    if(buildable.building.joyKind.defName == "Gaming_Dexterity")
                         buildable.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_BasicGames);
                    else
                        buildable.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_BoardGames);
                }
                else if ((buildable.GetCompProperties<CompProperties_HeatPusher>() != null && buildable.GetCompProperties<CompProperties_Refuelable>() != null) || buildable.GetCompProperties<CompProperties_FireOverlayRitual>() != null)
                {
                    buildable.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_Firemaking);
                }
                else if ((buildable.IsDoor && !buildable.thingClass.ToString().Contains("Windows")) || buildable.thingClass.ToString().Contains("Door") || buildable.thingClass.ToString().Contains("Gate"))
                {
                    buildable.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_Doors);
                }
                else if (buildable.designationCategory.defName == "Security")
                {
                    if (buildable.building.isTrap == true && buildable.stuffCategories != null && buildable.stuffCategories.Contains(StuffCategoryDefOf.Woody))
                    {
                        buildable.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_BasicTraps);
                    }
                    else
                    {
                        if (buildable.stuffCategories != null && buildable.stuffCategories.Contains(StuffCategoryDefOf.Fabric))
                            buildable.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_AdvancedCover);
                        else
                            buildable.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_BasicCover);
                    }
                }
                else if (buildable == ThingDefOf.Wall || buildable.IsFence || buildable == ThingDefOf.Column ||
                   (buildable.thingClass == typeof(Building) && buildable.tickerType == TickerType.Never && buildable.placingDraggableDimensions > 0 && buildable.rotatable == false && !buildable.designationCategory.defName.Contains("Security")) ||
                   buildable.thingClass == typeof(Building) && buildable.passability == Traversability.PassThroughOnly && (buildable.defName.Contains("Column") || buildable.defName.Contains("Frame")))
                {
                    if (buildable.MadeFromStuff && (buildable.stuffCategories.Contains(StuffCategoryDefOf.Fabric) || buildable.stuffCategories.Contains(StuffCategoryDefOf.Leathery)))
                        buildable.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_PrimitiveClothing);
                    else
                       buildable.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_Walls);
                }
                // else if (buildable.IsBed || buildable.IsTable || buildable.building != null && buildable.building.isSittable)
                else if (buildable.MadeFromStuff && buildable.holdsRoof == false)
                { 
                    if ( buildable.stuffCategories.Contains(StuffCategoryDefOf.Stony) && buildable.stuffCategories.Count() == 1)
                    {
                        buildable.researchPrerequisites.Add(ResearchProjectDef.Named("Stonecutting"));
                        buildable.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_BasicFurniture);
                    }
                    else if ((buildable.stuffCategories.Contains(StuffCategoryDefOf.Fabric) || buildable.building != null && buildable.stuffCategories.Contains(StuffCategoryDefOf.Leathery)) && buildable.stuffCategories.Count() <= 2)
                    {
                        if(buildable.building.bed_caravansCanUse == true)
                            buildable.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_Bedrolls);
                        else 
                        { 
                            buildable.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_Tailoring);
                            buildable.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_BasicFurniture);
                        }
                    }
                    else if (buildable.stuffCategories.Contains(StuffCategoryDefOf.Woody))
                    {
                        buildable.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_BasicFurniture);
                    }
                }
                else
                {
                    //buildable.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_FundamentalConstruction);
                }
            }
            foreach (var overide in ProjectBuildableDefsOverride.Except(noProjectBuildableDefs).ToHashSet())
            {
                if (overide.researchPrerequisites == null)
                    overide.researchPrerequisites = new List<ResearchProjectDef>();

                if(overide.building.shipPart == false) 
                    if (((overide.IsDoor && !overide.thingClass.ToString().Contains("Windows")) || overide.thingClass.ToString().Contains("Door") || overide.thingClass.ToString().Contains("Gate")) &&
                        (overide.stuffCategories.Contains(StuffCategoryDefOf.Stony) || overide.stuffCategories.Contains(StuffCategoryDefOf.Woody) || overide.stuffCategories.Contains(StuffCategoryDefOf.Metallic)))
                    {
                        overide.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_Doors);
                    }
                
            }

        }

        private static void GivePrerequisitesToPlacables(HashSet<ThingDef> noProjectPlacableDefs)
        {
            foreach (var placable in noProjectPlacableDefs)
            {
                if (placable.researchPrerequisites == null)
                    placable.researchPrerequisites = new List<ResearchProjectDef>();

                if (placable.thingClass == typeof(Building_Art) || placable.thingCategories.NotNullAndContains(ThingCategoryDefOf.BuildingsArt))
                {
                    placable.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_Art);
                }
            }
        }

        private static void GivePrerequisitesToResearch(HashSet<ResearchProjectDef> noProjectResearchDefs)
        {
            foreach (var research in noProjectResearchDefs)
            {
                if (research.prerequisites == null && research.hiddenPrerequisites == null)
                    research.prerequisites = new List<ResearchProjectDef>();

                var firstProjects = RecipeDefExtensions.FindEarliestPrerequisiteProjects(null, research);
                firstProjects = FilterOutSuperEarlyTechs(firstProjects);
                firstProjects = FilterCopy(firstProjects,research);

                if (firstProjects != null && firstProjects.Any())
                {
                    /*foreach (var r in firstProjects)
                    {
                        Log.Message(r.ToString());
                    }*/
                    research.prerequisites.AddRange(firstProjects);
                    continue;
                }
                else if (!research.prerequisites.Any())
                {
                    if (research.techLevel > TechLevel.Medieval)
                        research.prerequisites.Add(ResearchProjectDef.Named("Electricity"));
                    else if(research.techLevel > TechLevel.Neolithic)
                        research.prerequisites.Add(ResearchProjectDefOf_Custom.RR_MethodicalResearch);

                }             
            }
        }

        private static List<RecipeDef> GetRecipes(ThingDef buildable)
        {
            return buildable.recipes;
        }

        private static void GivePrerequisitesToPlants(HashSet<ThingDef> noProjectPlantDefs)
        {
            foreach (var plant in noProjectPlantDefs)
            {
                if (plant.plant.sowResearchPrerequisites == null)
                    plant.plant.sowResearchPrerequisites = new List<ResearchProjectDef>();

                if(plant.plant.humanFoodPlant == true)
                    plant.plant.sowResearchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_Agriculture);
                else
                    plant.plant.sowResearchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_DomHerb);
            }
        }

        private static void GivePrerequisitesToTerrain(HashSet<TerrainDef> noProjectTerrainDefs, HashSet<TerrainDef> ProjectTerrainDefsOverride)
        {
            foreach (var terrain in noProjectTerrainDefs)
            {
                if (terrain.researchPrerequisites == null)
                    terrain.researchPrerequisites = new List<ResearchProjectDef>();

                if (terrain.fertility > 1.0f)
                {
                    terrain.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_Agriculture);
                }
                else if (terrain.bridge || (terrain.defName.Contains("Bridge") && terrain.terrainAffordanceNeeded == TerrainAffordanceDefOf.Bridgeable))
                {
                    terrain.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_Bridges);
                }
                else if (terrain.defName.Contains("Flagstone") || terrain.costList == null || (terrain.costList != null && !terrain.costList.Any()))
                {
                    terrain.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_Roads);
                }
                else if (terrain.CostList?.FirstOrDefault()?.thingDef?.stuffProps?.categories != null  && terrain?.CostList?.FirstOrDefault(t => t.thingDef.stuffProps.categories.Contains(StuffCategoryDefOf.Stony)) != null)
                {
                    terrain.researchPrerequisites.Add(ResearchProjectDef.Named("Stonecutting"));
                    terrain.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_IndoorFlooring);              
                }
                else
                {
                    terrain.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_IndoorFlooring);
                }
            }
            foreach (var terrain in ProjectTerrainDefsOverride.Except(noProjectTerrainDefs))
            {
                if (terrain?.tags != null && !terrain.tags.Where(t => t == "Ship" || t == "Space").Any())
                {
                    if (terrain.CostList?.Where(t => t.thingDef.stuffProps.categories.Contains(StuffCategoryDefOf.Stony)) != null)
                    {                      
                        if (!terrain.researchPrerequisites.Contains(ResearchProjectDef.Named("Stonecutting")))
                            terrain.researchPrerequisites.Add(ResearchProjectDef.Named("Stonecutting"));
                        terrain.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_IndoorFlooring);
                    }
                    else if ( terrain?.CostList?.Where(t => t.thingDef.stuffProps.categories.Contains(StuffCategoryDefOf.Metallic)) != null && terrain.researchPrerequisites.Contains(ResearchProjectDef.Named("Stonecutting")))
                    {                     
                        ResearchProjectDef thing;
                        thing = terrain.researchPrerequisites.Where(r => r.defName == "Stonecutting").First();
                        if (thing != null)
                        {
                            terrain.researchPrerequisites.Replace(thing, ResearchProjectDef.Named("Electricity"));
                        }                    
                    }
                    else if(terrain?.CostList?.Where(t => t.thingDef.stuffProps.categories.Contains(StuffCategoryDefOf.Metallic)) != null)
                    {                       
                        if (!terrain.researchPrerequisites.Contains(ResearchProjectDef.Named("Smithing")))
                            terrain.researchPrerequisites.Add(ResearchProjectDef.Named("Smithing"));
                        terrain.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_IndoorFlooring);              
                    }
                }
            }
        }

        private static void GivePrerequisitesToRecipes(HashSet<RecipeDef> noProjectRecipeDefs, HashSet<RecipeDef> noProjectSurgeryRecipeDefs, HashSet<RecipeDef> noProjectFullBodySurgeryRecipeDefs)
        {
            foreach (var recipe in noProjectRecipeDefs)
            {
                if (recipe.researchPrerequisites == null)
                    recipe.researchPrerequisites = new List<ResearchProjectDef>();

                var firstProjects = recipe.FindEarliestPrerequisiteProjects(null);
                firstProjects = FilterOutSuperEarlyTechs(firstProjects);

                if (firstProjects != null && firstProjects.Any())
                {
                    recipe.researchPrerequisites.AddRange(firstProjects);
                    continue;
                }

                else if (recipe.ProducedThingDef != null)
                {
                    if (recipe.ProducedThingDef.IsWeapon && recipe.ProducedThingDef.techLevel == TechLevel.Neolithic)
                    {
                        if (recipe.ProducedThingDef.IsRangedWeapon)
                        {
                            recipe.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_PrimitiveRangedWeapons);
                        }
                        else
                        {
                            recipe.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_PrimitiveMeleeWeapons);
                        }
                    }
                    else if (recipe.ProducedThingDef.IsApparel)
                    {
                        if(recipe.ProducedThingDef.techLevel <= TechLevel.Neolithic)
                            recipe.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_PrimitiveClothing);
                        else
                            recipe.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_Tailoring);
                    }
                    else if (recipe.ProducedThingDef.IsIngestible )
                    {
                        if (recipe.ProducedThingDef.IsNutritionGivingIngestible)
                        {
                            if (recipe.ProducedThingDef.GetStatValueAbstract(StatDefOf.Nutrition) < 0.09f)
                                recipe.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_PrimitiveCooking);
                            else
                                recipe.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_Cooking);
                        }
                        else
                        {
                            recipe.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_DomHerb);
                        }
                    }
                    else
                    {

                    }
                }
                
            }

            foreach (var recipe in noProjectSurgeryRecipeDefs)
            {
                if (recipe.researchPrerequisites == null)
                    recipe.researchPrerequisites = new List<ResearchProjectDef>();

                var firstProjects = recipe.FindEarliestPrerequisiteProjects(null);
                firstProjects = FilterOutSuperEarlyTechs(firstProjects);
                if (firstProjects != null && firstProjects.Any())
                {
                    recipe.researchPrerequisites.AddRange(firstProjects);
                    continue;
                }

                //recipe.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_FundamentalSurgery);
            }

            foreach (var recipe in noProjectFullBodySurgeryRecipeDefs)
            {
                if (recipe.researchPrerequisites == null)
                    recipe.researchPrerequisites = new List<ResearchProjectDef>();

                var firstProjects = recipe.FindEarliestPrerequisiteProjects(null);
                firstProjects = FilterOutSuperEarlyTechs(firstProjects);
                if (firstProjects != null && firstProjects.Any())
                {
                    recipe.researchPrerequisites.AddRange(firstProjects);
                    continue;
                }

                //recipe.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_FundamentalMedicine);
            }
        }

        private static HashSet<ResearchProjectDef> _superEarlyTechs;
        public static HashSet<ResearchProjectDef> SuperEarlyTechs
        {
            get
            {
                if (_superEarlyTechs == null)
                    _superEarlyTechs = new HashSet<ResearchProjectDef>() {
                        ResearchProjectDefOf_Custom.RR_Walls,
                        ResearchProjectDefOf_Custom.RR_Doors,
                        ResearchProjectDefOf_Custom.RR_Bridges,
                        ResearchProjectDefOf_Custom.RR_Crafting,
                        ResearchProjectDefOf_Custom.RR_PrimitiveButchering,
                        ResearchProjectDefOf_Custom.RR_Butchering,
                        ResearchProjectDefOf_Custom.RR_PrimitiveCooking,
                        ResearchProjectDefOf_Custom.RR_Cooking,
                        ResearchProjectDefOf_Custom.RR_MethodicalResearch,
                        ResearchProjectDefOf_Custom.RR_Tailoring,
                        ResearchProjectDefOf_Custom.RR_Bedrolls,
                        ResearchProjectDefOf_Custom.RR_BasicFurniture,
                        ResearchProjectDefOf_Custom.RR_Art,
                        ResearchProjectDefOf_Custom.RR_BasicCover,
                        ResearchProjectDefOf_Custom.RR_AdvancedCover,
                        ResearchProjectDefOf_Custom.RR_BasicTraps,
                        ResearchProjectDefOf_Custom.RR_BurialRites,
                        ResearchProjectDefOf_Custom.RR_Roads,
                        ResearchProjectDefOf_Custom.RR_IndoorFlooring,
                        ResearchProjectDefOf_Custom.RR_BasicGames,
                        ResearchProjectDefOf_Custom.RR_BoardGames,
                        ResearchProjectDefOf_Custom.RR_Firemaking,
                        ResearchProjectDefOf_Custom.RR_DomHerb,
                        ResearchProjectDefOf_Custom.RR_Agriculture,
                        ResearchProjectDefOf_Custom.RR_Smokeleaf,
                        ResearchProjectDefOf_Custom.RR_Psychoid,
                        ResearchProjectDefOf_Custom.RR_PrimitiveClothing,
                        ResearchProjectDefOf_Custom.RR_PrimitiveMeleeWeapons,
                        ResearchProjectDefOf_Custom.RR_PrimitiveRangedWeapons,
                        ResearchProjectDefOf_Custom.RR_ReligiousThinking
                    };
                return _superEarlyTechs;
            }
        }

        private static HashSet<ResearchProjectDef> FilterOutSuperEarlyTechs(HashSet<ResearchProjectDef> projects)
        {
            if (projects == null)
                return null;
            return projects.Except(SuperEarlyTechs).ToHashSet();
        }
        private static HashSet<ResearchProjectDef> FilterCopy(HashSet<ResearchProjectDef> projects , ResearchProjectDef itsself)
        {
            if (projects == null)
                return null;
            return projects.Except(itsself).ToHashSet();
        }
    }
}
