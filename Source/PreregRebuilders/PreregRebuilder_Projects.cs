using PeteTimesSix.ResearchReinvented_SteppingStones.DefOfs;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using RimWorld;

namespace PeteTimesSix.ResearchReinvented_SteppingStones.PreregRebuilders
{
    public static partial class PreregRebuilder
    {
        public static void DoProjects()
        {
            var noProjectProjects = new HashSet<ResearchProjectDef>();
            foreach (var projectDef in DefDatabase<ResearchProjectDef>.AllDefsListForReading.Where(p =>
                (p.prerequisites == null || p.prerequisites.Count == 0) &&
                (p.hiddenPrerequisites == null || p.hiddenPrerequisites.Count == 0)))
            {
                noProjectProjects.Add(projectDef);
            }
            noProjectProjects = noProjectProjects.Except(SuperEarlyTechs).ToHashSet();

            foreach (var project in noProjectProjects)
            {
                try
                {
                    GivePrerequisitesToProject(project);
                    //Log.Message("New prerequisite for research newResearch added: " + newResearch);
                }
                catch (Exception e)
                {
                    Log.Warning($"RR.SS: Error during research newResearch assingment: {e}");
                }
            }
        }

        public static void GivePrerequisitesToProject(ResearchProjectDef project)
        {
            if (project.prerequisites == null)
                project.prerequisites = new List<ResearchProjectDef>();

            var unlockPoints = new Dictionary<ResearchProjectDef, int>();
            TotalUnlockPoints(project, unlockPoints);
            if (unlockPoints.Any())
            {
                var maxPoints = unlockPoints.Max(kv => kv.Value);
                var newPreregs = unlockPoints.Where(kv => kv.Value == maxPoints).Select(kv => kv.Key); //in case of equal amounts
                project.prerequisites.AddRange(newPreregs);
            }
            else
            {
                if(project.techLevel >= TechLevel.Medieval)
                {
                    project.prerequisites.Add(ResearchProjectDefOf_Custom.RR_MethodicalResearch);
                }
                else 
                project.prerequisites.Add(ResearchProjectDefOf_Custom.RR_Crafting);
            }
        }

        private static void TotalUnlockPoints(ResearchProjectDef project, Dictionary<ResearchProjectDef, int> unlockPoints)
        {
            var unlocks = project.UnlockedDefs;
            foreach (var def in unlocks)
            {
                if (def is TerrainDef terrain)
                {
                    if (terrain.BuildableByPlayer)
                    {
                        if (terrain.costList == null || !terrain.costList.Any())
                        {
                            unlockPoints.AddPoint(ResearchProjectDefOf_Custom.RR_Agriculture);
                        }
                        else
                        {
                            //buildable.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_FundamentalConstruction);
                        }
                    }
                }
                else if (def is ThingDef thing)
                {
                    if (thing.plant != null)
                    {
                        var plant = thing.plant;
                        if (plant.Sowable)
                        {
                            unlockPoints.AddPoint(ResearchProjectDefOf_Custom.RR_Agriculture);
                        }
                    }
                    else if (thing.BuildableByPlayer)
                    {
                        var buildable = thing;
                        if (buildable.IsButcherer()) { unlockPoints.AddPoint(HasOwnResearch(buildable, project, ResearchProjectDefOf_Custom.RR_Butchering)); }
                        if (buildable.IsDoorOrSimilar()) { unlockPoints.AddPoint(HasOwnResearch(buildable, project, ResearchProjectDefOf_Custom.RR_Doors)); }
                        if (buildable.IsFire()) { unlockPoints.AddPoint(HasOwnResearch(buildable, project, ResearchProjectDefOf_Custom.RR_Firemaking)); }
                        if (buildable.IsIdeological()) { unlockPoints.AddPoint(HasOwnResearch(buildable, project, ResearchProjectDefOf_Custom.RR_ReligiousThinking)); }
                        if (buildable.IsTentOrSimilar()) { unlockPoints.AddPoint(HasOwnResearch(buildable, project, ResearchProjectDefOf_Custom.RR_Tailoring)); }
                        if (buildable.IsGrave()) { unlockPoints.AddPoint(HasOwnResearch(buildable, project, ResearchProjectDefOf_Custom.RR_BurialRites)); }
                        if (buildable.IsTrap()) { unlockPoints.AddPoint(HasOwnResearch(buildable, project, ResearchProjectDefOf_Custom.RR_BasicTraps)); }
                        if (buildable.IsElectrical()) { unlockPoints.AddPoint(HasOwnResearch(buildable, project, ResearchProjectDef.Named("Electricity"))); }
                        if (buildable.IsCraftingFacility()) { unlockPoints.AddPoint(HasOwnResearch(buildable, project, ResearchProjectDefOf_Custom.RR_Crafting)); }
                        if (buildable.IsFurniture()) { unlockPoints.AddPoint(HasOwnResearch(buildable, project, ResearchProjectDefOf_Custom.RR_BasicFurniture)); }
                        if (buildable.IsStructure()) { unlockPoints.AddPoint(HasOwnResearch(buildable, project, ResearchProjectDefOf_Custom.RR_Walls)); }
                       
                        //else
                        {
                            //buildable.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_FundamentalConstruction);
                        }
                    }
                    else
                    {
                        AddPointsForThing(unlockPoints, thing, project);
                    }
                }
                else if (def is RecipeDef recipe)
                {
                    if (recipe.IsSurgery)
                    {
                        if (recipe.targetsBodyPart)
                        {
                            //recipe.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_FundamentalSurgery);
                        }
                        else
                        {
                            //recipe.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_FundamentalMedicine);
                        }
                    }
                    else
                    {
                        if (recipe.ProducedThingDef == null)
                        {
                            //recipe.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_FundamentalLabor);
                        }
                        else
                        {
                            var product = recipe.ProducedThingDef;
                            AddPointsForThing(unlockPoints, product, project);
                        }
                    }
                }
            }
        }      

        private static void AddPointsForThing(Dictionary<ResearchProjectDef, int> unlockPoints, ThingDef product, ResearchProjectDef project)
        {
            if (product.IsWeapon)
            {
                if (product.IsRangedWeapon)
                {                    
                    unlockPoints.AddPoint(HasOwnResearch(product, project, ResearchProjectDefOf_Custom.RR_PrimitiveRangedWeapons));
                }
                else
                {
                   unlockPoints.AddPoint(HasOwnResearch(product, project, ResearchProjectDefOf_Custom.RR_PrimitiveMeleeWeapons));
                }
            }
            else if (product.IsApparel)
            {
                unlockPoints.AddPoint(HasOwnResearch(product, project, ResearchProjectDefOf_Custom.RR_PrimitiveClothing));
            }
            else if (product.IsIngestible)
            {
                if (product.IsNutritionGivingIngestible)
                {
                    unlockPoints.AddPoint(HasOwnResearch(product, project, ResearchProjectDefOf_Custom.RR_Cooking));                 
                }
                else
                {
                    unlockPoints.AddPoint(HasOwnResearch(product, project, ResearchProjectDefOf_Custom.RR_DomHerb));
                }
            }
            else
            {
                //recipe.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_FundamentalCrafting);
            }
        }

        private static ResearchProjectDef HasOwnResearch(ThingDef thing, ResearchProjectDef project, ResearchProjectDef newResearch)
        {
            var unlockPoints = new Dictionary<ResearchProjectDef, int>();
            if (thing.researchPrerequisites != null && thing.researchPrerequisites.Except(project).Any())
            {
                foreach (var research in thing.researchPrerequisites)
                {
                    if (!SuperEarlyTechs.Contains(research) && research != project)
                        unlockPoints.AddPoint(research);
                }
                if (unlockPoints.Any())
                {
                    var maxPoints = unlockPoints.Max(kv => kv.Value);
                    var newPreregs = unlockPoints.Where(kv => kv.Value == maxPoints).Select(kv => kv.Key); //in case of equal amounts
                    newResearch = newPreregs.First();
                    if (thing.researchPrerequisites.Contains(newResearch))
                    {
                        thing.researchPrerequisites.Remove(newResearch);
                    }
                    Log.Message("New Research Found: " + newResearch);
                }
            }
            return newResearch;
        }

        private static void AddPoint(this Dictionary<ResearchProjectDef, int> dict, ResearchProjectDef project)
        {
            if (!dict.ContainsKey(project))
                dict.Add(project, 1);
            else
                dict[project]++;
        }
    }
}
