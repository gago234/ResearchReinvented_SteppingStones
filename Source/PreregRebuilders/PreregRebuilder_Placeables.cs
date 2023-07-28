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
        private static void DoPlaceables()
        {
            var noProjectPlaceableDefs = new HashSet<ThingDef>();
            foreach (var thingDef in DefDatabase<ThingDef>.AllDefsListForReading.Where(t => t.placeWorkers != null))
            {
                noProjectPlaceableDefs.Add(thingDef);
            }

            foreach (var palaceable in noProjectPlaceableDefs)
            {
                try
                {
                    GivePrerequisitesToPlaceables(palaceable);
                }
                catch (Exception e)
                {
                    Log.Warning($"RR.SS: Error during palaceable research project assingment with {palaceable}: {e}");
                }
            }
        }
        private static void GivePrerequisitesToPlaceables(ThingDef placable)
        {
            if (placable.researchPrerequisites == null)
                placable.researchPrerequisites = new List<ResearchProjectDef>();

            if (placable.thingClass == typeof(Building_Art) || placable.thingCategories.NotNullAndContains(ThingCategoryDefOf.BuildingsArt))
            {
                placable.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_Art);
            }
        }

    }   
}
