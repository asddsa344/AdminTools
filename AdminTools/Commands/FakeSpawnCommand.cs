using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using System;

namespace AdminTools.Commands
{
    using CustomPlayerEffects;
    using Exiled.API.Extensions;
    using Exiled.API.Features.Roles;
    using PlayerRoles;
    using System.Collections.Generic;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class FakeSpawnCommand : ICommand, IUsageProvider
    {
        public string Command { get; } = "fakesync";

        public string[] Aliases { get; } = Array.Empty<string>();

        public string Description { get; } = "Sets everyone or a user to be invisible";

        public string[] Usage { get; } = new string[] { "%player%", "%role%" };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("at.ghost"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (!Enum.TryParse(arguments.At(1), true, out RoleTypeId roletype))
            {
                response = "Usage:\nfakesync ((player id / name) or (all / *))";
                return false;
            }

            IEnumerable<Player> players = Player.GetProcessedData(arguments);
            foreach (Player player in players)
                player.ChangeAppearance(roletype);

            response = $"The followed player have been change to {roletype}:\n{Extensions.LogPlayers(players)}";
            return false;
        }
    }
}
