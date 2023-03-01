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
    public static partial class PreregRebuilder
    {

        public static void DoRecipes()
        {
            var noProjectRecipeDefs = new HashSet<RecipeDef>();
            var noProjectSurgeryRecipeDefs = new HashSet<RecipeDef>();
            var noProjectFullBodySurgeryRecipeDefs = new HashSet<RecipeDef>();
            foreach (var recipeDef in DefDatabase<RecipeDef>.AllDefsListForReading.Where(r => !r.NoResearchPrerequisites()))
            {
                if (recipeDef.IsSurgery)
                {
                    if (recipeDef.targetsBodyPart)
                    {
                        noProjectSurgeryRecipeDefs.Add(recipeDef);
                    }
                    else
                    {
                        noProjectFullBodySurgeryRecipeDefs.Add(recipeDef);
                    }
                }
                else
                {
                    noProjectRecipeDefs.Add(recipeDef);
                }
            }

            foreach (var recipe in noProjectRecipeDefs)
            {
                try
                {
                    GivePrerequisitesToRecipe(recipe);
                }
                catch (Exception e)
                {
                    Log.Warning($"RR.SS: Error during research project assingment: {e}");
                }
            }

            foreach (var recipe in noProjectSurgeryRecipeDefs)
            {
                try
                {
                    GivePrerequisitesToSurgery(recipe);
                }
                catch (Exception e)
                {
                    Log.Warning($"RR.SS: Error during research project assingment: {e}");
                }
            }

            foreach (var recipe in noProjectFullBodySurgeryRecipeDefs)
            {
                try
                {
                    GivePrerequisitesToFullBodySurgery(recipe);
                }
                catch (Exception e)
                {
                    Log.Warning($"RR.SS: Error during research project assingment: {e}");
                }
            }
        }

        private static void GivePrerequisitesToRecipe(RecipeDef recipe) 
        {
            if (recipe.researchPrerequisites == null)
                recipe.researchPrerequisites = new List<ResearchProjectDef>();

            var firstProjects = recipe.FindEarliestPrerequisiteProjects();
            firstProjects = FilterOutSuperEarlyTechs(firstProjects);
            if (firstProjects != null && firstProjects.Any())
            {
                recipe.researchPrerequisites.AddRange(firstProjects);
                return;
            }

            //the else needs perhaps to be removed.
            else if (recipe.ProducedThingDef != null)
            {
                if (recipe.ProducedThingDef.IsWeapon && recipe.ProducedThingDef.techLevel == TechLevel.Neolithic)
                {
                    if (recipe.ProducedThingDef.IsRangedWeapon)
                    {
                        recipe.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_PrimitiveRangedWeapons);
                    }
                    else
                    {
                        recipe.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_PrimitiveMeleeWeapons);
                    }
                }
                else if (recipe.ProducedThingDef.IsApparel)
                {
                    if (recipe.ProducedThingDef.techLevel <= TechLevel.Neolithic)
                        recipe.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_PrimitiveClothing);
                    else
                        recipe.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_Tailoring);
                }
                else if (recipe.ProducedThingDef.IsIngestible)
                {
                    if (recipe.ProducedThingDef.IsNutritionGivingIngestible)
                    {
                        if (recipe.ProducedThingDef.GetStatValueAbstract(StatDefOf.Nutrition) < 0.09f)
                            recipe.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_PrimitiveCooking);
                        else
                            recipe.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_Cooking);
                    }
                    else
                    {
                        recipe.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_DomHerb);
                    }
                }
                else
                {

                }
            }
        }

        private static void GivePrerequisitesToSurgery(RecipeDef recipe) 
        {
            if (recipe.researchPrerequisites == null)
                recipe.researchPrerequisites = new List<ResearchProjectDef>();

            var firstProjects = recipe.FindEarliestPrerequisiteProjects();
            firstProjects = FilterOutSuperEarlyTechs(firstProjects);
            if (firstProjects != null && firstProjects.Any())
            {
                recipe.researchPrerequisites.AddRange(firstProjects);
            }
            //recipe.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_FundamentalSurgery);
        }

        private static void GivePrerequisitesToFullBodySurgery(RecipeDef recipe) 
        { 
            if (recipe.researchPrerequisites == null)
                recipe.researchPrerequisites = new List<ResearchProjectDef>();

            var firstProjects = recipe.FindEarliestPrerequisiteProjects();
            firstProjects = FilterOutSuperEarlyTechs(firstProjects);
            if (firstProjects != null && firstProjects.Any())
            {
                recipe.researchPrerequisites.AddRange(firstProjects);
            }
            //recipe.researchPrerequisites.Add(ResearchProjectDefOf_Custom.RR_FundamentalMedicine);
        }
    }
}
