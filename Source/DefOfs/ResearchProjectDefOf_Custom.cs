using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PeteTimesSix.ResearchReinvented_SteppingStones.DefOfs
{
    [DefOf]
    public static class ResearchProjectDefOf_Custom
    {
        public static ResearchProjectDef RR_Walls;
        public static ResearchProjectDef RR_Doors;
        public static ResearchProjectDef RR_Bridges;
        public static ResearchProjectDef RR_Crafting;
        public static ResearchProjectDef RR_PrimitiveButchering;
        public static ResearchProjectDef RR_Butchering;
        public static ResearchProjectDef RR_PrimitiveCooking;
        public static ResearchProjectDef RR_Cooking;
        public static ResearchProjectDef RR_MethodicalResearch;
        public static ResearchProjectDef RR_Tailoring;
        public static ResearchProjectDef RR_Bedrolls;
        public static ResearchProjectDef RR_BasicFurniture;
        public static ResearchProjectDef RR_Art;
        public static ResearchProjectDef RR_BasicCover;
        public static ResearchProjectDef RR_AdvancedCover;
        public static ResearchProjectDef RR_BasicTraps;
        public static ResearchProjectDef RR_BurialRites;
        public static ResearchProjectDef RR_Roads;
        public static ResearchProjectDef RR_IndoorFlooring;
        public static ResearchProjectDef RR_BasicGames;
        public static ResearchProjectDef RR_BoardGames;
        public static ResearchProjectDef RR_Firemaking;
        public static ResearchProjectDef RR_DomHerb;
        public static ResearchProjectDef RR_Agriculture;
        public static ResearchProjectDef RR_Smokeleaf;
        public static ResearchProjectDef RR_Psychoid;
        public static ResearchProjectDef RR_PrimitiveClothing;
        public static ResearchProjectDef RR_PrimitiveMeleeWeapons;
        public static ResearchProjectDef RR_PrimitiveRangedWeapons;
        public static ResearchProjectDef RR_IndustrialClothing;

        [MayRequireIdeology]
        public static ResearchProjectDef RR_ReligiousThinking;

        //vanilla
		public static ResearchProjectDef ComplexClothing;
		public static ResearchProjectDef Prosthetics;
		public static ResearchProjectDef DrugProduction;
		public static ResearchProjectDef PenoxycylineProduction;

		static ResearchProjectDefOf_Custom()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(ResearchProjectDefOf_Custom));
        }
    }
}
