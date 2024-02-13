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
    using YamlDotNet.Core.Tokens;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Ball : ICommand, IUsageProvider
    {
        public string Command { get; } = "ball";

        public string[] Aliases { get; } = Array.Empty<string>();

        public string Description { get; } = "Spawns a bouncy ball (SCP-018) on a user or all users";

        public string[] Usage { get; } = new string[] { "%player%", "[IsMute]"};

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (sender.CheckPermission("at.ball"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count != 1)
            {
                response = "Usage: ball ((player id/ name) or (all / *)) [IsMute]";
                return false;
            }

            IEnumerable<Player> players = Player.GetProcessedData(arguments);

            if (!bool.TryParse(arguments.ElementAtOrDefault(1), out bool IsMute) || !IsMute)
                Cassie.Message("pitch_1.5 xmas_bouncyballs");

            foreach (Player p in players)
                Projectile.CreateAndSpawn(ProjectileType.Scp018, p.Position, p.Transform.rotation);

            response = $"Ball has been spawn for all the followed player:{Extensions.Fuckyou(players)}";
            return true;
        }
    }
}
