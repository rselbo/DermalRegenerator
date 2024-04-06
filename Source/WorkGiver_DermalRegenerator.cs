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
    internal class WorkGiver_DermalRegenerator : WorkGiver_Scanner
    {
        public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForDef(DermalRegenerator_ThingDefOf.DermalRegenerator);
        public override PathEndMode PathEndMode => PathEndMode.InteractionCell;
        public override Danger MaxPathDanger(Pawn pawn)
        {
            return Danger.Deadly;
        }

        public override bool ShouldSkip(Pawn pawn, bool forced = false)
        {
            if(!Utilities.HasDermalInjury(pawn))
            {
                Utilities.DebugLog($"{pawn.ToString()} ShouldSkip has no valid injuries");
                return true;
            }
            if(Utilities.HasDermalSickness(pawn) && !forced)
            {
                Utilities.DebugLog($"{pawn.ToString()} ShouldSkip already has dermal sickness");
                return true;
            }

            IEnumerable<Building> allDermalRegenerators = pawn.Map.listerBuildings.AllBuildingsColonistOfDef(DermalRegenerator_ThingDefOf.DermalRegenerator);
            foreach (Building building in allDermalRegenerators)
            {
                Building_DermalRegeneratorNew dermalBuilding = building as Building_DermalRegeneratorNew;
                if (dermalBuilding != null && dermalBuilding.ActiveTreatement())
                {
                    Utilities.DebugLog($"{pawn.ToString()} ShouldSkip regenerator {dermalBuilding.ToString()} is already doing a treatement");
                    continue;
                }

                CompPowerTrader comp = building.GetComp<CompPowerTrader>();
                if ((comp == null || comp.PowerOn) && building.Map.designationManager.DesignationOn(building, DesignationDefOf.Uninstall) == null)
                {
                    Utilities.DebugLog($"{pawn.ToString()} Should not skip");
                    return false;
                }

            }

            Utilities.DebugLog($"{pawn.ToString()} ShouldSkip no building");
            return true;
        }

        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            Utilities.DebugLog($"{pawn.ToString()} on Thing {t.ToString()} WorkGiver_DermalRegenerator.HasJobOnThing");

            if (t.Faction != pawn.Faction)
            {
                return false;
            }

            Building building = t as Building;
            if (building == null)
            {
                return false;
            }

            DermalRegeneratorComp dermalRegeneratorComp = building.GetComp<DermalRegeneratorComp>();
            if ((dermalRegeneratorComp == null || !dermalRegeneratorComp.AutomaticHeal) && !forced)
            {
                Utilities.DebugLog($"{pawn.ToString()} Skipping job due to no AutomaticHeal");
                return false;
            }


            if (building.IsForbidden(pawn))
            {
                return false;
            }

            if (!pawn.CanReserve(building, 1, -1, null, forced))
            {
                return false;
            }

            if (building.Map.designationManager.DesignationOn(building, DesignationDefOf.Uninstall) != null)
            {
                return false;
            }

            if (building.IsBurning())
            {
                return false;
            }
            Utilities.DebugLog($"{pawn.ToString()} on Thing {t.ToString()} WorkGiver_DermalRegenerator.HasJobOnThing returning true");

            return true;
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            Utilities.DebugLog($"{pawn.ToString()} WorkGiver_DermalRegenerator.JobOnThing {DermalRegenerator_JobDefOf.DermalRegeneratorHealScars.ToString()}");
            return JobMaker.MakeJob(DermalRegenerator_JobDefOf.DermalRegeneratorHealScars, t);
        }

    }
}
