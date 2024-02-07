using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using System;

namespace AdminTools.Commands
{
    using PlayerRoles;
    using System.Collections.Generic;
    using UnityEngine;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Size : ICommand
    {
        public string Command { get; } = "size";

        public string[] Aliases { get; } = new string[] { };

        public string Description { get; } = "Sets the size of all users or a user";

        public string[] Usage { get; } = new string[] { "%player%", "x", "y", "z" };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!((CommandSender)sender).CheckPermission("at.size"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count < 1)
            {
                response = "Usage:\nsize (player id / name) or (all / *)) (x value) (y value) (z value)" +
                    "\nsize reset";
                return false;
            }

            IEnumerable<Player> players = Player.GetProcessedData(arguments);

            if (arguments.Count != 4)
            {
                response = "Usage: size (all / *) (x) (y) (z)";
                return false;
            }

            if (!float.TryParse(arguments.At(1), out float x))
            {
                response = $"Invalid value for x size: {arguments.At(1)}";
                return false;
            }

            if (!float.TryParse(arguments.At(2), out float y))
            {
                response = $"Invalid value for y size: {arguments.At(2)}";
                return false;
            }

            if (!float.TryParse(arguments.At(3), out float z))
            {
                response = $"Invalid value for z size: {arguments.At(3)}";
                return false;
            }

            foreach (Player ply in players)
            {
                ply.Scale = new Vector3(x, y, z);
            }

            response = $"Everyone's scale has been set to {x} {y} {z}";
            return true;

        }
    }
}
