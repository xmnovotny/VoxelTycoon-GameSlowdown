using System;
using ModSettingsUtils;
using Newtonsoft.Json;
using UnityEngine;

namespace GameSlowdown
{
    [JsonObject(MemberSerialization.OptOut)]
    internal class Settings : ModSettings<Settings>
    {
        private float _slowDownCoefficient = 1f;
        private bool _slowDownDeposits = true;
        private bool _slowDownResearch = true;

        public float SlowDownCoefficient { 
            get => _slowDownCoefficient;
            set =>  SetProperty(Mathf.Round(Math.Max(Math.Min(value, 10f), 1f)), ref _slowDownCoefficient);
        }

        public bool SlowDownDeposits { 
            get => _slowDownDeposits;
            set =>  SetProperty(value, ref _slowDownDeposits);
        }

        public bool SlowDownResearch { 
            get => _slowDownResearch;
            set =>  SetProperty(value, ref _slowDownResearch);
        }
    }
}
