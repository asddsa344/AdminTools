using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using System;
using System.Collections.Generic;

namespace AdminTools.Commands.InstantKill
{
    public class Add : ICommand, IUsageProvider
    {
        public string Command { get; } = "add";

        public string[] Aliases { get; } = Array.Empty<string>();

        public string Description { get; } = "add instantkill to this player";

        public string[] Usage { get; } = new string[] { "%player%", };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("at.instakill"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count != 1)
            {
                response = "Usage: instakill add ((player id / name) or (all / *))";
                return false;
            }

            IEnumerable<Player> players = Player.GetProcessedData(arguments);
            if (players.IsEmpty())
            {
                response = $"Player not found: {arguments.At(0)}";
                return false;
            }

            Plugin.InstantKillPlayerList.AddRange(players);

            response = $"All the followed player have been added to InstantKill:\n{Extensions.LogPlayers(players)}";
            return true;
        }
    }
}
