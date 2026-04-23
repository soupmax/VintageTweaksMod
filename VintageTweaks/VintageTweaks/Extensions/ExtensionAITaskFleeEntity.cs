using System.Runtime.CompilerServices;
using Vintagestory.GameContent;

namespace VintageTweaks.Extensions
{
    //stores extra data per AiTaskFleeEntity instance
    internal class ExtensionAiTaskFleeEntity
    {
        internal class ExtensionsAiTaskFleeEntity
        {
            private static readonly ConditionalWeakTable<AiTaskFleeEntity, ExtensionsAiTaskFleeEntity> map = new();

            public float baseSpeed;
            public float minSpeed;
            public bool useHealthScaling;

            public static ExtensionsAiTaskFleeEntity Get(AiTaskFleeEntity task)
            {
                return map.GetValue(task, _ => new ExtensionsAiTaskFleeEntity());
            }
        }
    }
}