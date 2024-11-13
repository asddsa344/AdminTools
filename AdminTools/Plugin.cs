using System;
using System.Collections.Generic;
using System.IO;
using Exiled.API.Features;
using UnityEngine;
using HarmonyLib;
using Utils;
using System.Linq;
using AdminTools.Patches;
using CommandSystem.Commands.RemoteAdmin.Doors;
using Exiled.API.Enums;

namespace AdminTools
{
	public class Plugin : Plugin<Config>
	{
		public static string OverwatchFilePath;
		private Harmony Harmony = new("Exiled-AdminTools");
		public static List<string> Overwatch = new();
		public static Dictionary<string, Jailed> JailedPlayers = new();
		public static List<Player> PryGate = new();
		public static List<Player> InstantKill = new();
		public static List<Player> BreakDoors = new();
		public static List<Player> RoundStartMutes = new();
		public static Dictionary<Player, List<GameObject>> BchHubs = new();
		public static Plugin Instance;

		public override string Author => "Exiled-Team";
		
		public override string Name => "Admin Tools";
		
		public override string Prefix => "AdminTools";
		
		public override Version Version { get; } = new(7, 1, 4);

		public override Version RequiredExiledVersion { get; } = new(8, 8, 0);

        
		public override void OnEnabled()
		{
			string path = Path.Combine(Paths.Configs, "AdminTools");
			string overwatchFileName = Path.Combine(path, "AdminTools-Overwatch.txt");

			OverwatchFilePath = overwatchFileName;
			
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);

			if (!File.Exists(overwatchFileName))
				File.Create(overwatchFileName).Close();
			else
				Overwatch = File.ReadAllLines(overwatchFileName).ToList();

			Instance = this;

			if (Config.ExtendedCommandUsage)
			{
				Harmony.Patch(AccessTools.Method(typeof(RAUtils), nameof(RAUtils.ProcessPlayerIdOrNamesList)), new(AccessTools.Method(typeof(CustomRAUtilsAddon), nameof(CustomRAUtilsAddon.Prefix))));
				Harmony.Patch(AccessTools.Method(typeof(BaseDoorCommand), nameof(BaseDoorCommand.Execute)), transpiler: new(AccessTools.Method(typeof(DoorCommandPatch), nameof(DoorCommandPatch.Transpiler))));
			}

			base.OnEnabled();
		}

		public override void OnDisabled()
		{
			Harmony?.UnpatchAll(Harmony.Id);
			EventHandlers.UnRegisterEvents();
			Instance = null;
            
			base.OnDisabled();
		}
	}
}