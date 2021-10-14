using System;
using VoxelTycoon;
using VoxelTycoon.Game.UI;
using VoxelTycoon.Localization;
using VTOL.ModSettings;

namespace GameSlowdown.UI
{
    public class SettingsWindowPage : VTOLModSettingsWindowPage
    {
        protected override void InitializeInternal(SettingsControl settingsControl)
        {
            Settings settings = Settings.Current;
            Locale locale = LazyManager<LocaleManager>.Current.Locale;
            settingsControl.AddSlider(S.SlowdownCoefficient, null,() => settings.SlowDownCoefficient, delegate (float value)
            {
                settings.SlowDownCoefficient = value;
            }, 1f, 10f, value => Math.Round(value) == 1f ? S.Off : value.ToString("N0"));

            settingsControl.AddToggle(S.SlowdownDeposits, 
                null, settings.SlowDownDeposits, delegate ()
            {
                settings.SlowDownDeposits = true;
            }, delegate ()
            {
                settings.SlowDownDeposits = false;
            });

            settingsControl.AddToggle(S.SlowdownResearch, S.SlowdownResearchDescription, settings.SlowDownResearch, delegate ()
            {
                settings.SlowDownResearch = true;
            }, delegate ()
            {
                settings.SlowDownResearch = false;
            });
        }

    }
}
