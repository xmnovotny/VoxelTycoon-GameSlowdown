using System;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using ModSettingsUtils;
using UnityEngine;
using VoxelTycoon.Cities;
using VoxelTycoon.Deposits;
using VoxelTycoon.Tracks;
using XMNUtils;

namespace GameSlowdown
{
    [HarmonyPatch]
    public class GameSlowdownManager: SimpleLazyManager<GameSlowdownManager>
    {
        private Action<City,float> _setCityGrowIntervalDays;

        private void SetCityGrowIntervalDays(City city, float interval)
        {
            if (_setCityGrowIntervalDays == null)
            {
                MethodInfo mInf = typeof(City).GetMethod("set_GrowIntervalDays", BindingFlags.NonPublic | BindingFlags.Instance);
                _setCityGrowIntervalDays = (Action<City, float>)Delegate.CreateDelegate(typeof(Action<City, float>), mInf!);
            }

            _setCityGrowIntervalDays.Invoke(city, interval);
        }        
        
        
        [UsedImplicitly]
        [HarmonyPostfix]
        [HarmonyPatch(typeof(City), "InvalidateGrowIntervalDays")]
        // ReSharper disable InconsistentNaming
        private static void City_InvalidateGrowIntervalDays_pof(City __instance)
        {
            Current.SetCityGrowIntervalDays(__instance, __instance.GrowIntervalDays * ModSettings<Settings>.Current.SlowDownCoefficient);
        }
        
        [UsedImplicitly]
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Deposit), "set_Count")]
        // ReSharper disable InconsistentNaming
        private static void Deposit_set_Count_prf(Deposit __instance, ref float value)
        {
            if (ModSettings<Settings>.Current.SlowDownDeposits)
            {
                float origCount = __instance.Count;
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (value == origCount - 1f)
                {
                    value = origCount - 1f / ModSettings<Settings>.Current.SlowDownCoefficient;
                }
            }
        }
    }
}