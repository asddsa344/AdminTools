using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using System;

namespace AdminTools.Commands.Grenade
{
    using Exiled.API.Enums;
    using Exiled.API.Features.Items;
    using Exiled.API.Extensions;
    using InventorySystem.Items.ThrowableProjectiles;
    using PlayerRoles;
    using System.Collections.Generic;
    using System.Linq;
    using Exiled.API.Features.Pickups.Projectiles;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Grenade : ICommand
    {
        public string Command { get; } = "grenade";

        public string[] Aliases { get; } = new string[] { "gn" };

        public string Description { get; } = "Spawns a frag/flash/scp018 grenade on a user or users";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!((CommandSender)sender).CheckPermission("at.grenade"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count < 2 || arguments.Count > 3)
            {
                response = "Usage: grenade ((player id / name) or (all / *)) (ProjectileType) (grenade time)";
                return false;
            }

            if (!Enum.TryParse(arguments.At(1), true, out ProjectileType type))
            {
                response = $"Invalid value for projectile type: {arguments.At(1)}\n{string.Join(", ",Enum.GetNames(typeof(ProjectileType)))}.";
                return false;
            }

            if (!float.TryParse(arguments.At(2), out float fuseTime))
            {
                response = $"Invalid fuse time for grenade: {arguments.At(2)}";
                return false;
            }

            IEnumerable<Player> players = Player.GetProcessedData(arguments);

            foreach (Player player in players)
            {
                if (player.IsDead)
                    continue;
                if (Projectile.CreateAndSpawn(type, player.Position, player.Rotation).Is(out TimeGrenadeProjectile timeGrenadeProjectile))
                    timeGrenadeProjectile.FuseTime = fuseTime;
            }

            response = $"{type} has been sent to {string.Join(" ,", players.Select(x => $"({x.Id}){x.Nickname}"))}";
            return true;
        }
    }
}
