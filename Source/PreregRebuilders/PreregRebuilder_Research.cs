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

    //Unessessary and old.

   /* public static partial class PreregRebuilder
    {
        private static void DoResearch()
        {
            var noProjectResearchDefs = new HashSet<ResearchProjectDef>();
            foreach (var researchDef in DefDatabase<ResearchProjectDef>.AllDefsListForReading.Where((ResearchProjectDef r) => r != null
            && (r.prerequisites == null || !r.prerequisites.Any()) && (r.hiddenPrerequisites == null || !r.hiddenPrerequisites.Any())))
            {
                noProjectResearchDefs.Add(researchDef);
            }

            foreach (var research in noProjectResearchDefs)
            {
                try
                {
                    GivePrerequisitesToResearch(research);
                }
                catch (Exception e)
                {
                    Log.Warning($"RR.SS: Error during research project assingment: {e}");
                }
            }
        }

        private static void GivePrerequisitesToResearch(ResearchProjectDef research)
        {
            if (research.prerequisites == null && research.hiddenPrerequisites == null)
                research.prerequisites = new List<ResearchProjectDef>();

            var firstProjects = RecipeDefExtensions.FindEarliestPrerequisiteProjects(null, research);
            firstProjects = FilterOutSuperEarlyTechs(firstProjects);
            firstProjects = FilterCopy(firstProjects, research);

            if (firstProjects != null && firstProjects.Any())
            {
                research.prerequisites.AddRange(firstProjects);
            }
            else if (!research.prerequisites.Any())
            {
                if (research.techLevel >= TechLevel.Industrial)
                {
                    if (research.UnlockedDefs.First() is ThingDef thing)
                    { 
                        if(thing.BuildableByPlayer && thing.IsElectrical())
                            research.prerequisites.Add(ResearchProjectDef.Named("Electricity"));
                    }
                    else
                        research.prerequisites.Add(ResearchProjectDef.Named("Smithing"));
                }
                else if (research.techLevel >= TechLevel.Medieval)
                    research.prerequisites.Add(ResearchProjectDefOf_Custom.RR_MethodicalResearch);

            }
        }
    }*/
}
