using System;
using System.Collections.Generic;
using System.Text;

namespace VintageTweaks.Utilities
{
    //applies health-based scaling consistently
    internal class SpeedHealthUtil
    {
        public static float ApplyHealthScaling(
            float baseSpeed,
            float minSpeed,
            float health,
            float maxHealth
        )
        {
            if (VintageTweaksModSystem.Config == null)
                return baseSpeed;
            if (maxHealth <= 0) return baseSpeed;

            float percent = health / maxHealth;

            float configValue = VintageTweaksModSystem.Config.healthslow;

            float factor = configValue == 0 ? 1f : 1f / configValue;

            float result = baseSpeed * percent * factor;

            return Math.Clamp(result, minSpeed, baseSpeed);
        }
    }
}
