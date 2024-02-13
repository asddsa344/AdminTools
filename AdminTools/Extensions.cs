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
    }
}
