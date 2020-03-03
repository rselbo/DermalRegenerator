using Verse;

namespace DermalRegenerator
{
    class DermalRegeneratorSettings : ModSettings
    {
        public float RegenerationRate = 100;
        public float SicknessRate = 100;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref RegenerationRate, "RegenerationRate");
            Scribe_Values.Look(ref SicknessRate, "DermalSicknessRate");
            base.ExposeData();
        }

    }
}
