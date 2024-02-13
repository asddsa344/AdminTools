using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using System;

namespace AdminTools.Commands
{
    using Exiled.API.Enums;
    using Exiled.API.Features.Items;
    using Exiled.API.Features.Pickups.Projectiles;
    using PlayerRoles;
    using System.Collections.Generic;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Explode : ICommand, IUsageProvider
    {
        public string Command { get; } = "expl";

        public string[] Aliases { get; } = new string[] { "boom" };

        public string Description { get; } = "Explodes a specified user or everyone instantly";

        public string[] Usage { get; } = new string[] { "%player%", };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (sender.CheckPermission("at.explode"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count != 1)
            {
                response = "Usage: expl ((player id / name) or (all / *))";
                return false;
            }

            IEnumerable<Player> players = Player.GetProcessedData(arguments);

            if (arguments.Count != 1)
            {
                response = "Usage: expl (all / *)";
                return false;
            }

            foreach (Player ply in players)
            {
                if (ply.IsDead)
                    continue;

                ply.Kill("Exploded by admin.");
                Projectile.CreateAndSpawn(ProjectileType.FragGrenade, ply.Position, ply.Rotation);
            }
            response = "Everyone exploded, Hubert cannot believe you have done this";
            return true;
        }
    }
}
