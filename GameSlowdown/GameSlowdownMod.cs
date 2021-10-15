using GameSlowdown.UI;
using HarmonyLib;
using JetBrains.Annotations;
using VoxelTycoon.Modding;
using VTOL.ModSettings;

namespace GameSlowdown
{
    [UsedImplicitly]
    public class GameSlowdownMod: Mod
    {
        private Harmony _harmony;
        private const string HarmonyID = "cz.xmnovotny.gameslowdown.patch";

        protected override void Initialize()
        {
            Harmony.DEBUG = false;
            _harmony = new Harmony(HarmonyID);
            FileLog.Reset();
            _harmony.PatchAll();
        }

        protected override void OnGameStarted()
        {
            VTOLModSettingsWindowManager.Current.Register<SettingsWindowPage>(VTOLModSettings<Settings>.Current.ModPackName, "Game slowdown");
        }

        protected override void Deinitialize()
        {
            _harmony.UnpatchAll(HarmonyID);
            _harmony = null;
        }
        
    }
}