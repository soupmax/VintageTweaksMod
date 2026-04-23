using HarmonyLib;
using System;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.GameContent;
using VintageTweaks.Extensions;
using static VintageTweaks.Extensions.ExtensionAiTaskFleeEntity;

namespace VintageTweaks.Patches
{
    [HarmonyPatch(typeof(AiTaskFleeEntity))]
    public class FleeTaskPatch
    {
        //called after constructor
        [HarmonyPostfix]
        [HarmonyPatch(MethodType.Constructor,
            new[] { typeof(EntityAgent), typeof(JsonObject), typeof(JsonObject) })]
        public static void Init(AiTaskFleeEntity __instance, JsonObject taskConfig)
        {
            var data = ExtensionsAiTaskFleeEntity.Get(__instance);

            data.baseSpeed = taskConfig["movespeed"].AsFloat(0.02f);
            data.minSpeed = taskConfig["minmovespeed"].AsFloat(data.baseSpeed * 0.5f);
            data.useHealthScaling = taskConfig["healthslow"].AsBool(true);

            __instance.entity.World.Logger.Event(
                $"[vt]init {__instance.entity.Code} | base={data.baseSpeed} min={data.minSpeed} enabled={data.useHealthScaling}"
            );
        }

        //called when fleeing starts
        [HarmonyPrefix]
        [HarmonyPatch(nameof(AiTaskFleeEntity.StartExecute))]
        public static void Apply(AiTaskFleeEntity __instance)
        {
            var data = ExtensionsAiTaskFleeEntity.Get(__instance);

            if (!data.useHealthScaling) return;

            var health = __instance.entity.GetBehavior<EntityBehaviorHealth>();
            if (health == null || health.MaxHealth <= 0) return;

            float percent = health.Health / health.MaxHealth;

            float config = GetConfig(__instance);
            float factor = config == 0 ? 1f : 1f / config;

            float speed = ComputeSpeed(data, percent, factor);

            SetMoveSpeed(__instance, speed);

            Log(__instance, percent, speed);
        }

        //compute final speed
        private static float ComputeSpeed(ExtensionsAiTaskFleeEntity data, float hp, float factor)
        {
            float result = data.baseSpeed * hp * factor;
            return Math.Clamp(result, data.minSpeed, data.baseSpeed);
        }

        //read config safely
        private static float GetConfig(AiTaskFleeEntity task)
        {
            return task.world.Api.ModLoader
                .GetModSystem<VintageTweaksModSystem>()
                .Config.healthslow;
        }

        //set private movespeed field
        private static void SetMoveSpeed(AiTaskFleeEntity task, float value)
        {
            var field = Traverse.Create(task).Field("moveSpeed");
            field.SetValue(value);
        }

        //debug output
        private static void Log(AiTaskFleeEntity task, float hp, float speed)
        {
            task.entity.World.Logger.Event(
                $"[vt]flee hp={hp:0.00} speed={speed:0.00}"
            );

            task.entity.DebugAttributes.SetFloat("vt-flee-speed", speed);
        }
    }
}