using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DermalRegenerator
{
    internal class Utilities
    {
        private static HediffDef DermalRegeneratorSickness = HediffDef.Named("DermalRegeneratorSickness");
        private static DermalRegeneratorSettings settings = LoadedModManager.GetMod<DermalRegenerationMod>().GetSettings<DermalRegeneratorSettings>();
        public static bool HasDermalSickness(Pawn pawn)
        {
            return pawn.health.hediffSet.hediffs
                .Where(hediff => hediff.def == DermalRegeneratorSickness)
                .Any();
        }
        public static Hediff GetDermalSickness(Pawn pawn)
        {
            return pawn.health.hediffSet.GetFirstHediffOfDef(DermalRegeneratorSickness);
        }
        public static Hediff AddDermalSickness(Pawn pawn)
        {
            HediffDef dermalRegeneratorSickness = HediffDef.Named("DermalRegeneratorSickness");
            HediffCompProperties_SeverityPerDay severity = (HediffCompProperties_SeverityPerDay)dermalRegeneratorSickness.comps.First(i => i.GetType() == typeof(HediffCompProperties_SeverityPerDay));
            if (severity != null)
            {
                severity.severityPerDay = -settings.SeverityReductionPerDay;
            }

            Hediff pawnDermalSickness = pawn.health.AddHediff(dermalRegeneratorSickness, null, null);
            pawnDermalSickness.Severity = 1f;
            return pawnDermalSickness;
        }

        public static bool HasDermalInjury(Pawn pawn)
        {
            return pawn.health.hediffSet.hediffs
                .Where(hediff => hediff is Hediff_Injury && hediff.IsPermanent())
                .Any();
        }
        public static Hediff GetDermalInjury(Pawn pawn)
        {
            return pawn.health.hediffSet.hediffs
                .Where(hediff => hediff is Hediff_Injury && hediff.IsPermanent())
                .FirstOrDefault();
        }
        [Conditional("DEBUG")]
        public static void DebugLog(string log)
        {
                Log.Message(log);
        }
    }
}
