using PeteTimesSix.ResearchReinvented_SteppingStones.DefOfs;
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
        private static bool IsCraftingFacility(this ThingDef buildable)
        {
            return ((buildable.recipes != null && buildable.recipes.Any()) ||
                    (buildable.inspectorTabs != null &&
                    buildable.inspectorTabs.Contains(typeof(ITab_Bills)))) &&
                    !buildable.defName.Contains("Campfire");
        }

        private static bool IsButcherer(this ThingDef buildable)
        {
            return buildable.AllRecipes.Any((RecipeDef r) => r.defName == "ButcherCorpseFlesh");
        }

        private static bool IsMadeFromStuff(this ThingDef buildable)
        {
            return buildable.MadeFromStuff || buildable.costList != null;
        }

        private static bool IsGrave(this ThingDef buildable)
        {
            return buildable.thingClass == typeof(Building_Grave) ||
                (buildable.defName.Contains("Grave") ||
                buildable.defName.Contains("funeral") && buildable.recipes == null);
        }

        private static bool IsFire(this ThingDef buildable)
        {
            return (buildable.GetCompProperties<CompProperties_HeatPusher>() != null && buildable.GetCompProperties<CompProperties_Refuelable>() != null) ||
                buildable.GetCompProperties<CompProperties_FireOverlayRitual>() != null;
        }

        private static bool IsDoorOrSimilar(this ThingDef buildable)
        {
            return (buildable.IsDoor && 
                !buildable.thingClass.ToString().Contains("Windows")) ||
                buildable.thingClass.ToString().Contains("Door") ||
                buildable.thingClass.ToString().Contains("Gate");
        }

        private static bool IsTrap(this ThingDef buildable)
        {
            return buildable.building.isTrap == true && buildable.stuffCategories != null && buildable.stuffCategories.Contains(StuffCategoryDefOf.Woody);
        }

        private static bool IsSandbag(this ThingDef buildable)
        {
            return buildable.stuffCategories != null && buildable.stuffCategories.Contains(StuffCategoryDefOf.Fabric);
        }

        private static bool IsFireBased(this ThingDef buildable)
        {
            return  (buildable.HasComp(typeof(CompFireOverlay)) || buildable.HasComp(typeof(CompDarklightOverlay))) ||
                    (buildable.HasComp(typeof(CompMeditationFocus)) && buildable.GetCompProperties<CompProperties_MeditationFocus>().focusTypes.Contains(MeditationFocusDefOf_Custom.Flame));
        }

        private static bool IsFurniture(this ThingDef buildable)
        {
            return buildable.MadeFromStuff && buildable.holdsRoof == false;
        }
        private static bool IsStoneFurniture(this ThingDef buildable)
        {
            return buildable.stuffCategories.Contains(StuffCategoryDefOf.Stony) && buildable.stuffCategories.Count() == 1;
        }
        private static bool IsFurnitureWithFabric(this ThingDef buildable)
        {
            return (buildable.stuffCategories.Contains(StuffCategoryDefOf.Fabric) || buildable.building != null &&
                buildable.stuffCategories.Contains(StuffCategoryDefOf.Leathery)) &&
                buildable.stuffCategories.Count() <= 2;
        }

        private static bool IsWall(this ThingDef buildable)
        {
            return buildable == ThingDefOf.Wall || (buildable.thingClass == typeof(Building) &&
                   buildable.tickerType == TickerType.Never && buildable.placingDraggableDimensions > 0 &&
                   buildable.rotatable == false && (buildable.graphicData.linkType == LinkDrawerType.Basic || buildable.graphicData.linkType == LinkDrawerType.CornerFiller));
        }

        private static bool IsStructure(this ThingDef buildable)
        {
            return buildable == ThingDefOf.Wall || buildable.IsFence || buildable == ThingDefOf.Column ||
                   (IsWall(buildable) && !buildable.designationCategory.defName.Contains("Security")) ||
                   (buildable.thingClass == typeof(Building) && buildable.passability == Traversability.PassThroughOnly &&
                   (buildable.defName.Contains("Column") || buildable.defName.Contains("Frame")));
        }

        private static bool IsTentOrSimilar(this ThingDef buildable)
        {
            return buildable.MadeFromStuff && (buildable.stuffCategories.Contains(StuffCategoryDefOf.Fabric) ||
                buildable.stuffCategories.Contains(StuffCategoryDefOf.Leathery));
        }


        private static bool IsWall(this ThingDef buildable)
        {
            return buildable == ThingDefOf.Wall ||
                (
                    (buildable.graphicData?.linkFlags.HasFlag(LinkFlags.Wall) ?? false) &&
                    buildable.graphicData.linkType == LinkDrawerType.CornerFiller &&
                    (buildable.building?.isInert ?? false) &&
                    buildable.building.isPlaceOverableWall == true &&
                    buildable.fillPercent == 1 &&
                    buildable.passability == Traversability.Impassable &&
                    buildable.size == IntVec2.One
                );
        }

        private static bool IsElectrical(this ThingDef buildable)
        {
            return buildable.techLevel >= TechLevel.Industrial &&
                (buildable.HasComp(typeof(CompPower)) ||
                buildable.HasComp(typeof(CompPowerTrader)) ||
                buildable.HasComp(typeof(CompPowerBattery)) ||
                buildable.HasComp(typeof(CompPowerTransmitter)) ||
                buildable.HasComp(typeof(CompPowerPlant)));
        }

        private static bool IsIdeological(this ThingDef buildable)
        {
            return ModsConfig.IdeologyActive == true && buildable.designationCategory.defName == "Ideology";
        }

    }
}
