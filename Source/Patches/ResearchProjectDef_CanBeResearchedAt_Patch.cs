using System;
using System.Collections.Generic;
using HarmonyLib;
using PeteTimesSix.ResearchReinvented_SteppingStones.DefOfs;
using RimWorld;
using Verse;

namespace PeteTimesSix.ResearchReinvented_SteppingStones.Extensions
{
	[HarmonyPatch(typeof(ResearchProjectDef), "CanBeResearchedAt")]
	public static class ResearchProjectDef_CanBeResearchedAt_Patch
	{
		public static void Postfix(Building_ResearchBench bench, bool ignoreResearchBenchPowerStatus, ResearchProjectDef __instance, ref bool __result)
		{

			if (__result && bench.def.defName.Contains("RR_ThinkingSpot") && __instance.techLevel > TechLevel.Neolithic && __instance.defName != "TB_MedievalTechLock" && __instance.defName != "TB_MedievalTheory" && __instance != ResearchProjectDefOf_Custom.RR_MethodicalResearch)
			{
				__result = false;
			}
        }
    }
}
