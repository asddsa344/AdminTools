using CommandSystem;
using Exiled.API.Extensions;
using Exiled.API.Features;
using RemoteAdmin;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AdminTools;

namespace AdminTools.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class RandomTeleport : ICommand, IUsageProvider
    {
        public string Command { get; } = "randomtp";

        public string[] Aliases { get; } = Array.Empty<string>();

        public string Description { get; } = "Randomly teleports a user or all users to a random room in the facility";

        public string[] Usage { get; } = new string[] { "%player%", };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!CommandProcessor.CheckPermissions(((CommandSender)sender), "randomtp", PlayerPermissions.PlayersManagement, "AdminTools", false))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count != 1)
            {
                response = "Usage: randomtp ((player id / name) or (all / *))";
                return false;
            }

            IEnumerable<Player> players = Player.GetProcessedData(arguments);

            foreach (Player ply in players)
            {
                Room randRoom = Room.List.GetRandomValue();
                ply.Position = randRoom.Position + Vector3.up;
            }

            response = $"Followed player was teleported to a random room in the facility:\n{Extensions.LogPlayers(players)}";
            return true;
        }
    }
}
