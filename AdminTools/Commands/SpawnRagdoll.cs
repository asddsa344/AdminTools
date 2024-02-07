using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using MEC;
using System;

namespace AdminTools.Commands
{
    using System.Collections.Generic;
    using Mirror;
    using PlayerRoles;
    using PlayerStatsSystem;
    using UnityEngine;
    using Ragdoll = Exiled.API.Features.Ragdoll;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class SpawnRagdoll : ICommand
    {
        public string Command { get; } = "spawnragdoll";

        public string[] Aliases { get; } = new string[] { "ragdoll", "rd", "rag", "doll" };

        public string Description { get; } = "Spawns a specified number of ragdolls on a user";
        
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!((CommandSender)sender).CheckPermission("at.dolls"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count != 3)
            {
                response = "Usage: spawnragdoll ((player id / name) or (all / *)) (RoleTypeId) (amount)";
                return false;
            }

            if (!Enum.TryParse(arguments.At(1), true, out RoleTypeId type))
            {
                response = $"Invalid RoleTypeId for ragdoll: {arguments.At(1)}";
                return false;
            }

            if (!int.TryParse(arguments.At(2), out int amount))
            {
                response = $"Invalid amount of ragdolls to spawn: {arguments.At(2)}";
                return false;
            }

            IEnumerable<Player> players = Player.GetProcessedData(arguments);

            foreach (Player player in players)
            {
                Timing.RunCoroutine(SpawnDolls(player, type, amount));
            }


            response = $"{amount} {type} ragdoll(s) have been spawned on {arguments.At(0)}.";
            return true;
        }

        private IEnumerator<float> SpawnDolls(Player player, RoleTypeId type, int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                if (player.IsAlive)
                    Ragdoll.CreateAndSpawn(type, "SCP-343", "End of the Universe", player.Position, default);
                yield return Timing.WaitForSeconds(0.5f);
            }
        }
    }
}
