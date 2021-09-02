using GameSlowdown.UI;
using HarmonyLib;
using ModSettingsUtils;
using VoxelTycoon.Modding;
using XMNUtils;

namespace GameSlowdown
{
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
            ModSettingsWindowManager.Current.Register<SettingsWindowPage>("GameSlowdown", "Game slowdown");
        }

        protected override void Deinitialize()
        {
            _harmony.UnpatchAll(HarmonyID);
            _harmony = null;
        }
        
    }
}