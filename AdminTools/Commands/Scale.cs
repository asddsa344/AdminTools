using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace AdminTools.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Scale : ICommand, IUsageProvider
    {
        public string Command { get; } = "scale";

        public string[] Aliases { get; } = Array.Empty<string>();

        public string Description { get; } = "Scales all users or a user by a specified value";

        public string[] Usage { get; } = new string[] { "%player%", "size" };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("at.size"))
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

            if (arguments.Count != 2)
            {
                response = "Usage: scale (all / *) (value)";
                return false;
            }

            IEnumerable<Player> players = Player.GetProcessedData(arguments);

            if (!float.TryParse(arguments.At(1), out float scale))
            {
                response = $"Invalid value for scale: {arguments.At(1)}";
                return false;
            }

            foreach (Player ply in players)
                ply.Scale = Vector3.one * scale;

            response = $"Scale has been set to {scale} for the followed player:\n{Extensions.LogPlayers(players)}";
            return true;
        }
    }
}
