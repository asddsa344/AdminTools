using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using MEC;
using System;

namespace AdminTools.Commands
{
    using Exiled.API.Enums;
    using Exiled.API.Features.Pickups.Projectiles;
    using PlayerRoles;
    using System.Collections.Generic;
    using UnityEngine;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Rocket : ICommand, IUsageProvider
    {
        public string Command { get; } = "rocket";

        public string[] Aliases { get; } = Array.Empty<string>();

        public string Description { get; } = "Sends players high in the sky and explodes them";

        public string[] Usage { get; } = new string[] { "%player%", "speed" };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("at.rocket"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count < 2)
            {
                response = "Usage: rocket ((player id / name) or (all / *)) (speed)";
                return false;
            }

            IEnumerable<Player> players = Player.GetProcessedData(arguments);

            if (!float.TryParse(arguments.At(1), out float speed) && speed <= 0)
            {
                response = $"Speed argument invalid: {arguments.At(1)}";
                return false;
            }

            foreach (Player ply in players)
                Timing.RunCoroutine(DoRocket(ply, speed));

            response = $"All the followed player has been rocketed into the sky\n(We're going on a trip, in our favorite rocketship)\n{Extensions.LogPlayers(players)}";
            return true;
        }
        public static IEnumerator<float> DoRocket(Player player, float speed)
        {
            const int maxAmnt = 50;
            int amnt = 0;
            while (player.IsAlive)
            {
                player.Position += Vector3.up * speed;
                amnt++;
                if (amnt >= maxAmnt)
                {
                    player.IsGodModeEnabled = false;
                    if (Projectile.CreateAndSpawn(ProjectileType.FragGrenade, player.Position, player.Rotation).Is(out TimeGrenadeProjectile timeGrenadeProjectile))
                        timeGrenadeProjectile.Explode();
                    player.Kill("Went on a trip in their favorite rocket ship.");
                }

                yield return Timing.WaitForOneFrame;
            }
        }
    }
}
