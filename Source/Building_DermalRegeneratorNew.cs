using System;
using System.Collections.Generic;
using System.Text;
using Verse;
using Verse.AI;
using RimWorld;
using UnityEngine;
using System.Linq;

namespace DermalRegenerator
{
    class Building_DermalRegeneratorNew : Building
    {
        enum Particles
        {
            Green,
            GreenGlow,
            Blue,
            BlueGlow
        }

        private DermalRegeneratorSettings settings;

        public Building_DermalRegeneratorNew()
        {
            settings = LoadedModManager.GetMod<DermalRegenerationMod>().GetSettings<DermalRegeneratorSettings>();
        }
        private int m_count = 0;
        private Pawn m_ownerPawn = null;
        private Hediff m_scar = null;
        private float m_scarInitialSeverity = 0.0f;
        private CompPowerTrader powerComp;

        public virtual bool UsableNow
        {
            get
            {
                return this.powerComp == null || this.powerComp.PowerOn;
            }
        }

        private MoteThrown GetMote(Particles particle)
        {
            String load = "";
            switch (particle)
            {
                case Particles.Green: load = "DermalRegenerator_Mote_HealGreen"; break;
                case Particles.GreenGlow: load = "DermalRegenerator_Mote_LightningGlowGreen"; break;
                case Particles.Blue: load = "DermalRegenerator_Mote_ScanBlue"; break;
                case Particles.BlueGlow: load = "DermalRegenerator_Mote_LightningGlowBlue"; break;
            }
            return (MoteThrown)ThingMaker.MakeThing(ThingDef.Named(load), null);
        }

        public override void ExposeData()
        {
            base.ExposeData();
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            this.powerComp = base.GetComp<CompPowerTrader>();
        }

        private void ThrowParticles(Particles particle, Vector3 loc, Map map, float minSize, float maxSize)
        {
            if (!loc.ShouldSpawnMotesAt(map) || map.moteCounter.SaturatedLowPriority)
            {
                return;
            }

            MoteThrown moteThrown = GetMote(particle);
            moteThrown.Scale = Rand.Range(minSize, maxSize);
            moteThrown.exactPosition = loc;
            moteThrown.exactPosition += new Vector3(0.5f, 0f, 0.5f);
            moteThrown.SetVelocity((float)Rand.Range(0, 2), Rand.Range(0.003f, 0.003f));
            GenSpawn.Spawn(moteThrown, loc.ToIntVec3(), map);
        }

        public override string GetInspectString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            String inspect = base.GetInspectString();
            if (!inspect.NullOrEmpty())
                stringBuilder.Append(inspect);
            if (m_ownerPawn == null)
            {
                if (stringBuilder.Length > 0) stringBuilder.AppendLine();
                stringBuilder.Append("Waiting for patient.");
            }

            if (m_ownerPawn != null && m_scar != null)
            {
                float progress = 100 / m_scarInitialSeverity * (m_scarInitialSeverity - m_scar.Severity);
                if (stringBuilder.Length > 0) stringBuilder.AppendLine();
                stringBuilder.Append($"Treating {m_scar.Label} on {m_ownerPawn.ToString()} Progress: {progress:F0}%");
            }
            return stringBuilder.ToString();
        }

        public override void Tick()
        {
            base.Tick();

            if(m_ownerPawn != null)
            {
                Map.glowGrid.VisualGlowAt(Position);
                if (m_count < 4500)
                {
                    if (m_count % 90 == 0)
                    {
                        ThrowParticles(Particles.Blue, Position.ToVector3(), Map, 1.2f, 1.5f);
                    }
                    if (m_count % 200 == 0)
                    {
                        ThrowParticles(Particles.BlueGlow, Position.ToVector3(), Map, 6f, 6f);
                    }
                }
                else if (m_count > 4500)
                {
                    if (m_count % 90 == 0)
                    {
                        ThrowParticles(Particles.Green, Position.ToVector3(), Map, 1.2f, 1.5f);
                    }
                    if (m_count % 200 == 0)
                    {
                        ThrowParticles(Particles.GreenGlow, Position.ToVector3(), Map, 6f, 6f);
                    }
                }

                m_count++;
            }
        }

        public void StartTreatement(Pawn pawn, Hediff scar)
        {
            m_count = 0;
            m_ownerPawn = pawn;
            m_scar = scar;
            m_scarInitialSeverity = scar.Severity;
            CompPowerTrader power = GetComp<CompPowerTrader>();
            power.PowerOutput = -settings.PowerUsageWorking;
        }

        public void StopTreatement(Pawn pawn)
        {
            m_ownerPawn = null;
            m_scar = null;
            CompPowerTrader power = GetComp<CompPowerTrader>();
            power.PowerOutput = -settings.PowerUsageIdle;
        }
    }

    class DermalRegeneratorComp : ThingComp
    {
        public CompProperties_DermalRegenerator Props => (CompProperties_DermalRegenerator)this.props;
        public bool AutomaticHeal => Props.AutomaticHeal;

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            Command_Toggle commandToggle = new Command_Toggle();
            commandToggle.defaultLabel = (string)"AutomaticHeal";
            commandToggle.defaultDesc = (string)"AutomaticHeal";
            commandToggle.hotKey = KeyBindingDefOf.Command_ItemForbid;

            //CompProperties_AssignableToPawn
            commandToggle.icon = ContentFinder<Texture2D>.Get("UI/Icons/ColonistBar/MedicalRest"); 
            commandToggle.isActive = () => Props.AutomaticHeal;
            commandToggle.toggleAction = () => Props.AutomaticHeal = !Props.AutomaticHeal;
            yield return (Gizmo)commandToggle;
        }
    }
    public class CompProperties_DermalRegenerator : CompProperties
    {
        public bool AutomaticHeal = false;
        public CompProperties_DermalRegenerator()
        {
            this.compClass = typeof(DermalRegeneratorComp);
        }
    }

    [DefOf]
    class DermalRegenerator_ThingDefOf
    {
        public static ThingDef DermalRegenerator;
    }
}
