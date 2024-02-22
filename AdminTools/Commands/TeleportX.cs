using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using System;

namespace AdminTools.Commands
{
    using Exiled.API.Extensions;
    using PlayerRoles;
    using System.Collections.Generic;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class TeleportX : ICommand, IUsageProvider
    {
        public string Command { get; } = "teleportx";

        public string[] Aliases { get; } = new string[] { "tpx", "tpto" };

        public string Description { get; } = "Teleports all users or a user to another user";

        public string[] Usage { get; } = new string[] { "%player%", "%player%", };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("at.tp"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count != 2)
            {
                response = "Usage: teleportx (People teleported: (player id / name) or (all / *)) (Teleported to: (player id / name) or (all / *))";
                return false;
            }

            Player ply = Player.GetProcessedData(arguments, 1).GetRandomValue();
            if (ply == null)
            {
                response = $"Player not found: {arguments.At(1)}";
                return false;
            }

            IEnumerable<Player> players = Player.GetProcessedData(arguments);
            if (players.IsEmpty())
            {
                response = $"Player not found: {arguments.At(0)}";
                return false;
            }

            foreach (Player plyr in players)
            {
                plyr.Position = ply.Position;
            }

            response = $"All the followed player has been teleported to {ply.Nickname}({ply.Id}):\n{Extensions.LogPlayers(players)}";
            return true;
        }
    }
}
