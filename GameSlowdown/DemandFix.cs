using System;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;
using VoxelTycoon;
using VoxelTycoon.Cities;
using XMNUtils;

namespace GameSlowdown
{
    [HarmonyPatch]
    public static class DemandFix
    {
        private static Action<CityDemand, int> _setCityDemandCountAction;
        
        private static void SetCityDemandCount(CityDemand demand, int count)
        {
            if (_setCityDemandCountAction == null)
            {
                MethodInfo mInf = typeof(CityDemand).GetMethod("set_Count", BindingFlags.NonPublic | BindingFlags.Instance);
                _setCityDemandCountAction = (Action<CityDemand, int>)Delegate.CreateDelegate(typeof(Action<CityDemand, int>), mInf!);
            }

            _setCityDemandCountAction(demand, count);
        }
        
        [UsedImplicitly]
        [HarmonyPrefix]
        [HarmonyPatch(typeof(CityDemand), "InvalidateSupply")]
        // ReSharper disable InconsistentNaming
        private static bool CityDemand_InvalidateSupply_prf(CityDemand __instance, ref PeriodCounterInt ____supplyCounter, TimeSpanCounterFloat ____satisfactionCounter)
        {
            int daysInMonth = LazyManager<TimeManager>.Current.GetDaysInMonth();
            int currentDay = LazyManager<TimeManager>.Current.DateTime.Day;
            int dailyDemand = __instance.DailyDemand;
            float dailyError = dailyDemand - (float)__instance.Demand / daysInMonth;
            if (currentDay == 1 || Mathf.FloorToInt(dailyError * currentDay / dailyDemand) == Mathf.FloorToInt(dailyError * (currentDay - 1) / dailyDemand))
            {
                int required = Mathf.Min(dailyDemand, __instance.RemainingDemand);
                int available = Mathf.Min(Mathf.Min(required, __instance.Count), __instance.RemainingDemand);
                SetCityDemandCount(__instance, __instance.Count - available);
                ____supplyCounter.Add(available);
                ____satisfactionCounter.Set(0, (required > 0) ? Mathf.Clamp01((float)available / (float)Mathf.Max(required)) : 1f);
            }
            else
            {
                ____satisfactionCounter.Set(0, __instance.Count > 0 ? 1f : 0f);
            }
            return false;
        }
        
    }
}