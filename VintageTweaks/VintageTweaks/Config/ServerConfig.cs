namespace VintageTweaks.Config
{
    public class ServerConfig
    {
        // multiplier strength for health-based slowdown
        // 1 == normal behavior
        // >1 makes slower
        // 0 == disabled
        public float healthslow { get; set; } = 1f;
    }
}