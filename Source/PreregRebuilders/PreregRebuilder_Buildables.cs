using PeteTimesSix.ResearchReinvented_SteppingStones.DefOfs;
using PeteTimesSix.ResearchReinvented_SteppingStones.Extensions;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PeteTimesSix.ResearchReinvented_SteppingStones.PreregRebuilders
{
    public static partial class PreregRebuilder
    {
        public static void DoBuildables()
        {
            var noProjectBuildableDefs = new HashSet<ThingDef>();
            var ProjectBuildableDefsOverride = new HashSet<ThingDef>();
            foreach (var thingDef in DefDatabase<ThingDef>.AllDefsListForReading.Where(t => t.BuildableByPlayer))
            {
                if (thingDef.IsInstantBuild())
                {
                    continue;
                }
                else if (thingDef.researchPrerequisites == null || !thingDef.researchPrerequisites.Any() ||
                   thingDef.researchPrerequisites?.First()?.prerequisites == null ||
                   !thingDef.researchPrerequisites.First().prerequisites.Any())
                {
                    noProjectBuildableDefs.Add(thingDef);
                }
                else 
                {
                    ProjectBuildableDefsOverride.Add(thingDef);
                }
            }

            foreach (var buildable in noProjectBuildableDefs)
            {
                try
                {
                    if(SkipBuildables(buildable))
                    {
                        continue; 
                    }
                    GivePrerequisitesToBuildables(buildable);
                }
                catch (Exception e)
                {
                    Log.Warning($"RR.SS: Error during research project assingment: {e}");
                }
            }

            foreach (var buildable in ProjectBuildableDefsOverride)
            {
                try
                {
                    GiveProjectBuildableDefsOverride(buildable);
                }
                catch (Exception e)
                {
                    Log.Warning($"RR.SS: Error during research project assingment: {e}");
                }
            }
        }


        private static void GivePrerequisitesToBuildables(ThingDef buildable)
        {
          
            if (buildable.researchPrerequisites == null)
                buildable.researchPrerequisites = new List<ResearchProjectDef>();

            if (buildable.IsElectrical())
                buildable.researchPrerequisites.Add(ResearchProjectDef.Named("Electricity"));
            else if (buildable.IsCraftingFacility())
            {
                if (buildable.IsButcherer())
                    buildable.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_Butchering);
                else
                { 
                    buildable.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_Crafting);
                    if (buildable.IsMadeFromStuff())
                        buildable.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_BasicFurniture);
                }
            }
            else if (buildable.IsIdeological())
            {
                buildable.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_ReligiousThinking);
            }
            else if (buildable.IsGrave())
            {
                buildable.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_BurialRites);
            }
            else if (buildable.thingClass == typeof(Building_ResearchBench))
            {
                buildable.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_MethodicalResearch);
            }
            else if (buildable.building.joyKind != null)
            {
                if (buildable.building.joyKind.defName == "Gaming_Dexterity")
                    buildable.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_BasicGames);
                else
                    buildable.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_BoardGames);
            }
            else if (buildable.IsFire())
            {
                buildable.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_Firemaking);
            }
            else if (buildable.IsDoorOrSimilar())
            {
                buildable.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_Doors);
            }
            else if (buildable.designationCategory.defName == "Security" || (buildable.IsWall() && buildable.fillPercent < 1f))
            {
                if(buildable.IsTrap())
                    buildable.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_BasicTraps);
                else
                    if (buildable.IsSandbag())
                         buildable.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_AdvancedCover);
                    else
                         buildable.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_BasicCover);
            }
            else if (buildable.IsStructure())
            {
                if(buildable.IsTentOrSimilar())
                    buildable.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_PrimitiveClothing);
                else
                    buildable.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_Walls);
            }
            else if (buildable.IsFurniture())
            {
                if (buildable.IsStoneFurniture())
                {
                    buildable.researchPrerequisites.Add(ResearchProjectDef.Named("Stonecutting"));
                    buildable.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_BasicFurniture);
                }
                else if (buildable.IsFurnitureWithFabric())
                {
                    if (buildable.building.bed_caravansCanUse == true)
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
            else if (buildable.techLevel == TechLevel.Medieval)
            {
               buildable.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_MethodicalResearch);
            }
            else
            {
                buildable.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_Crafting);
            }
        }

        private static bool SkipBuildables(ThingDef buildable) 
        {            
            if (buildable.researchPrerequisites == null)
                buildable.researchPrerequisites = new List<ResearchProjectDef>();

            var b = false;
            if (buildable.building.shipPart)
            {
                b = true;
            }
            return b;
        }

        private static void GiveProjectBuildableDefsOverride(ThingDef buildableOverride)
        {
            if (buildableOverride.researchPrerequisites == null)
                buildableOverride.researchPrerequisites = new List<ResearchProjectDef>();

            if (!buildableOverride.building.shipPart)
            {
                if (buildableOverride.IsDoor && !buildableOverride.thingClass.ToString().Contains("Windows") &&
                    (!buildableOverride.stuffCategories.Contains(StuffCategoryDefOf.Fabric) || !buildableOverride.stuffCategories.Contains(StuffCategoryDefOf.Leathery)))
                {
                    buildableOverride?.researchPrerequisites?.Add(ResearchProjectDefOf_Custom.RR_Doors);
                }
            }
        }

    }
}
