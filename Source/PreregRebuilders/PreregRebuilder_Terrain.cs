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
        public static void DoTerrains()
        {
            var noProjectTerrainDefs = new HashSet<TerrainDef>();
            var ProjectTerrainDefsOverride = new HashSet<TerrainDef>();
            foreach (var terrainDef in DefDatabase<TerrainDef>.AllDefsListForReading.Where(t => (t.BuildableByPlayer)))
            {
                if (terrainDef.researchPrerequisites == null || !terrainDef.researchPrerequisites.Any() ||
                 terrainDef.researchPrerequisites.FirstOrDefault().prerequisites == null || !terrainDef.researchPrerequisites.FirstOrDefault().prerequisites.Any())
                {
                    noProjectTerrainDefs.Add(terrainDef);
                }
                else
                {
                    ProjectTerrainDefsOverride.Add(terrainDef);
                }
            }
            foreach (var terrain in noProjectTerrainDefs)
            {
                try
                {
                    GivePrerequisitesToTerrain(terrain);
                }
                catch (Exception e)
                {
                    Log.Warning($"RR.SS: Error during research project assingment: {e}");
                }
            }
            foreach (var terrainOveride in ProjectTerrainDefsOverride)
            {
                try
                {
                    OverridePrerequisitesToTerrain(terrainOveride);
                }
                catch (Exception e)
                {
                    Log.Warning($"RR.SS: Error during research project assingment: {e}");
                }
            }
        }

        public static void GivePrerequisitesToTerrain(TerrainDef terrain)
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
            else if (terrain.CostList?.FirstOrDefault()?.thingDef?.stuffProps?.categories != null && terrain?.CostList?.FirstOrDefault(t => t.thingDef.stuffProps.categories.Contains(StuffCategoryDefOf.Stony)) != null)
            {
                terrain.researchPrerequisites.Add(ResearchProjectDef.Named("Stonecutting"));
                terrain.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_IndoorFlooring);
            }
            else
            {
                terrain.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_IndoorFlooring);
            }
        }

        public static void OverridePrerequisitesToTerrain(TerrainDef terrain)
        {
            if (terrain?.tags != null && !terrain.tags.Where(t => t == "Ship" || t == "Space").Any())
            {
                if (terrain.CostList?.Where(t => t.thingDef.stuffProps.categories.Contains(StuffCategoryDefOf.Stony)) != null)
                {
                    if (!terrain.researchPrerequisites.Contains(ResearchProjectDef.Named("Stonecutting")))
                        terrain.researchPrerequisites.Add(ResearchProjectDef.Named("Stonecutting"));
                    terrain.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_IndoorFlooring);
                }
                else if (terrain?.CostList?.Where(t => t.thingDef.stuffProps.categories.Contains(StuffCategoryDefOf.Metallic)) != null && terrain.researchPrerequisites.Contains(ResearchProjectDef.Named("Stonecutting")))
                {
                    ResearchProjectDef thing;
                    thing = terrain.researchPrerequisites.Where(r => r.defName == "Stonecutting").First();
                    if (thing != null)
                    {
                        terrain.researchPrerequisites.Replace(thing, ResearchProjectDef.Named("Electricity"));
                    }
                }
                else if (terrain?.CostList?.Where(t => t.thingDef.stuffProps.categories.Contains(StuffCategoryDefOf.Metallic)) != null)
                {
                    if (!terrain.researchPrerequisites.Contains(ResearchProjectDef.Named("Smithing")))
                        terrain.researchPrerequisites.Add(ResearchProjectDef.Named("Smithing"));
                    terrain.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_IndoorFlooring);
                }
            }
        }
    }
}
