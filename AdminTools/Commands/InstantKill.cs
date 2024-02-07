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

namespace AdminTools.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class InstantKill : ICommand
    {
        public string Command { get; } = "instakill";

        public string[] Aliases { get; } = new string[] { "ik" };

        public string Description { get; } = "Manage instant kill properties for users";
        
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!((CommandSender)sender).CheckPermission("at.ik"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count < 1)
            {
                response = "Usage:\ninstakill ((player id / name) or (all / *))" +
                    "\ninstakill clear" +
                    "\ninstakill list" +
                    "\ninstakill remove (player id / name)";
                return false;
            }
            
            IEnumerable<Player> players;

            switch (arguments.At(0))
            {
                case "clear":
                    if (arguments.Count != 1)
                    {
                        response = "Usage: instakill clear";
                        return false;
                    }

                    Main.IK.Clear();

                    response = "Instant killing has been removed from everyone";
                    return true;
                case "list":
                    if (arguments.Count != 1)
                    {
                        response = "Usage: instakill clear";
                        return false;
                    }

                    StringBuilder playerLister = StringBuilderPool.Shared.Rent(Main.IK.Count != 0 ? "Players with instant killing on:\n" : "No players currently online have instant killing on");
                    if (Main.IK.Count == 0)
                    {
                        response = playerLister.ToString();
                        return true;
                    }

                    foreach (Player ply in Main.IK)
                    {
                        playerLister.Append(ply.Nickname);
                        playerLister.Append(", ");
                    }

                    string msg = playerLister.ToString().Substring(0, playerLister.ToString().Length - 2);
                    StringBuilderPool.Shared.Return(playerLister);
                    response = msg;
                    return true;
                case "remove":
                    if (arguments.Count != 2)
                    {
                        response = "Usage: instakill remove (player id / name)";
                        return false;
                    }

                    players = Player.GetProcessedData(arguments, 1);
                    if (players.Count() is 0)
                    {
                        response = $"Player not found: {arguments.At(1)}";
                        return false;
                    }

                    response = "";
                    foreach (Player ply in players)
                    {
                        if (Main.IK.Remove(ply))
                            response += $"Instant killing is off for {ply.Nickname} now";
                        else
                            response += $"Player {ply.Nickname} does not have the ability to instantly kill others";

                    }

                    return true;
                default:
                    if (arguments.Count != 1)
                    {
                        response = "Usage: instakill (player id / name)";
                        return false;
                    }

                    players = Player.GetProcessedData(arguments);
                    if (players.Count() is 0)
                    {
                        response = $"Player not found: {arguments.At(0)}";
                        return false;
                    }

                    response = "";
                    return true;
            }
        }
        public static void RunWhenPlayerIsHurt(HurtingEventArgs ev)
        {
            if (ev.Attacker != ev.Player && Main.IK.Contains(ev.Attacker))
                ev.Amount = StandardDamageHandler.KillValue;
        }

    }
}
