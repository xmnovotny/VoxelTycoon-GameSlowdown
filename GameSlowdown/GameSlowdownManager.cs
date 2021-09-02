using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using ModSettingsUtils;
using UnityEngine;
using VoxelTycoon;
using VoxelTycoon.Cities;
using VoxelTycoon.Deposits;
using VoxelTycoon.Researches;
using VoxelTycoon.Tracks;
using VoxelTycoon.UI;
using XMNUtils;

namespace GameSlowdown
{
    [HarmonyPatch]
    public class GameSlowdownManager: SimpleLazyManager<GameSlowdownManager>
    {
        private Action<City,float> _setCityGrowIntervalDays;
        private Dictionary<Research, float> _researchStates = new ();

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

        [UsedImplicitly]
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ResearchManager), "CompleteDay")]
        // ReSharper disable InconsistentNaming
        private static bool ResearchManager_CompleteDay_prf(ResearchManager __instance, Research research)
        {
            if (ModSettings<Settings>.Current.SlowDownResearch)
            {
                GameSlowdownManager current = Current;
                float progress = current._researchStates.AddFloatToDict(research, 1f / ModSettings<Settings>.Current.SlowDownCoefficient);
                if (progress >= 0.99f)
                {
                    current._researchStates[research] = 0f;
                    return true;
                }

                return false;
            }

            return true;
        }

        private static float _researchMultiplier = 0f;

        [UsedImplicitly]
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ResearchPickerWindowTreeItem), "Initialize")]
        // ReSharper disable InconsistentNaming
        private static void ResearchPickerWindowTreeItem_Initialize_prf()
        {
            if (ModSettings<Settings>.Current.SlowDownResearch && ModSettings<Settings>.Current.SlowDownCoefficient > 1f)
            {
                _researchMultiplier = ModSettings<Settings>.Current.SlowDownCoefficient;
            }
        }
        
        [UsedImplicitly]
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ResearchPickerWindowTreeItem), "Initialize")]
        // ReSharper disable InconsistentNaming
        private static void ResearchPickerWindowTreeItem_Initialize_pof()
        {
            _researchMultiplier = 0f;
        }

        [UsedImplicitly]
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ResourceView), "ShowPrice")]
        // ReSharper disable InconsistentNaming
        private static void ResourceView_ShowPrice_prf(ref double price)
        {
            if (_researchMultiplier > 0f)
                price *= _researchMultiplier;
        }

        [UsedImplicitly]
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ResourceView), "ShowDays")]
        // ReSharper disable InconsistentNaming
        private static void ResourceView_ShowDays_prf(ref float time)
        {
            if (_researchMultiplier > 0f)
                time *= _researchMultiplier;
        }

        [UsedImplicitly]
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ResourceView), "ShowItem", typeof(Item), typeof(int))]
        // ReSharper disable InconsistentNaming
        private static void ResourceView_ShowItem_prf(ref int count)
        {
            if (_researchMultiplier > 0f)
                count = Mathf.RoundToInt(count * _researchMultiplier);
        }
    }
}