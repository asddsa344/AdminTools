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
    using Exiled.Events.Commands.Reload;

    public class EventHandlers
	{
		private readonly Main plugin;
		public EventHandlers(Main main) => plugin = main;

		public void OnInteractingDoor(InteractingDoorEventArgs ev)
		{
			if (Main.PryGate.Contains(ev.Player) && ev.Door is Gate gate)
                gate.TryPry();
		}

        public static void OnHurting(HurtingEventArgs ev)
        {
            if (ev.Attacker != ev.Player && Main.InstantKill.Contains(ev.Attacker))
                ev.Amount = StandardDamageHandler.KillValue;
        }

        public void OnPlayerDestroying(DestroyingEventArgs ev)
        {
			if (Main.RoundStartMutes.Remove(ev.Player))
            {
				ev.Player.IsMuted = false;
            }
			if (!Round.IsEnded)
				Extensions.SavingPlayerData(ev.Player);
        }

		public void OnPlayerVerified(VerifiedEventArgs ev)
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
			// Update all the jails that it is no longer the current round, so when they are unjailed they don't teleport into the void.
            foreach (Jailed jail in Main.JailedPlayers)
            {
                if (jail.CurrentRound)
                    jail.CurrentRound = false;
            }

            foreach (Player player in Player.List)
                Extensions.SavingPlayerData(player);

            File.WriteAllLines(plugin.OverwatchFilePath, Main.Overwatch);
            File.WriteAllLines(plugin.HiddenTagsFilePath, Main.HiddenTags);
        }

        public void OnTriggeringTesla(TriggeringTeslaEventArgs ev)
		{
			if (ev.Player.IsGodModeEnabled)
				ev.IsAllowed = false;
		}

		public void OnChangingRole(ChangingRoleEventArgs ev)
		{
			if (plugin.Config.GodTuts)
				ev.Player.IsGodModeEnabled = ev.NewRole is RoleTypeId.Tutorial;
		}

        public void OnWaitingForPlayers()
		{
			Main.InstantKill.Clear();
            Main.BreakDoors.Clear();
            Main.PryGate.Clear();
            if (plugin.Config.ClearJailsOnRestart)
                Main.JailedPlayers.Clear();
        }

        public void OnPlayerInteractingDoor(InteractingDoorEventArgs ev)
		{
			if (Main.BreakDoors.Contains(ev.Player) && ev.Door is IDamageableDoor damageableDoor)
                damageableDoor.Break();
        }
	}
}
