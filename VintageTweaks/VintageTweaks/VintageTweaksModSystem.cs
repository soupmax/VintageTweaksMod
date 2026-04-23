using HarmonyLib;
using System;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Server;
using VintageTweaks.Config;

namespace VintageTweaks
{
    public class VintageTweaksModSystem : ModSystem
    {

        private Harmony harmony;

        public ServerConfig config { get; private set; }

        public override void Start(ICoreAPI api)
        {
            if (!Harmony.HasAnyPatches(Mod.Info.ModID))
            {
                harmony = new Harmony(Mod.Info.ModID);
                harmony.PatchAll();
            }
        }

        public override void StartServerSide(ICoreServerAPI api)
        {
            var harmony = new Harmony(Mod.Info.ModID);

            try
            {
                config = api.LoadModConfig<ServerConfig>($"{Mod.Info.ModID}-server.json") ?? new ServerConfig();
                api.StoreModConfig(config, $"{Mod.Info.ModID}-server.json");
            }
            catch (Exception e)
            {
                Mod.Logger.Error("Error while loading server config. Using default values.");
                Mod.Logger.Error(e);
                config = new ServerConfig();
            }

            api.ChatCommands.Create("sethealthslow")
                .WithArgs(api.ChatCommands.Parsers.Float("value"))
                .WithDescription("Sets the health slowdown multiplier. 0 turns it off.")
                .WithExamples("/sethealthslow 0.5")
                .RequiresPrivilege(Privilege.controlserver)
                .HandleWith(args =>
                {
                    if (args.ArgCount == 0 || args[0] == null)
                        return TextCommandResult.Error("no value provided");

                    string raw = args[0]?.ToString();

                    //is it a number?????
                    if (!float.TryParse(raw, out float value))
                    {
                        return TextCommandResult.Error("That's not a number.");
                    }

                    if (value < 0f || value > 5f)
                        return TextCommandResult.Error("The value must be between 0 and 5");

                    config.healthslow = value;

                    api.StoreModConfig(config, "vintagetweaks-server.json");

                    return TextCommandResult.Success($"Set value to {value}. Restart Server for this change to take effect.");
                }
            );


            api.ChatCommands.Create("gethealthslow")
                .WithDescription("Shows the health slowdown multiplier.")
                .RequiresPrivilege(Privilege.chat)
                .HandleWith(args =>
                {
                    return TextCommandResult.Success(config.healthslow.ToString());
                }
            );
        }

    }
}
