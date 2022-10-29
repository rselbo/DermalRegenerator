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
    internal class JobDriver_DermalRegenerator : JobDriver
    {
        //JobDriver_DoBill
        private const TargetIndex Regenerator = TargetIndex.A;
        private DermalRegeneratorSettings settings = LoadedModManager.GetMod<DermalRegenerationMod>().GetSettings<DermalRegeneratorSettings>();

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            Utilities.DebugLog($"{pawn.ToString()} TryMakePreToilReservations on target");
            return this.pawn.Reserve(this.job.GetTarget(Regenerator), this.job, 1, -1, null);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            Utilities.DebugLog($"{pawn.ToString()} MakeNewToils");
            //these are the fail conditions. If at any time during this toil the Dermal Regenerator becomes unavailable, our toil ends.
            this.FailOnBurningImmobile(Regenerator);

            //go to our cat. These are all vanilla Toils, which is easy to use.
            Toil gotoRegenerator = Toils_Goto.GotoThing(Regenerator, PathEndMode.InteractionCell);
            yield return gotoRegenerator;
            
            Hediff injury = Utilities.GetDermalInjury(pawn);

            Toil toil = ToilMaker.MakeToil("HealScars");
            toil.initAction = delegate
            {
                Building_DermalRegeneratorNew building = job.GetTarget(Regenerator).Thing as Building_DermalRegeneratorNew;
                Utilities.DebugLog($"{pawn.ToString()} HealScars init on {building}");
                building.StartTreatement(pawn, injury);
                toil.actor.pather.StopDead();
            };

            toil.AddEndCondition(delegate
            {
                Building_DermalRegeneratorNew building = job.GetTarget(Regenerator).Thing as Building_DermalRegeneratorNew;
                if(building == null)
                {
                    Utilities.DebugLog($"{pawn.ToString()} MakeNewToils end condition DermalRegenerator disapeared");
                    return JobCondition.Incompletable;
                }
                CompPowerTrader power = building.GetComp<CompPowerTrader>();
                if (power == null || !power.PowerOn)
                {
                    Utilities.DebugLog($"{pawn.ToString()} MakeNewToils end condition power condition");
                    return JobCondition.Incompletable;
                }
                if (injury == null)
                {
                    Utilities.DebugLog($"{pawn.ToString()} MakeNewToils end condition injury: {injury}");
                    return JobCondition.Incompletable;
                }
                if(injury.ShouldRemove)
                {
                    Utilities.DebugLog($"{pawn.ToString()} MakeNewToils end condition healed");
                    return JobCondition.Succeeded;
                }
                return JobCondition.Ongoing;
            });

            int counter = 0;
            toil.tickAction = delegate
            {
                if(++counter % 100 == 0)
                {
                    Utilities.DebugLog($"{pawn.ToString()} HealScars toil tick {counter}");
                }
                Hediff tickSickness = Utilities.GetDermalSickness(toil.actor);
                if (tickSickness == null) tickSickness = Utilities.AddDermalSickness(toil.actor);

                tickSickness.Severity += 0.00084f * (settings.SicknessRate / 100.0f);
                injury.Heal(0.00075f * (settings.RegenerationRate / 100.0f));
            };

            if(toil.finishActions == null) toil.finishActions = new List<Action>();
            toil.finishActions.Add(delegate
            {
                Building_DermalRegeneratorNew building = job.GetTarget(Regenerator).Thing as Building_DermalRegeneratorNew;
                Utilities.DebugLog($"{pawn.ToString()} HealScars finishActions on {building}");
                building.StopTreatement(pawn);
            });
            toil.FailOnDestroyedNullOrForbidden<Toil>(Regenerator);

            toil.defaultCompleteMode = ToilCompleteMode.Never;
            Utilities.DebugLog($"{pawn.ToString()} yielding toil HealScars");
            yield return toil;
        }
    }
}
