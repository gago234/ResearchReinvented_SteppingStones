using PeteTimesSix.ResearchReinvented_SteppingStones.DefOfs;
using PeteTimesSix.ResearchReinvented_SteppingStones.Extensions;
using PeteTimesSix.ResearchReinvented_SteppingStones.PreregRebuilders;
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
        public static void SetPrerequisitesOnOrphans()
        {
           
            DoBuildables();
            DoPlaceables();
            DoPlants();
            DoTerrains();
            DoRecipes();
            DoProjects();
                        

            ThingDefOf_Custom.RR_ThinkingSpot.researchPrerequisites = null;
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
        private static HashSet<ResearchProjectDef> FilterCopy(HashSet<ResearchProjectDef> projects, ResearchProjectDef itsself)
        {
            if (projects == null)
                return null;
            return projects.Except(itsself).ToHashSet();
        }
    }
}
