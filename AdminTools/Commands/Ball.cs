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
    using PlayerRoles.FirstPersonControl;
    using UnityEngine;
    using YamlDotNet.Core.Tokens;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Ball : ICommand, IUsageProvider
    {
        public string Command { get; } = "ball";

        public string[] Aliases { get; } = Array.Empty<string>();

        public string Description { get; } = "Spawns a bouncy ball (SCP-018) on a user or all users";

        public string[] Usage { get; } = new string[] { "%player%", "[Speed = 5]","[IsMute = false]"};

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("at.ball"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count < 1)
            {
                response = "Usage: ball ((player id/ name) or (all / *)) [IsMute]";
                return false;
            }

            IEnumerable<Player> players = Player.GetProcessedData(arguments);

            if (!float.TryParse(arguments.ElementAtOrDefault(1), out float speed))
                speed = 5;

            if (!bool.TryParse(arguments.ElementAtOrDefault(2), out bool IsMute) || !IsMute)
                Cassie.Message("pitch_1.5 xmas_bouncyballs");

            foreach (Player p in players)
            {
                Scp018Projectile scp018 = Projectile.CreateAndSpawn(ProjectileType.Scp018, p.Position, p.Transform.rotation).As<Scp018Projectile>();
                scp018.Rigidbody.velocity = p.ReferenceHub.GetVelocity() + Random.onUnitSphere * speed;
            }

            response = $"Ball has been spawn for all the followed player:\n{Extensions.LogPlayers(players)}";
            return true;
        }
    }
}
