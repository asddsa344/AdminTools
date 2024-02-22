using System;
using System.Collections.Generic;
using System.IO;
using Exiled.API.Features;
using Handlers = Exiled.Events.Handlers;
using UnityEngine;
using HarmonyLib;
using Utils;
using System.Linq;
using AdminTools.Patches;

namespace AdminTools
{
	public class Main : Plugin<Config>
	{
		public override string Author { get; } = "Exiled-Team";
		public override string Name { get; } = "Admin Tools";
		public override string Prefix { get; } = "AT";
        public override Version Version { get; } = new(8, 0, 0);

        public override Version RequiredExiledVersion { get; } = new(8, 8, 0);
        public EventHandlers EventHandlers { get; private set; }
        public string HarmonyID { get; } = "AdminTools-" + DateTime.Now;
		public static System.Random NumGen { get; } = new();
        public static List<string> Overwatch { get; internal set; }
        public static List<string> HiddenTags { get; internal set; }
        public static List<Jailed> JailedPlayers { get; } = new();
		public static List<Player> PryGate { get; } = new();
		public static List<Player> InstantKill { get; } = new();
		public static List<Player> BreakDoors { get; } = new();
        public static List<Player> RoundStartMutes { get; } = new();
        public static Dictionary<Player, List<GameObject>> BchHubs { get; } = new();
        public static float HealthGain { get; } = 5;
		public static float HealthInterval { get; } = 1;
		public string OverwatchFilePath { get; private set; }
        public string HiddenTagsFilePath { get; private set; }
        public static Harmony harmony { get; private set; }
        public override void OnEnabled()
		{
			try
			{
				string path = Path.Combine(Paths.Configs, "AdminTools");
				string overwatchFileName = Path.Combine(path, "AdminTools-Overwatch.txt");
				string hiddenTagFileName = Path.Combine(path, "AdminTools-HiddenTags.txt");

                if (!Directory.Exists(path))
					Directory.CreateDirectory(path);

				if (!File.Exists(overwatchFileName))
					File.Create(overwatchFileName).Close();
				else
                    Overwatch = File.ReadAllLines(overwatchFileName).ToList();
                if (!File.Exists(hiddenTagFileName))
					File.Create(hiddenTagFileName).Close();
                else
                    HiddenTags = File.ReadAllLines(hiddenTagFileName).ToList();

                OverwatchFilePath = overwatchFileName;
				HiddenTagsFilePath = hiddenTagFileName;
            }
            catch (Exception e)
            {
                Log.Error($"Loading error: {e}");
            }

            EventHandlers = new EventHandlers(this);
            harmony = new(HarmonyID);

            if (Config.BetterCommand)
			{
                harmony.Patch(AccessTools.Method(typeof(RAUtils), nameof(RAUtils.ProcessPlayerIdOrNamesList)), new(AccessTools.Method(typeof(CustomRAUtilsAddon), nameof(CustomRAUtilsAddon.Prefix))));
            }

            Handlers.Player.Verified += EventHandlers.OnPlayerVerified;
            Handlers.Server.RoundEnded += EventHandlers.OnRoundEnded;
            Handlers.Player.TriggeringTesla += EventHandlers.OnTriggeringTesla;
            Handlers.Player.ChangingRole += EventHandlers.OnChangingRole;
            Handlers.Server.WaitingForPlayers += EventHandlers.OnWaitingForPlayers;
            Handlers.Player.InteractingDoor += EventHandlers.OnInteractingDoor;
            Handlers.Server.RoundStarted += EventHandlers.OnRoundStarted;
            Handlers.Player.Destroying += EventHandlers.OnPlayerDestroying;
            Handlers.Player.InteractingDoor += EventHandlers.OnPlayerInteractingDoor;
		}

		public override void OnDisabled()
		{
			harmony?.UnpatchAll(HarmonyID);

            Handlers.Player.InteractingDoor -= EventHandlers.OnInteractingDoor;
			Handlers.Player.Verified -= EventHandlers.OnPlayerVerified;
			Handlers.Server.RoundEnded -= EventHandlers.OnRoundEnded;
			Handlers.Player.TriggeringTesla -= EventHandlers.OnTriggeringTesla;
			Handlers.Player.ChangingRole -= EventHandlers.OnChangingRole;
            Handlers.Server.WaitingForPlayers -= EventHandlers.OnWaitingForPlayers;
			Handlers.Server.RoundStarted -= EventHandlers.OnRoundStarted;
			Handlers.Player.Destroying -= EventHandlers.OnPlayerDestroying;
            Handlers.Player.InteractingDoor -= EventHandlers.OnPlayerInteractingDoor;
            EventHandlers = null;
			harmony = null;
		}
	}
}