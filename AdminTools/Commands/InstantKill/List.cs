using CommandSystem;
using Exiled.API.Features.Pools;
using Exiled.Permissions.Extensions;
using System;
using System.Linq;
using System.Text;

namespace AdminTools.Commands.InstantKill
{
    public class List : ICommand
    {
        public string Command { get; } = "List";

        public string[] Aliases { get; } = Array.Empty<string>();

        public string Description { get; } = "Get instantkill list player";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("at.instakill"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            StringBuilder playerLister = StringBuilderPool.Pool.Get();

            playerLister.Append(Main.InstantKill.Any() ? "Players with instant killing on:\n" : "No players currently online have instant killing on");
            playerLister.Append(Extensions.LogPlayers(Main.InstantKill));

            response = StringBuilderPool.Pool.ToStringReturn(playerLister);
            return true;
        }
    }
}
