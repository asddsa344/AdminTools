using Exiled.API.Enums;
using System.Collections.Generic;
using UnityEngine;

namespace AdminTools
{
	using Exiled.API.Features.Items;
	using PlayerRoles;

	public class Jailed
	{
		public string Userid;
		public string Name;
		public List<Item> Items;
		public RoleTypeId Role;
		public Vector3 Position;
		public float Health;
		public Dictionary<AmmoType, ushort> Ammo;
		public bool CurrentRound;
	}
}