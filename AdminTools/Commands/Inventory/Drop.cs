using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using System;
using System.Collections.Generic;

namespace AdminTools.Commands.Inventory
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Drop : ICommand, IUsageProvider
    {
        public string Command { get; } = "drop";

        public string[] Aliases { get; } = Array.Empty<string>();

        public string Description { get; } = "Drops the items in a players inventory";

        public string[] Usage { get; } = new string[] { "%player%", };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("at.inv"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count != 1)
            {
                response = "Usage: inventory drop ((player id / name) or (all / *))";
                return false;
            }

            IEnumerable<Player> players = Player.GetProcessedData(arguments);
            if (players.IsEmpty())
            {
                response = $"Player not found: {arguments.At(0)}";
                return false;
            }

            foreach (Player p in players)
                p.DropItems();

            response = $"All item has been dropped for all the followed players: \n{Extensions.LogPlayers(players)}";
            return true;
        }
    }
}