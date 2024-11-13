using YamlDotNet.Serialization.NodeTypeResolvers;

namespace AdminTools;

using System.IO;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using PlayerRoles;
using Exiled.API.Interfaces;
using Log = Exiled.API.Features.Log;
using PlayerStatsSystem;
using Handlers = Exiled.Events.Handlers;
using Exiled.API.Enums;

public class EventHandlers
{
	public static void RegisterEvents()
	{
		Handlers.Player.Verified += OnPlayerVerified;
		Handlers.Server.RoundEnded += OnRoundEnded;
		Handlers.Player.TriggeringTesla += OnTriggeringTesla;
		Handlers.Server.WaitingForPlayers += OnWaitingForPlayers;
		Handlers.Player.InteractingDoor += OnInteractingDoor;
		Handlers.Server.RoundStarted += OnRoundStarted;
		Handlers.Player.Destroying += OnPlayerDestroying;
		Handlers.Player.Hurting += OnHurting;
		Handlers.Player.InteractingDoor += OnPlayerInteractingDoor;
		Handlers.Player.ChangingRole += OnChangingRole;
	}

	public static void UnRegisterEvents()
	{
		Handlers.Player.Verified -= OnPlayerVerified;
		Handlers.Server.RoundEnded -= OnRoundEnded;
		Handlers.Player.TriggeringTesla -= OnTriggeringTesla;
		Handlers.Player.ChangingRole -= OnChangingRole;
		Handlers.Server.WaitingForPlayers -= OnWaitingForPlayers;
		Handlers.Player.InteractingDoor -= OnInteractingDoor;
		Handlers.Server.RoundStarted -= OnRoundStarted;
		Handlers.Player.Destroying -= OnPlayerDestroying;
		Handlers.Player.Hurting -= OnHurting;
		Handlers.Player.InteractingDoor -= OnPlayerInteractingDoor;
		Handlers.Player.ChangingRole -= OnChangingRole;
	}

	private static void OnInteractingDoor(InteractingDoorEventArgs ev)
	{
		if (Plugin.PryGatePlayerList.Contains(ev.Player) && ev.Door is Gate gate)
			gate.TryPry();
	}

	private static void OnHurting(HurtingEventArgs ev)
	{
		if (ev.Attacker != ev.Player && Plugin.InstantKillPlayerList.Contains(ev.Attacker))
			ev.Amount = StandardDamageHandler.KillValue;
	}

	private static void OnPlayerDestroying(DestroyingEventArgs ev)
	{
		if (ev.Player == null)
			return;

		if (Plugin.RoundStartMutesList.Remove(ev.Player))
			ev.Player.IsMuted = false;

		if (!Round.IsEnded)
			ev.Player.SavingPlayerData();
	}

	private static void OnPlayerVerified(VerifiedEventArgs ev)
	{
		if (ev.Player is null)
			return;
		
		if (Plugin.JailedPlayers.ContainsKey(ev.Player.UserId))
			Commands.Jail.DoJail(ev.Player, true);

		if (ev.Player.RemoteAdminPermissions.HasFlag(PlayerPermissions.Overwatch) && Plugin.Overwatch.Contains(ev.Player.UserId))
		{
			Log.Debug($"Putting {ev.Player.UserId} into overwatch.");
			ev.Player.IsOverwatchEnabled = true;
		}

		if (Plugin.RoundStartMutesList.Count != 0 && !ev.Player.RemoteAdminAccess && !Plugin.RoundStartMutesList.Contains(ev.Player))
		{
			Log.Debug($"Muting {ev.Player.UserId} (no RA).");
			ev.Player.IsMuted = true;
			Plugin.RoundStartMutesList.Add(ev.Player);
		}
	}

	private static void OnRoundStarted()
	{
		foreach (Player ply in Plugin.RoundStartMutesList)
		{
			if (ply != null)
			{
				ply.IsMuted = false;
			}
		}

		Plugin.RoundStartMutesList.Clear();
	}

	private static void OnRoundEnded(RoundEndedEventArgs ev)
	{
		// Update all the jails that it is no longer the current round, so when they are unjailed they don't teleport into the void.
		foreach (Jailed jail in Plugin.JailedPlayers.Values)
		{
			if (jail.CurrentRound)
				jail.CurrentRound = false;
		}

		foreach (Player player in Player.List)
			player.SavingPlayerData();

		File.WriteAllLines(Plugin.OverwatchFilePath, Plugin.Overwatch);
	}

	private static void OnTriggeringTesla(TriggeringTeslaEventArgs ev)
	{
		if (ev.Player.IsGodModeEnabled)
			ev.IsAllowed = false;
	}

	private static void OnChangingRole(ChangingRoleEventArgs ev)
	{
		if (Plugin.Instance.Config.GodTuts && (ev.Reason is SpawnReason.ForceClass or SpawnReason.None))
			ev.Player.IsGodModeEnabled = ev.NewRole == RoleTypeId.Tutorial;
	}

	private static void OnWaitingForPlayers()
	{
		Plugin.InstantKillPlayerList.Clear();
		Plugin.BreakDoorsPlayerList.Clear();
		Plugin.PryGatePlayerList.Clear();

		if (Plugin.Instance.Config.ClearJailsOnRestart)
			Plugin.JailedPlayers.Clear();

		if (Plugin.Instance.Config.DisableLockOnWaiting)
		{
			Round.IsLobbyLocked = false;
			Round.IsLocked = false;
		}
	}

	private static void OnPlayerInteractingDoor(InteractingDoorEventArgs ev)
	{
		if (Plugin.BreakDoorsPlayerList.Contains(ev.Player) && ev.Door is IDamageableDoor damageableDoor)
			damageableDoor.Break();
	}
}