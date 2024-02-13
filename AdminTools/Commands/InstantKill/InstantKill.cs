using AdminTools.Commands.Inventory;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Permissions.Extensions;
using NorthwoodLib.Pools;
using PlayerStatsSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdminTools.Commands.InstantKill
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class InstantKill : ParentCommand, IUsageProvider
    {
        public override string Command { get; } = "instakill";

        public override string[] Aliases { get; } = new string[] { "ik" };

        public override string Description { get; } = "Manage instant kill properties for users";

        public string[] Usage { get; } = new string[] { "Add or Remove or List", };

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new Add());
            RegisterCommand(new Remove());
            RegisterCommand(new List());
        }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (sender.CheckPermission("at.ik"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            response = "Usage:\ninstakill add ((player id / name) or (all / *))" +
                "\ninstakill clear" +
                "\ninstakill list" +
                "\ninstakill remove (player id / name) or (all / *))";
            return false;

        }
    }
}
