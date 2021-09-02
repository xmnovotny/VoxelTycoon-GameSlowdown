using VoxelTycoon;
using VoxelTycoon.Localization;

namespace GameSlowdown
{
    public static class S
    {
        public static string SlowdownCoefficient => LazyManager<LocaleManager>.Current.Locale.GetString("game_slowdown/slowdown_coefficient");
        public static string SlowdownDeposits => LazyManager<LocaleManager>.Current.Locale.GetString("game_slowdown/slowdown_deposits");
        public static string SlowdownResearch => LazyManager<LocaleManager>.Current.Locale.GetString("game_slowdown/slowdown_research");
        public static string SlowdownResearchDescription => LazyManager<LocaleManager>.Current.Locale.GetString("game_slowdown/slowdown_research_description");
        public static string Off => LazyManager<LocaleManager>.Current.Locale.GetString("game_slowdown/off");

    }
}