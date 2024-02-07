using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using MEC;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdminTools.Commands.Jail
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Jail : ICommand
    {
        public string Command { get; } = "jail";

        public string[] Aliases { get; } = new string[] { };

        public string Description { get; } = "Jails or unjails a user";
        
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!((CommandSender)sender).CheckPermission("at.jail"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count != 1)
            {
                response = "Usage: jail (player id / name) [true/false]";
                return false;
            }

            IEnumerable<Player> players = Player.GetProcessedData(arguments);
            if (players.IsEmpty())
            {
                response = $"Player not found: {arguments.At(0)}";
                return false;
            }
            response = string.Empty;
            foreach (Player ply in players)
            {
                if (Main.JailedPlayers.Any(j => j.Userid == ply.UserId))
                {
                    EventHandlers.DoUnJail(ply);
                    response += $"Player {ply.Nickname} has been unjailed now\n";
                }
                else
                {
                    EventHandlers.DoJail(ply);
                    response += $"Player {ply.Nickname} has been jailed now\n";
                }
            }
            return true;
        }
    }
}
