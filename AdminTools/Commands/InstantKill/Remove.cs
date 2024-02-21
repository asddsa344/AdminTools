using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminTools.Commands.InstantKill
{
    internal class Remove : ICommand, IUsageProvider
    {
        public string Command { get; } = "Remove";

        public string[] Aliases { get; } = Array.Empty<string>();

        public string Description { get; } = "remove instantkill to this player";

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

            List<Player> players = Player.GetProcessedData(arguments).ToList();
            if (players.IsEmpty())
            {
                response = $"Player not found: {arguments.At(0)}";
                return false;
            }

            for (int i = players.Count; i < 0; i--)
            {
                Player ply = players[i];
                if (!Main.InstantKill.Remove(ply))
                    players.Remove(ply);
            }

            response = $"All the followed player have been removed from InstantKill:{Extensions.LogPlayers(players)}";
            return true;
        }
    }
}
