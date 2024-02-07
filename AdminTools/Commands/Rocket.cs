using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using MEC;
using System;

namespace AdminTools.Commands
{
    using PlayerRoles;
    using System.Collections.Generic;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Rocket : ICommand, IUsageProvider
    {
        public string Command { get; } = "rocket";

        public string[] Aliases { get; } = new string[] { };

        public string Description { get; } = "Sends players high in the sky and explodes them";

        public string[] Usage { get; } = new string[] { "%player%", "speed" };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!((CommandSender)sender).CheckPermission("at.rocket"))
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
                Timing.RunCoroutine(EventHandlers.DoRocket(ply, speed));

            response = "Everyone has been rocketed into the sky (We're going on a trip, in our favorite rocketship)";
            return true;
        }
    }
}
