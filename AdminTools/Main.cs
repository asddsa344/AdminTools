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
using CommandSystem.Commands.RemoteAdmin.Doors;

namespace AdminTools
{
	public class Main : Plugin<Config>
	{
		public static System.Random NumGen { get; } = new();
		public static List<string> Overwatch { get; internal set; }
		public static List<string> HiddenTags { get; internal set; }
		public static Dictionary<string, Jailed> JailedPlayers { get; } = new();
		public static List<Player> PryGate { get; } = new();
		public static List<Player> InstantKill { get; } = new();
		public static List<Player> BreakDoors { get; } = new();
		public static List<Player> RoundStartMutes { get; } = new();
		public static Dictionary<Player, List<GameObject>> BchHubs { get; } = new();
		public static float HealthGain { get; } = 5;
		public static float HealthInterval { get; } = 1;
		public string OverwatchFilePath { get; private set; }
		public string HiddenTagsFilePath { get; private set; }
		public static Harmony Harmony { get; private set; }

		public EventHandlers EventHandlers { get; private set; }
		
		public override string Author { get; } = "Exiled-Team";
		public override string Name { get; } = "Admin Tools";
		public override string Prefix { get; } = "AT";
        public override Version Version { get; } = new(8, 0, 0);
        public override Version RequiredExiledVersion { get; } = new(8, 8, 0);

        public override void OnEnabled()
		{
			try
			{
				string path = Path.Combine(Paths.Configs, "AdminTools");
				string overwatchFileName = Path.Combine(path, "AdminTools-Overwatch.txt");
				string hiddenTagFileName = Path.Combine(path, "AdminTools-HiddenTags.txt");
                OverwatchFilePath = overwatchFileName;
                HiddenTagsFilePath = hiddenTagFileName;

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
            }
            catch (Exception e)
            {
                Log.Error($"Loading error: {e}");
            }

            EventHandlers = new(this);
            Harmony = new("ExiledTeam-AdminTools-" + DateTime.Now);

            if (Config.BetterCommand)
            {
	            Harmony.Patch(AccessTools.Method(typeof(RAUtils), nameof(RAUtils.ProcessPlayerIdOrNamesList)), new(AccessTools.Method(typeof(CustomRAUtilsAddon), nameof(CustomRAUtilsAddon.Prefix))));
                Harmony.Patch(AccessTools.Method(typeof(BaseDoorCommand), nameof(BaseDoorCommand.Execute)), transpiler: new(AccessTools.Method(typeof(DoorCommandPatche), nameof(DoorCommandPatche.Transpiler))));
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
            
            base.OnEnabled();
		}

		public override void OnDisabled()
		{
			Harmony?.UnpatchAll(Harmony.Id);

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
            Harmony = null;
            
            base.OnDisabled();
		}
	}
}