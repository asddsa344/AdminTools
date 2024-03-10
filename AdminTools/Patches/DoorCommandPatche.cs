﻿using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using static HarmonyLib.AccessTools;
using CommandSystem.Commands.RemoteAdmin.Doors;
using Exiled.API.Features.Pools;
using PlayerRoles;
using Interactables.Interobjects.DoorUtils;
using Exiled.API.Features.Doors;
using Exiled.API.Enums;

namespace AdminTools.Patches
{
    internal static class DoorCommandPatche
    {
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label found = generator.DefineLabel();

            int offset = -1;
            int index = newInstructions.FindLastIndex(x => x.opcode == OpCodes.Ldloc_S && x.operand == (object)10) + offset;

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // doorVariant
                new CodeInstruction(OpCodes.Ldloc_S, 6).MoveLabelsFrom(newInstructions[index]),

                // array[]
                new(OpCodes.Ldloc_2),
                
                // text
                new(OpCodes.Ldarga, 9),

                // DoorCommandPatche.GetExiledDoor(doorvariant, array, ref text)
                new(OpCodes.Call, Method(typeof(DoorCommandPatche), nameof(DoorCommandPatche.GetExiledDoor))),
                new(OpCodes.Brtrue_S, found),
            });

            offset = 2;
            index = newInstructions.FindIndex(x => x.operand == (object)PropertyGetter(typeof(DoorNametagExtension), nameof(DoorNametagExtension.GetName))) + offset;

            newInstructions[index].labels.Add(found);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
        
        private static bool GetExiledDoor(DoorVariant doorVariant, string[] doors, ref string text)
        {
            DoorType searchdoortype = Door.Get(doorVariant).Type;
            foreach (string door in doors)
            {
                if (string.IsNullOrEmpty(door) || !Enum.TryParse(door, true, out DoorType doorType) || doorType != searchdoortype)
                    continue;
                text = ", " + doorType.ToString();
                return true;
            }

            return false;
        }
    }
}
