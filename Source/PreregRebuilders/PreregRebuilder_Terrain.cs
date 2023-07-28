using PeteTimesSix.ResearchReinvented_SteppingStones.DefOfs;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.Noise;

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
                    terrainDef.researchPrerequisites?.First()?.prerequisites == null ||
                    (terrainDef.researchPrerequisites?.First()?.prerequisites != null && !terrainDef.researchPrerequisites.First().prerequisites.Any())) // is not null but empty
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
                    Log.Warning($"RR.SS: Error during terrain research project assingment with {terrain}: {e}");
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
                    Log.Warning($"RR.SS: Error during terrain override research project assingment with {terrainOveride}: {e}");
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
            else 
            {                
                if (terrain.CostList.Any(t => t.thingDef?.stuffProps?.categories != null))
                {
                    if (terrain.CostList.Any(t => t.thingDef.stuffProps.categories.Where(c => c == StuffCategoryDefOf.Metallic) != null))
                    {
                        terrain.researchPrerequisites.Add(ResearchProjectDef.Named("Smithing"));
                    }
                    else if (terrain.CostList.Any(t => t.thingDef.stuffProps.categories.Where(c => c == StuffCategoryDefOf.Stony) != null))
                    {
                        terrain.researchPrerequisites.Add(ResearchProjectDef.Named("Stonecutting"));
                    }
                    else if (terrain.CostList.Any(t => t.thingDef.stuffProps.categories.Where(c => c == StuffCategoryDefOf.Fabric) != null ||
                                                       t.thingDef.stuffProps.categories.Where(c => c == StuffCategoryDefOf.Leathery) != null))
                    {
                        terrain.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_Tailoring);
                    }
                }            
                terrain.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_IndoorFlooring);
            }
        }

        public static void OverridePrerequisitesToTerrain(TerrainDef terrain)
        {
            if (terrain.tags != null && !terrain.tags.Any(t => t == "Ship" || t == "Space"))
            {
                if (terrain.CostList.Any(t => t.thingDef?.stuffProps?.categories?.Where(c => c == StuffCategoryDefOf.Stony) != null))
                {
                    if (!terrain.researchPrerequisites.Contains(ResearchProjectDef.Named("Stonecutting")))
                        terrain.researchPrerequisites.Add(ResearchProjectDef.Named("Stonecutting"));
                    terrain.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_IndoorFlooring);
                }
                else if (terrain.CostList.Any(t => t.thingDef?.stuffProps?.categories?.Where(c => c == StuffCategoryDefOf.Metallic) != null))
                {
                    if (terrain.researchPrerequisites.Contains(ResearchProjectDef.Named("Stonecutting")))
                    {                    
                        var research = terrain.researchPrerequisites.First(r => r.defName == "Stonecutting");
                        if (research != null)
                        {
                            terrain.researchPrerequisites.Replace(research, ResearchProjectDef.Named("Electricity"));
                        }
                    }

                    if (!terrain.researchPrerequisites.Contains(ResearchProjectDef.Named("Smithing")))
                        terrain.researchPrerequisites.Add(ResearchProjectDef.Named("Smithing"));
                    terrain.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_IndoorFlooring);

                }
            }
        }
    }
}
