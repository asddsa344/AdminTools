using System;
using CommandSystem;
using Exiled.Permissions.Extensions;
using Exiled.API.Features;

namespace AdminTools.Commands.Tags
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Tags : ParentCommand, IUsageProvider
    {
        public Tags() => LoadGeneratedCommands();

        public override string Command { get; } = "tags";

        public override string[] Aliases { get; } = Array.Empty<string>();

        public override string Description { get; } = "Hides staff tags in the server";

        public string[] Usage { get; } = new string[] { "hide/show" };

        public override void LoadGeneratedCommands() 
        {
            RegisterCommand(new Hide());
            RegisterCommand(new Show());
        }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (sender.CheckPermission("at.tags"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            response = "Invalid subcommand. Available ones: hide, show";
            return false;
        }
    }
}
