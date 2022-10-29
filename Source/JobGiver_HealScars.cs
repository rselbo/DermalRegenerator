using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace DermalRegenerator
{
    internal class JobGiver_HealScars : ThinkNode_JobGiver
    {
        protected override Job TryGiveJob(Pawn pawn)
        {
            Log.Message($"{pawn.ToString()} JobGiver_HealScars.TryGiveJob");
            return new Job(DermalRegenerator_JobDefOf.DermalRegeneratorHealScars);
        }
    }
    //using [DefOf] allows us to reference any Defs we've defined in XML. 
    [DefOf]
    class DermalRegenerator_JobDefOf
    {
        public static JobDef DermalRegeneratorHealScars;
    }
}
