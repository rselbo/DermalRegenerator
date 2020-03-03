using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace DermalRegenerator
{
    class DermalRegenerationMod : Mod
    {
        private DermalRegeneratorSettings settings;

        public DermalRegenerationMod(ModContentPack content) : base(content)
        {
            this.settings = GetSettings<DermalRegeneratorSettings>();
        }

        /// <summary>
        /// The (optional) GUI part to set your settings.
        /// </summary>
        /// <param name="inRect">A Unity Rect with the size of the settings window.</param>
        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);
            listingStandard.Label($"Dermal Regeneration Rate - { settings.RegenerationRate:0}% ");
            settings.RegenerationRate = listingStandard.Slider(settings.RegenerationRate, 0f, 300f);
            listingStandard.Label($"Dermal Sickness Rate - { settings.SicknessRate:0}% ");
            settings.SicknessRate = listingStandard.Slider(settings.SicknessRate, 0f, 300f);
            listingStandard.End();
            base.DoSettingsWindowContents(inRect);
        }

        /// <summary>
        /// Override SettingsCategory to show up in the list of settings.
        /// Using .Translate() is optional, but does allow for localisation.
        /// </summary>
        /// <returns>The (translated) mod name.</returns>
        public override string SettingsCategory()
        {
            return "Dermal Regeneration";
        }
    }
}
