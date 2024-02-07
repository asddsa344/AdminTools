using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using System;

namespace AdminTools.Commands
{
    using System.Collections.Generic;
    using System.Linq;
    using Exiled.API.Enums;
    using Exiled.API.Features.Items;
    using Exiled.API.Features.Pickups;
    using Exiled.API.Features.Pickups.Projectiles;
    using PlayerRoles;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Ball : ICommand, IUsageProvider
    {
        public string Command { get; } = "ball";

        public string[] Aliases { get; } = new string[] { };

        public string Description { get; } = "Spawns a bouncy ball (SCP-018) on a user or all users";

        public string[] Usage { get; } = new string[] { "%player%" };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!((CommandSender)sender).CheckPermission("at.ball"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count != 1)
            {
                response = "Usage: ball ((player id/ name) or (all / *))";
                return false;
            }

            IEnumerable<Player> players = Player.GetProcessedData(arguments);

            response = players.Count() == 1
                ? $"{players.Single().Nickname} has received a bouncing ball!"
                : $"The balls are bouncing for {players.Count()} players!";

            Cassie.Message("pitch_1.5 xmas_bouncyballs");

            foreach (Player p in players)
                Projectile.CreateAndSpawn(ProjectileType.Scp018, p.Position, p.Transform.rotation);
            return true;
        }
    }
}
