using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using System;

namespace AdminTools.Commands
{
    using CustomPlayerEffects;
    using Exiled.API.Extensions;
    using PlayerRoles;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class FakeSpawnCommand : ICommand, IUsageProvider
    {
        public string Command { get; } = "fakesync";

        public string[] Aliases { get; } = Array.Empty<string>();

        public string Description { get; } = "Sets everyone or a user to be invisible";

        public string[] Usage { get; } = new string[] { "%player% / Clear", };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (sender.CheckPermission("at.ghost"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (!Enum.TryParse(arguments.At(1), true, out RoleTypeId roletype))
            {
                response = "Usage:\nghost ((player id / name) or (all / *))" +
                    "\nghost clear";
                return false;
            }

            switch (arguments.At(0))
            {
                case "clear":
                    foreach (Player pl in Player.List)
                        pl.ChangeAppearance(roletype, Player.List, true, 0);

                    response = "Everyone is no longer invisible";
                    return true;
                case "*":
                case "all":
                    foreach (Player pl in Player.List)
                        pl.EnableEffect<Invisible>();

                    response = "Everyone is now invisible";
                    return true;
                default:
                    Player ply = Player.Get(arguments.At(0));
                    if (ply == null)
                    {
                        response = $"Player not found: {arguments.At(0)}";
                        return false;
                    }

                    if (!ply.IsEffectActive<Invisible>())
                    {
                        ply.EnableEffect<Invisible>();
                        response = $"Player {ply.Nickname} is now invisible";
                    }
                    else
                    {
                        ply.DisableEffect<Invisible>();
                        response = $"Player {ply.Nickname} is no longer invisible";
                    }
                    return true;
            }
        }
    }
}
