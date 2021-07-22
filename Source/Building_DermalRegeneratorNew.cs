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
        private DermalRegeneratorSettings settings;
        private bool DebugLog = false;

        public Building_DermalRegeneratorNew()
        {
            settings = LoadedModManager.GetMod<DermalRegenerationMod>().GetSettings<DermalRegeneratorSettings>();
        }
        private int count = 0;
        private Pawn JobPawn = null;
        private Pawn OwnerPawn = null;
        private Hediff foundInj = null;
        private float foundInfInitialServerity = 0.0f;
        private Hediff dermalSickness = null;
        private CompPowerTrader powerComp;

        public virtual bool UsableNow
        {
            get
            {
                return this.powerComp == null || this.powerComp.PowerOn;
            }
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


        private bool HasDermalInjury(Pawn pawn)
        {
            return pawn.health.hediffSet.hediffs
                .Where(hediff => hediff is Hediff_Injury && hediff.IsPermanent())
                .Any();
        }

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn myPawn)
        {
            List<FloatMenuOption> list = new List<FloatMenuOption>();
            {
                if (!myPawn.CanReserve(this))
                {
                    return new List<FloatMenuOption> { new FloatMenuOption("Cannot use, reserved by another pawn", null) };
                }
                if (!myPawn.CanReach(this, PathEndMode.Touch, Danger.Some))
                {
                    return new List<FloatMenuOption> { new FloatMenuOption("Cannot use, no path to Dermal Regenerator", null) };
                }
                if(!HasDermalInjury(myPawn))
                {
                    return new List<FloatMenuOption> { new FloatMenuOption("No injury to heal", null) };
                }

                if (OwnerPawn == null )
                {

                    Action action = delegate
                    {
                        var job1 = new Job(JobDefOf.Goto, this.InteractionCell);
                        var job2 = new Job(JobDefOf.Wait);
                        myPawn.jobs.TryTakeOrderedJob(job1);
                        myPawn.jobs.jobQueue.EnqueueFirst(job2);
                        JobPawn = myPawn;
                        myPawn.Reserve(this, job1);
                        if(DebugLog) Log.Message($"{JobPawn.ToString()} going to start treatment");
                    };
                    list.Add(new FloatMenuOption("Use Dermal Regenerator", action));
                }
            }
            return list;
        }

        public static void ThrowMicroSparksGreen(Vector3 loc, Map map)
        {
            if (!loc.ShouldSpawnMotesAt(map) || map.moteCounter.SaturatedLowPriority)
            {
                return;
            }
            MoteThrown moteThrown = (MoteThrown)ThingMaker.MakeThing(ThingDef.Named("DermalRegenerator_Mote_HealGreen"), null);
            moteThrown.Scale = Rand.Range(1.2f, 1.5f);
            //moteThrown.exactRotationRate = Rand.Range(0f, 0f);
            moteThrown.exactPosition = loc;
            moteThrown.exactPosition += new Vector3(0.5f, 0f, 0.5f);
            //moteThrown.exactPosition += new Vector3(Rand.Value, 0f, Rand.Value);
            moteThrown.SetVelocity((float)Rand.Range(0, 2), Rand.Range(0.003f, 0.003f));
            GenSpawn.Spawn(moteThrown, loc.ToIntVec3(), map);
        }

        public static void ThrowMicroSparksBlue(Vector3 loc, Map map)
        {
            if (!loc.ShouldSpawnMotesAt(map) || map.moteCounter.SaturatedLowPriority)
            {
                return;
            }
            MoteThrown moteThrown = (MoteThrown)ThingMaker.MakeThing(ThingDef.Named("DermalRegenerator_Mote_ScanBlue"), null);
            moteThrown.Scale = Rand.Range(1.2f, 1.5f);
            //moteThrown.exactRotationRate = Rand.Range(0f, 0f);
            moteThrown.exactPosition = loc;
            moteThrown.exactPosition += new Vector3(0.5f, 0f, 0.5f);
            //moteThrown.exactPosition += new Vector3(Rand.Value, 0f, Rand.Value);
            moteThrown.SetVelocity((float)Rand.Range(0, 2), Rand.Range(0.003f, 0.003f));
            GenSpawn.Spawn(moteThrown, loc.ToIntVec3(), map);
        }

        public static void ThrowLightningGlowBlue(Vector3 loc, Map map, float size)
        {
            if (!loc.ShouldSpawnMotesAt(map) || map.moteCounter.SaturatedLowPriority)
            {
                return;
            }
            MoteThrown moteThrown = (MoteThrown)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("DermalRegenerator_Mote_LightningGlowBlue", true), null);
            moteThrown.Scale = 6f * size;
            moteThrown.rotationRate = 0f;
            moteThrown.exactPosition = loc;
            moteThrown.exactPosition += new Vector3(0.5f, 0f, 0.5f);
            //moteThrown.SetVelocityAngleSpeed((float)Rand.Range(0, 0), Rand.Range(0.0002f, 0.0002f));
            GenSpawn.Spawn(moteThrown, loc.ToIntVec3(), map);
        }

        public static void ThrowLightningGlowGreen(Vector3 loc, Map map, float size)
        {
            if (!loc.ShouldSpawnMotesAt(map) || map.moteCounter.SaturatedLowPriority)
            {
                return;
            }
            MoteThrown moteThrown = (MoteThrown)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("DermalRegenerator_Mote_LightningGlowGreen", true), null);
            moteThrown.Scale = 6f * size;
            moteThrown.rotationRate = 0f;
            moteThrown.exactPosition = loc;
            moteThrown.exactPosition += new Vector3(0.5f, 0f, 0.5f);
            //moteThrown.SetVelocityAngleSpeed((float)Rand.Range(0, 0), Rand.Range(0.0002f, 0.0002f));
            GenSpawn.Spawn(moteThrown, loc.ToIntVec3(), map);
        }

        public override string GetInspectString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            String inspect = base.GetInspectString();
            if (!inspect.NullOrEmpty())
                stringBuilder.Append(inspect);
            if (JobPawn != null && OwnerPawn == null)
            {
                if (stringBuilder.Length > 0) stringBuilder.AppendLine();
                stringBuilder.Append( "Waiting for patient.");
            }

            if (OwnerPawn != null && foundInj != null)
            {
                float progress = 100 / foundInfInitialServerity * (foundInfInitialServerity - foundInj.Severity);
                if (stringBuilder.Length > 0) stringBuilder.AppendLine();
                stringBuilder.Append(inspect + "\n" + $"Treating {foundInj.Label} Progress: {progress:F0}%");
            }
            return stringBuilder.ToString();
        }

        public override void Tick()
        {
            base.Tick();
            if (OwnerPawn != null && OwnerPawn.Position != this.InteractionCell)
            {
                if (DebugLog) Log.Message($"{OwnerPawn.ToString()} moved off the interaction cell while working");
                ResetMachine();
            }
            
            if (JobPawn != null)
            {
                if (OwnerPawn != null)
                {
                    string messageText = "Dermal Regenerator in use.";
                    Messages.Message(messageText, MessageTypeDefOf.NegativeEvent);
                    JobPawn.jobs.EndCurrentJob(JobCondition.InterruptForced);
                    JobPawn = null;
                    return;
                }
                else
                {
                    if (this.UsableNow)
                    {
                        if (JobPawn.Position == this.InteractionCell)
                        {
                            if (DebugLog) Log.Message($"{JobPawn.ToString()} entered the interaction cell");
                            OwnerPawn = JobPawn;
                            JobPawn = null;
                            return;
                        }
                    }
                    else if (OwnerPawn == null && !this.UsableNow)
                    {
                        string messageText = "Dermal Regenerator doesn't have power.";
                        Messages.Message(messageText, MessageTypeDefOf.NegativeEvent);
                        JobPawn.jobs.EndCurrentJob(JobCondition.InterruptForced);
                        JobPawn = null;
                        ResetMachine();
                        return;
                    }
                }
            }
            if (OwnerPawn != null && !this.UsableNow)
            {
                string messageText = "Dermal Regenerator power interupted.";
                Messages.Message(messageText, MessageTypeDefOf.NegativeEvent);
                ResetMachine();
                return;
            }

            if (OwnerPawn != null && OwnerPawn.Position == this.InteractionCell && this.UsableNow)
            {
                CompPowerTrader power = GetComp<CompPowerTrader>();
                power.PowerOutput = settings.PowerUsageWorking * -1;

                Map.glowGrid.VisualGlowAt(Position);
                if (count < 4500)
                {
                    if (count % 90 == 0)
                    {
                        ThrowMicroSparksBlue(Position.ToVector3(), Map);
                    }
                    if (count % 200 == 0)
                    {
                        ThrowLightningGlowBlue(Position.ToVector3(), Map, 1f);
                    }
                }
                else if (count > 4500)
                {
                    if (count % 90 == 0)
                    {
                        ThrowMicroSparksGreen(Position.ToVector3(), Map);
                    }
                    if (count % 200 == 0)
                    {
                        ThrowLightningGlowGreen(Position.ToVector3(), Map, 1f);
                    }
                }

                count++;

                if (dermalSickness == null)
                {
                    dermalSickness = GetOrAddDermalSickness();
                }

                foreach (var current in OwnerPawn.health.hediffSet.GetHediffs<Hediff_Injury>())
                {
                    if (current.IsPermanent())
                    {
                        if(foundInfInitialServerity == 0.0f) foundInfInitialServerity = current.Severity;

                        foundInj = current;
                        dermalSickness.Severity += 0.00084f * (settings.SicknessRate / 100.0f);
                        current.Heal(0.00075f * (settings.RegenerationRate / 100.0f));
                        if (current.ShouldRemove)
                        {
                            string messageText = $"Treatment of {current.Label} is complete.";
                            Messages.Message(messageText, MessageTypeDefOf.PositiveEvent);
                            ResetMachine();
                        }
                        break;
                    }
                }
            }
            else
            {
                CompPowerTrader power = GetComp<CompPowerTrader>();
                power.PowerOutput = settings.PowerUsageIdle * -1;
            }
        }

        private void ResetMachine()
        {
            if (DebugLog) Log.Message($"{OwnerPawn?.ToString()} was in the machine when it reset");
            OwnerPawn.jobs.EndCurrentJob(JobCondition.InterruptForced);
            count = 0;
            foundInj = null;
            foundInfInitialServerity = 0.0f;
            OwnerPawn = null;
            dermalSickness = null;
            JobPawn = null;
        }

        private Hediff GetOrAddDermalSickness()
        {
            var pawnDermalSickness = OwnerPawn.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("DermalRegeneratorSickness"));
            if (pawnDermalSickness == null)
            {
                pawnDermalSickness = OwnerPawn.health.AddHediff(HediffDef.Named("DermalRegeneratorSickness"), null, null);
                pawnDermalSickness.Severity = float.MinValue;
            }
            return pawnDermalSickness;
        }
    }
}
