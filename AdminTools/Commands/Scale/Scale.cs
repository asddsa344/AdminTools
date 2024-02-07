using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace AdminTools.Commands.Scale
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Scale : ParentCommand
    {
        public Scale() => LoadGeneratedCommands();

        public override string Command { get; } = "scale";

        public override string[] Aliases { get; } = new string[] { };

        public override string Description { get; } = "Scales all users or a user by a specified value";

        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!((CommandSender)sender).CheckPermission("at.size"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count < 1)
            {
                response = "Usage:\nscale ((player id / name) or (all / *)) (value)" +
                    "\nscale reset";
                return false;
            }

            switch (arguments.At(0))
            {
                case "reset":
                    if (arguments.Count != 1)
                    {
                        response = "Usage: scale reset";
                        return false;
                    }
                    foreach (Player plyr in Player.List)
                        SetPlayerScale(plyr, 1);

                    response = $"Everyone's scale has been reset";
                    return true;
                default:
                    if (arguments.Count != 2)
                    {
                        response = "Usage: scale (all / *) (value)";
                        return false;
                    }
                    IEnumerable<Player> players = Player.GetProcessedData(arguments);

                    if (!float.TryParse(arguments.At(1), out float value))
                    {
                        response = $"Invalid value for scale: {arguments.At(1)}";
                        return false;
                    }

                    foreach (Player ply in players)
                        SetPlayerScale(ply, value);

                    response = $"Everyone's scale has been set to {value}";
                    return true;
            }
            void SetPlayerScale(Player target, float scale) => target.Scale = Vector3.one * scale;
        }
    }
}
