using Verse;

namespace DermalRegenerator
{
    class DermalRegeneratorSettings : ModSettings
    {
        public float RegenerationRate = 100;
        public float SicknessRate = 100;
        public float SeverityReductionPerDay = 5;
        public int PowerUsageIdle = 100;
        public int PowerUsageWorking = 1000;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref RegenerationRate, "RegenerationRate");
            Scribe_Values.Look(ref SicknessRate, "DermalSicknessRate");
            Scribe_Values.Look(ref SeverityReductionPerDay, "DermalSeverityReductionPerDay");
            Scribe_Values.Look(ref PowerUsageIdle, "PowerUsageIdle");
            Scribe_Values.Look(ref PowerUsageWorking, "PowerUsageWorking");
            base.ExposeData();
        }

    }
}
