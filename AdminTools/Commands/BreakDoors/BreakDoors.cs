using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using NorthwoodLib.Pools;
using System;
using System.Text;

namespace AdminTools.Commands.BreakDoors
{
    using System.Collections.Generic;
    using System.Linq;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class BreakDoors : ParentCommand
    {
        public BreakDoors() => LoadGeneratedCommands();

        public override string Command { get; } = "breakdoors";

        public override string[] Aliases { get; } = new string[] { "bd" };

        public override string Description { get; } = "Manage breaking door/gate properties for players";

        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!((CommandSender)sender).CheckPermission("at.bd"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            IEnumerable<Player> players = Player.GetProcessedData(arguments);

            foreach (Player player in players)
                if (EventHandlers.BreakDoorsList.Contains(player))
                    EventHandlers.BreakDoorsList.Remove(player);
                else
                    EventHandlers.BreakDoorsList.Add(player);

            response = $"{players.Count()} players have been updated. (Players with BD were removed, those without it were added)";
            return true;
        }
    }
}
