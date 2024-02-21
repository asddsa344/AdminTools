using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using NorthwoodLib.Pools;
using System;
using System.Linq;
using System.Text;

namespace AdminTools.Commands
{
    using Exiled.API.Features.Roles;
    using System.Collections.Generic;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class TargetGhost : ICommand
    {
        public const string HelpStr = "Usage: targetghost (player id / name) (player id / name) ...";

        public string Command { get; } = "targetghost";

        public string[] Aliases { get; } = new string[] { "tg" };

        public string Description { get; } = "Sets a user to be invisible to another user";

        public string[] Usage { get; } = new string[] { "%player%", "%player%", };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("at.targetghost"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count < 2)
            {
                response = HelpStr;
                return false;
            }

            IEnumerable<Player> sourcePlayers = Player.GetProcessedData(arguments);

            if (sourcePlayers.IsEmpty())
            {
                response = $"Invalid source player: {arguments.At(0)}";
                return false;
            }

            IEnumerable<Player> players = Player.GetProcessedData(arguments, 1);
            if (players.IsEmpty())
            {
                response = $"Player not found: {arguments.At(1)}";
                return false;
            }

            foreach (Player sourcePlayer in sourcePlayers)
            {
                foreach (Player victim in players)
                {
                    if (sourcePlayer.Role.Is(out FpcRole role))
                    {
                        if (!role.IsInvisibleFor.Add(victim))
                            role.IsInvisibleFor.Remove(victim);
                    }
                }
            }

            response = $"Done modifying who can see";
            return true;
        }
    }
}