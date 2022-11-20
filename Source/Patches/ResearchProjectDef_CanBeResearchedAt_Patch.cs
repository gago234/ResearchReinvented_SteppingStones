using System;
using HarmonyLib;
using RimWorld;
using Verse;

namespace ThinkingSpot
{
	[HarmonyPatch(typeof(ResearchProjectDef), "CanBeResearchedAt")]
	public static class ResearchProjectDef_CanBeResearchedAt_Patch
	{
		public static void Postfix(Building_ResearchBench bench, bool ignoreResearchBenchPowerStatus, ResearchProjectDef __instance, ref bool __result)
		{
			if (__result && bench.def.defName.Contains("RR_ThinkingSpot") && __instance.techLevel > TechLevel.Neolithic)
			{
				__result = false;
			}
        }
	}
}
