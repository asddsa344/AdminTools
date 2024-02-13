namespace AdminTools
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using MEC;
    using Mirror;
    using UnityEngine;
    using Exiled.API.Extensions;
    using Exiled.API.Features.Items;
    using Exiled.API.Features.Doors;
    using Exiled.Events.EventArgs.Player;
    using Exiled.Events.EventArgs.Server;
    using InventorySystem.Items.Firearms.Attachments;
    using PlayerRoles;
    using Utils.NonAllocLINQ;
    using Exiled.API.Interfaces;
    using Log = Exiled.API.Features.Log;
    using Object = UnityEngine.Object;
    using Exiled.API.Features.Pickups.Projectiles;
    using PlayerStatsSystem;
    using HarmonyLib;

    public class EventHandlers
	{
		private readonly Main plugin;
		public EventHandlers(Main main) => plugin = main;

		public void OnInteractingDoor(InteractingDoorEventArgs ev)
		{
			if (Main.PryGateHubs.Contains(ev.Player) && ev.Door is Gate gate)
                gate.TryPry();
		}

        public static void OnHurting(HurtingEventArgs ev)
        {
            if (ev.Attacker != ev.Player && Main.IK.Contains(ev.Attacker))
                ev.Amount = StandardDamageHandler.KillValue;
        }
        public static string FormatArguments(ArraySegment<string> sentence, int index)
		{
			StringBuilder sb = new();
			foreach (string word in sentence.Segment(index))
			{
				sb.Append(word);
				sb.Append(" ");
			}
			string msg = sb.ToString();
			return msg;
		}

        public void OnPlayerDestroying(DestroyingEventArgs ev)
        {
			if (Main.RoundStartMutes.Remove(ev.Player))
            {
				ev.Player.IsMuted = false;
            }
			if (!Round.IsEnded)
				SavingPlayerData(ev.Player);
        }

        public static void SpawnWorkbench(Player ply, Vector3 position, Vector3 rotation, Vector3 size, out int benchIndex)
		{
			try
			{
				Log.Debug($"Spawning workbench");
				benchIndex = 0;
				GameObject bench =
					Object.Instantiate(
						NetworkManager.singleton.spawnPrefabs.Find(p => p.gameObject.name == "Work Station"));
				rotation.x += 180;
				rotation.z += 180;
                Offset offset = new()
                {
                    position = position,
                    rotation = rotation,
                    scale = Vector3.one,
                };
                bench.gameObject.transform.localScale = size;
				NetworkServer.Spawn(bench);
				if (Main.BchHubs.TryGetValue(ply, out List<GameObject> objs))
				{
					objs.Add(bench);
				}
				else
				{
					Main.BchHubs.Add(ply, new());
					Main.BchHubs[ply].Add(bench);
					benchIndex = Main.BchHubs[ply].Count();
				}

				if (benchIndex != 1)
					benchIndex = objs.Count();
				bench.transform.localPosition = offset.position;
				bench.transform.localRotation = Quaternion.Euler(offset.rotation);
				bench.AddComponent<WorkstationController>();
			}
			catch (Exception e)
			{
				Log.Error($"{nameof(SpawnWorkbench)}: {e}");
				benchIndex = -1;
			}
		}

		public void OnPlayerVerified(VerifiedEventArgs ev)
		{
			try
			{
				if (Main.JailedPlayers.Any(j => j.Userid == ev.Player.UserId))
					AdminTools.Commands.Jail.DoJail(ev.Player, true);

				if (ev.Player.RemoteAdminPermissions.HasFlag(PlayerPermissions.Overwatch) && Main.Overwatch.Contains(ev.Player.UserId))
				{
					Log.Debug($"Putting {ev.Player.UserId} into overwatch.");
					ev.Player.IsOverwatchEnabled = true;
				}

				if (Main.HiddenTags.Contains(ev.Player.UserId))
				{
					Log.Debug($"Hiding {ev.Player.UserId}'s tag.");
					Timing.CallDelayed(Timing.WaitForOneFrame, () => ev.Player.BadgeHidden = true);
				}

				if (Main.RoundStartMutes.Count != 0 && !ev.Player.RemoteAdminAccess && !Main.RoundStartMutes.Contains(ev.Player))
                {
					Log.Debug($"Muting {ev.Player.UserId} (no RA).");
					ev.Player.IsMuted = true;
					Main.RoundStartMutes.Add(ev.Player);
                }
			}
			catch (Exception e)
			{
				Log.Error($"Player Join: {e}");
			}
		}

		public void OnRoundStarted()
		{
			foreach (Player ply in Main.RoundStartMutes)
			{
				if (ply != null)
				{
					ply.IsMuted = false;
				}
			}
			Main.RoundStartMutes.Clear();
		}

		public void OnRoundEnded(RoundEndedEventArgs ev)
		{
			try
			{
				foreach (Player player in Player.List)
					SavingPlayerData(player);

                File.WriteAllLines(plugin.OverwatchFilePath, Main.Overwatch);
				File.WriteAllLines(plugin.HiddenTagsFilePath, Main.HiddenTags);

				// Update all the jails that it is no longer the current round, so when they are unjailed they don't teleport into the void.
				foreach (Jailed jail in Main.JailedPlayers)
				{
					if(jail.CurrentRound)
						jail.CurrentRound = false;
				}
			}
			catch (Exception e)
			{
				Log.Error($"Round End: {e}");
			}
		}
		public void SavingPlayerData(Player player)
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
        public void OnTriggerTesla(TriggeringTeslaEventArgs ev)
		{
			if (ev.Player.IsGodModeEnabled)
				ev.IsAllowed = false;
		}

		public void OnSetClass(ChangingRoleEventArgs ev)
		{
			if (plugin.Config.GodTuts)
				ev.Player.IsGodModeEnabled = ev.NewRole is RoleTypeId.Tutorial;
		}

        public void OnWaitingForPlayers()
		{
			Main.IK.Clear();
            Main.BreakDoors.Clear();
		}

		public void OnPlayerInteractingDoor(InteractingDoorEventArgs ev)
		{
			if (Main.BreakDoors.Contains(ev.Player) && ev.Door is IDamageableDoor damageableDoor)
                damageableDoor.Break();
        }
	}
}
