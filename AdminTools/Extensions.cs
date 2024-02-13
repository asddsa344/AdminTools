using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminTools
{
    public static class Extensions
    {
        public static string Fuckyou(IEnumerable<Player> players) => string.Join("\n - ", players.Select(x => $"{x.Nickname}({x.Id})"));

        public static void SavingPlayerData(Player player)
        {
            List<string> overwatchRead = Main.Overwatch;
            List<string> tagsRead = Main.HiddenTags;

            string userId = player.UserId;

            if (player.IsOverwatchEnabled && !overwatchRead.Contains(userId))
            {
                overwatchRead.Add(userId);
                Log.Debug($"{player.Nickname}({player.UserId}) has added their overwatch.");
            }
            else if (!player.IsOverwatchEnabled && overwatchRead.Remove(userId))
                Log.Debug($"{player.Nickname}({player.UserId}) has remove their overwatch.");

            if (player.BadgeHidden && !tagsRead.Contains(userId))
            {
                tagsRead.Add(userId);
                Log.Debug($"{player.Nickname}({player.UserId}) has added their tag hidden.");
            }
            else if (!player.BadgeHidden && tagsRead.Remove(userId))
                Log.Debug($"{player.Nickname}({player.UserId}) has remove their tag hidden.");
        }
    }
}
