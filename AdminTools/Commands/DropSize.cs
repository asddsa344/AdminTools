using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using Mirror;
using System;
using UnityEngine;

namespace AdminTools.Commands
{
    using Exiled.API.Features.Items;
    using Exiled.API.Features.Pickups;
    using PlayerRoles;
    using System.Collections.Generic;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class DropSize : ICommand
    {
        public string Command { get; } = "dropsize";

        public string[] Aliases { get; } = new string[] { "drops" };

        public string Description { get; } = "Drops a selected amount of a selected item on a user or all users";

        public void LoadGeneratedCommands() { }

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!((CommandSender)sender).CheckPermission("at.items"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count < 3)
            {
                response = "Usage:\ndropsize ((player id / name) or (all / *)) (ItemType) (size)\ndropsize ((player id / name) or (all / *)) (ItemType) (x size) (y size) (z size)";
                return false;
            }

            IEnumerable<Player> players = Player.GetProcessedData(arguments);

            if (!Enum.TryParse(arguments.At(1), true, out ItemType type))
            {
                response = $"Invalid value for item name: {arguments.At(1)}";
                return false;
            }

            switch (arguments.Count)
            {
                case 3:
                    if (!float.TryParse(arguments.At(2), out float size))
                    {
                        response = $"Invalid value for item scale: {arguments.At(2)}";
                        return false;
                    }
                    SpawnItem(players, type, size, out string msg);
                    response = msg;
                    return true;
                case 5:
                    if (!float.TryParse(arguments.At(2), out float xval))
                    {
                        response = $"Invalid value for item scale: {arguments.At(2)}";
                        return false;
                    }

                    if (!float.TryParse(arguments.At(3), out float yval))
                    {
                        response = $"Invalid value for item scale: {arguments.At(3)}";
                        return false;
                    }

                    if (!float.TryParse(arguments.At(4), out float zval))
                    {
                        response = $"Invalid value for item scale: {arguments.At(4)}";
                        return false;
                    }
                    SpawnItem(players, type, xval, yval, zval, out string message);
                    response = message;
                    return true;
                default:
                    response = "\nUsage:\ndrops (all / *) (ItemType) (size) \ndrops (all / *) (ItemType) (x size) (y size) (z size)";
                    return false;
            }
        }

        private void SpawnItem(IEnumerable<Player> players, ItemType type, float size, out string message)
        {
            foreach (Player ply in players)
            {
                if (ply.IsDead)
                    continue;

                Pickup.CreateAndSpawn(type, ply.Position, default, ply).Scale = Vector3.one * size;
            }
            message = $"Spawned in a {type.ToString()} that is a size of {size} at every player's position (\"Yay! Items with sizes!\" - Galaxy119)";
        }

        private void SpawnItem(IEnumerable<Player> players, ItemType type, float x, float y, float z, out string message)
        {
            foreach (Player ply in players)
            {
                if (ply.IsDead)
                    continue;

                Pickup.CreateAndSpawn(type, ply.Position, default, ply).Scale = new(x, y, z);
            }
            message = $"Spawned in a {type.ToString()} that is {x}x{y}x{z} at every player's position (\"Yay! Items with sizes!\" - Galaxy119)";
        }
    }
}
