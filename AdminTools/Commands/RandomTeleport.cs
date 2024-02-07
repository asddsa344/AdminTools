using CommandSystem;
using Exiled.API.Extensions;
using Exiled.API.Features;
using RemoteAdmin;
using System;
using System.Linq;
using UnityEngine;

namespace AdminTools.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class RandomTeleport : ICommand
    {
        public string Command { get; } = "randomtp";

        public string[] Aliases { get; } = new string[] { };

        public string Description { get; } = "Randomly teleports a user or all users to a random room in the facility";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!CommandProcessor.CheckPermissions(((CommandSender)sender), "randomtp", PlayerPermissions.PlayersManagement, "AdminTools", false))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count != 1)
            {
                response = "Usage: randomtp ((player id / name) or (all / *))";
                return false;
            }

            switch (arguments.At(0))
            {
                case "*":
                case "all":
                    foreach (Player ply in Player.List)
                    {
                        Room randRoom = Room.List.GetRandomValue();
                        ply.Position = randRoom.Position + Vector3.up;
                    }

                    response = $"Everyone was teleported to a random room in the facility";
                    return true;
                default:
                    Player pl = Player.Get(arguments.At(0));
                    if (pl == null)
                    {
                        response = $"Player not found: {arguments.At(0)}";
                        return false;
                    }

                    Room rand = Room.List.GetRandomValue();
                    pl.Position = rand.Position + Vector3.up;

                    response = $"Player {pl.Nickname} was teleported to {rand.Name}";
                    return true;
            }
        }
    }
}
