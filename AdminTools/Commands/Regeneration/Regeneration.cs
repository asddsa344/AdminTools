using CommandSystem;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using Exiled.Permissions.Extensions;
using InventorySystem.Items.Usables;
using NorthwoodLib.Pools;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace AdminTools.Commands.Regeneration
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Regeneration : ParentCommand
    {
        public Regeneration() => LoadGeneratedCommands();

        public override string Command { get; } = "reg";

        public override string[] Aliases { get; } = new string[] { };

        public override string Description { get; } = "Manages regeneration properties for users";

        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!((CommandSender)sender).CheckPermission("at.reg"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count < 1)
            {
                response = "Usage:\nreg ((player id / name) or (all / *)) (duration) (rate)" +
                    "\nreg clear" +
                    "\nreg list" +
                    "\nreg health (value)" +
                "\nreg time (value)";
                return false;
            }

            if (float.TryParse(arguments.At(1), out float duration))
            {
                response = "Usage:\nreg ((player id / name) or (all / *)) (duration) (rate)" +
                    "\nreg clear" +
                    "\nreg list" +
                    "\nreg health (value)" +
                "\nreg time (value)";
                return false;
            }
            if (float.TryParse(arguments.At(2), out float rate))
            {
                response = "Usage:\nreg ((player id / name) or (all / *)) (duration) (rate)" +
                    "\nreg clear" +
                    "\nreg list" +
                    "\nreg health (value)" +
                "\nreg time (value)";
                return false;
            }
            IEnumerable<Player> players = Player.GetProcessedData(arguments);

            response = string.Empty;
            AnimationCurve animationCurve = AnimationCurve.Constant(0f, duration, rate);
            RegenerationProcess reg = new(animationCurve, 1f, 1f);
            foreach (Player player in players)
                UsableItemsController.GetHandler(player.ReferenceHub).ActiveRegenerations.Add(reg);
            return true;
        }
    }
}